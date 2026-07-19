using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project1.ChatApi.Features.Chat;
using Project1.ChatApi.Features.Chat.Dtos;

namespace Project1.ChatApi.Features.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Send a message to the AI and get a response.
        /// 
        /// POST /api/chat/send
        /// Body: { "message": "Hello AI", "chatSessionId": "optional-session-id" }
        /// Response: ChatResponse with AI's reply
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            //validate request
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Invalid chat request received.");
                return BadRequest(new { Error = "Message cannot be empty." });
            }
            try
            {
                _logger.LogInformation("Received message: {MessagePreview}...", request.Message[..Math.Min(50, request.Message.Length)], request.ChatSessionId);

                //Call the chat service to send the message and get a response
                var assistantMessage = await _chatService.SendChatMessageAsync(request.Message, request.ChatSessionId ?? string.Empty, cancellationToken);

                //Build response DTO
                var response = new ChatResponse
                {
                    ChatSessionId = assistantMessage.ChatSessionId,
                    UserMessage = request.Message,
                    AssistantMessage = assistantMessage.Content,
                    TimeStamp = assistantMessage.CreatedAt
                };
                _logger.LogInformation("Successfully processed message for session {SessionId}.", response.ChatSessionId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument provided: {ErrorMessage}", ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation performed: {ErrorMessage}", ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the chat message.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Get the complete chat history for a session.
        /// 
        /// GET /api/chat/history/{chatSessionId}
        /// Response: ChatHistoryResponse with all messages in the session
        /// </summary>
        [HttpGet("history/{chatSessionId}")]
        [ProducesResponseType(typeof(ChatHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChatHistory(string chatSessionId)
        {
            if (string.IsNullOrEmpty(chatSessionId))
            {
                return BadRequest(new { error = "Chat session ID is required" });
            }

            try
            {
                var messages = await _chatService.GetChatHistoryAsync(chatSessionId);

                var response = new ChatHistoryResponse
                {
                    ChatSessionId = chatSessionId,
                    Messages = messages
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new ChatMessageDto
                        {
                            Id = m.Id,
                            Role = m.Role,
                            Content = m.Content,
                            CreatedAt = m.CreatedAt
                        })
                        .ToList()
                };

                if (!response.Messages.Any())
                {
                    _logger.LogInformation("No chat history found for session {SessionId}", chatSessionId);
                    return NotFound(new { error = "Chat session not found" });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat history for session {SessionId}", chatSessionId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error retrieving chat history" });
            }
        }

        /// <summary>
        /// Clear the chat history for a session.
        /// 
        /// DELETE /api/chat/clear/{chatSessionId}
        /// </summary>
        [HttpDelete("clear/{chatSessionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearChatHistory(string chatSessionId)
        {
            if (string.IsNullOrEmpty(chatSessionId))
            {
                return BadRequest(new { error = "Chat session ID is required" });
            }

            try
            {
                await _chatService.ClearChatHistoryAsync(chatSessionId);
                _logger.LogInformation("Cleared chat history for session {SessionId}", chatSessionId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing chat history for session {SessionId}", chatSessionId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Error clearing chat history" });
            }
        }
    }
}

