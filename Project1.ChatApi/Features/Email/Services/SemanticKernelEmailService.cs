using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Project1.ChatApi.Features.Email.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.ChatApi.Features.Email.Services
{
    public class SemanticKernelEmailService : IEmailGeneratorService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<SemanticKernelEmailService> _logger;

        public SemanticKernelEmailService(Kernel kernel, ILogger<SemanticKernelEmailService> logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EmailMessage> GenerateEmailAsync(EmailGenerationRequest request, CancellationToken cancellationToken = default)
        {
            ValidateRequest(request);

            _logger.LogInformation("Generating email for topic: {Topic}, Type: {EmailType}, Tone: {Tone}", request.Topic, request.EmailType, request.Tone);

            try
            {
                var prompt = BuildEmailPrompt(request);

                // 1. Create the settings to force a large response limit
                var settings = new PromptExecutionSettings();
                settings.ExtensionData = new Dictionary<string, object>
                {
                    { "max_tokens", 8192 },
                    { "temperature", 0.7 }
                };

                // 2. Call the AI with the settings injected into the arguments
                var response = await _kernel.InvokePromptAsync(
                    prompt,
                    new KernelArguments(settings),
                    cancellationToken: cancellationToken);

                var GeneratedContent = response.ToString() ?? "String generation failed";
                var (subject, body) = ParseEmailResponse(GeneratedContent);
                var tokenUsed = EstimateTokens(prompt) + EstimateTokens(GeneratedContent);

                var emailMessage = new EmailMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = subject,
                    Body = body,
                    Type = request.EmailType,
                    Tone = request.Tone,
                    GenerationPrompt = prompt,
                    GeneratedAt = DateTime.UtcNow,
                    TokensUsed = tokenUsed
                };

                _logger.LogInformation(
                    "Successfully generated email. Subject: {Subject}",
                    subject);

                return emailMessage;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed");
                throw new InvalidOperationException("Failed to communicate with AI service", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error generating email");
                throw;
            }
        }

        public async Task<IEnumerable<EmailMessage>> GetGeneratedEmailsVariationsAsync(EmailGenerationRequest request, int count = 3, CancellationToken cancellationToken = default)
        {
            if (count < 1 || count > 5)
                throw new ArgumentException("Count must be between 1 and 5", nameof(count));

            _logger.LogInformation("Generating {Count} email variations", count);

            var variations = new List<EmailMessage>();

            for (int i = 0; i < count; i++)
            {
                // Create a new object manually instead of using 'with'
                var variationRequest = new EmailGenerationRequest
                {
                    Topic = $"{request.Topic} (Variation {i + 1}/{count})",

                    // Manually copy the other properties over
                    EmailType = request.EmailType,
                    Tone = request.Tone,
                    Length = request.Length,
                    KeyPoints = request.KeyPoints,
                    RecipientInfo = request.RecipientInfo
                };

                var email = await GenerateEmailAsync(variationRequest, cancellationToken);
                variations.Add(email);
            }

            return variations;
        }

        private int EstimateTokens(string text) => Math.Max(1, text.Length / 4);

        private void ValidateRequest(EmailGenerationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Topic))
                throw new ArgumentException("Topic cannot be empty", nameof(request.Topic));
        }

        private string BuildEmailPrompt(EmailGenerationRequest request)
        {
            var prompt = $@"Generate a {request.EmailType} email with a {request.Tone} tone.

Topic: {request.Topic}
Length: {request.Length}

";

            if (!string.IsNullOrEmpty(request.KeyPoints))
                prompt += $"Key points: {request.KeyPoints}\n";

            if (!string.IsNullOrEmpty(request.RecipientInfo))
                prompt += $"Recipient: {request.RecipientInfo}\n";

            prompt += @"
Format as:
Subject: [subject line]

[email body]

No markdown. Plain text only.";

            return prompt;
        }

        private (string subject, string body) ParseEmailResponse(string response)
        {
            const string subjectPrefix = "Subject: ";
            var subjectIndex = response.IndexOf(subjectPrefix, StringComparison.OrdinalIgnoreCase);

            if (subjectIndex == -1)
                return ("No Subject", response);

            var subjectStart = subjectIndex + subjectPrefix.Length;
            var subjectEnd = response.IndexOf('\n', subjectStart);

            if (subjectEnd == -1)
                return (response.Substring(subjectStart).Trim(), "");

            var subject = response.Substring(subjectStart, subjectEnd - subjectStart).Trim();
            var body = response.Substring(subjectEnd).Trim();

            return (subject, body);
        }
    }
}