using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Logging;
using Project1.ChatApi.Application.Interfaces;
using Project1.ChatApi.Domain;

namespace Project1.ChatApi.Infrastructure.AI
{
    public class SemanticKernelChatService : IChatService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<SemanticKernelChatService> _logger;

        private static readonly Dictionary<string, List<Domain.ChatMessage>> ChatSessions = new();

        public SemanticKernelChatService(Kernel kernel, ILogger<SemanticKernelChatService> logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.ChatMessage> SendChatMessageAsync(
            string userMessage,
            string chatSessionId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                throw new ArgumentException("User message cannot be empty.", nameof(userMessage));

            var sessionId = string.IsNullOrWhiteSpace(chatSessionId)
                ? Guid.NewGuid().ToString()
                : chatSessionId;

            _logger.LogInformation("Processing message for session {SessionId}", sessionId);

            try
            {
                if (!ChatSessions.ContainsKey(sessionId))
                {
                    ChatSessions[sessionId] = new List<Domain.ChatMessage>();
                }

                var userChatMessage = new Domain.ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Role = "user",
                    Content = userMessage,
                    CreatedAt = DateTime.UtcNow,
                    ChatSessionId = sessionId
                };
                ChatSessions[sessionId].Add(userChatMessage);

                var history = new ChatHistory();
                history.AddSystemMessage(
                    "You are a helpful AI assistant. Provide detailed, comprehensive answers. " +
                    "Respond clearly and concisely. If you don't know something, say so honestly.");

                foreach (var msg in ChatSessions[sessionId].Where(m => m.Role != "assistant" || m.Id != userChatMessage.Id))
                {
                    if (msg.Role == "user")
                    {
                        history.AddUserMessage(msg.Content);
                    }
                    else if (msg.Role == "assistant")
                    {
                        history.AddAssistantMessage(msg.Content);
                    }
                }

                // Get the chat completion service from the kernel
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Create execution settings to force a large response limit (~6000 words)
                var settings = new PromptExecutionSettings();
                settings.ExtensionData = new Dictionary<string, object>
                {
                    { "max_tokens", 8192 },
                    { "temperature", 0.7 }
                };

                // Call Gemini with the full conversation history AND the settings
                var response = await chatCompletionService.GetChatMessageContentAsync(
                    history,
                    executionSettings: settings,
                    kernel: _kernel,
                    cancellationToken: cancellationToken);

                // Extract the full response text
                var assistantResponseText = response.Content ?? "No response received.";

                _logger.LogInformation(
                    "Successfully generated response for session {SessionId}. Response length: {Length} characters",
                    sessionId,
                    assistantResponseText.Length);

                var assistantMessage = new Domain.ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Role = "assistant",
                    Content = assistantResponseText,
                    CreatedAt = DateTime.UtcNow,
                    ChatSessionId = sessionId
                };
                ChatSessions[sessionId].Add(assistantMessage);

                return assistantMessage;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed for session {SessionId}", sessionId);
                throw new InvalidOperationException("Failed to communicate with AI service", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing message for session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<Domain.ChatMessage>> GetChatHistoryAsync(string chatSessionId)
        {
            if (string.IsNullOrEmpty(chatSessionId))
            {
                throw new ArgumentException("Chat session ID cannot be empty", nameof(chatSessionId));
            }

            return await Task.FromResult(
                ChatSessions.ContainsKey(chatSessionId)
                    ? ChatSessions[chatSessionId].AsReadOnly()
                    : Enumerable.Empty<Domain.ChatMessage>());
        }

        public async Task ClearChatHistoryAsync(string chatSessionId)
        {
            if (string.IsNullOrEmpty(chatSessionId))
            {
                throw new ArgumentException("Chat session ID cannot be empty", nameof(chatSessionId));
            }

            if (ChatSessions.ContainsKey(chatSessionId))
            {
                ChatSessions.Remove(chatSessionId);
                _logger.LogInformation("Cleared chat history for session {SessionId}", chatSessionId);
            }

            await Task.CompletedTask;
        }
    }
}