using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project1.ChatApi.Features.Email.DTOs;

namespace Project1.ChatApi.Features.Email.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailGeneratorService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailGeneratorService emailService,
            ILogger<EmailController> logger)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(EmailGenerationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateEmail(
            [FromBody] EmailGenerationRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Topic))
            {
                _logger.LogWarning("Invalid email generation request");
                return BadRequest(new { error = "Topic is required" });
            }

            try
            {
                _logger.LogInformation("Generating email. Type: {Type}, Tone: {Tone}",
                    request.EmailType, request.Tone);

                var email = await _emailService.GenerateEmailAsync(request, cancellationToken);

                var response = new EmailGenerationResponse
                {
                    EmailId = email.Id,
                    Subject = email.Subject,
                    Body = email.Body,
                    EmailType = email.Type,
                    Tone = email.Tone,
                    GeneratedAt = email.GeneratedAt,
                    Stats = new EmailGenerationStats
                    {
                        TotalTokens = email.TokensUsed,
                        WordCount = email.Body.Split(' ').Length,
                        GenerationTimeMs = 0
                    }
                };

                _logger.LogInformation("Email generated successfully");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service error");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "AI service is unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred" });
            }
        }

        [HttpPost("generate-variations")]
        public async Task<IActionResult> GenerateVariations(
            [FromBody] EmailVariationsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var emails = await _emailService.GetGeneratedEmailsVariationsAsync(
                    request.GenerationRequest,
                    request.VariationCount,
                    cancellationToken);

                var responses = emails.Select(email => new EmailGenerationResponse
                {
                    EmailId = email.Id,
                    Subject = email.Subject,
                    Body = email.Body,
                    EmailType = email.Type,
                    Tone = email.Tone,
                    GeneratedAt = email.GeneratedAt,
                    Stats = new EmailGenerationStats
                    {
                        TotalTokens = email.TokensUsed,
                        WordCount = email.Body.Split(' ').Length,
                        GenerationTimeMs = 0
                    }
                }).ToList();

                return Ok(responses);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating variations");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred" });
            }
        }
    }

    public class EmailVariationsRequest
    {
        public EmailGenerationRequest GenerationRequest { get; set; }
        public int VariationCount { get; set; } = 3;
    }
}