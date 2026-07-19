using Project1.ChatApi.Features.Email.DTOs;

namespace Project1.ChatApi.Features.Email
{
    public interface IEmailGeneratorService
    {
        Task<EmailMessage> GenerateEmailAsync(EmailGenerationRequest request, CancellationToken cancellationToken = default);

        Task<IEnumerable<EmailMessage>> GetGeneratedEmailsVariationsAsync(
            EmailGenerationRequest request,
            int count = 3,
            CancellationToken cancellationToken = default);
    }
}
