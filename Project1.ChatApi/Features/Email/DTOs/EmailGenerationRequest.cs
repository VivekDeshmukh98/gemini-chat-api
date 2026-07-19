namespace Project1.ChatApi.Features.Email.DTOs
{
    public class EmailGenerationRequest
    {
        public string Topic { get; set; } = string.Empty;
        public string EmailType { get; set; }= "professional"; // "professional", "marketing", "personal"
        public string Tone { get; set; } = "professional"; // "formal", "friendly", "urgent"
        public string KeyPoints { get; set; } = string.Empty;
        public string Length { get; set; } = "medium";
        public string RecipientInfo { get; set; } = string.Empty; 
    }
}
