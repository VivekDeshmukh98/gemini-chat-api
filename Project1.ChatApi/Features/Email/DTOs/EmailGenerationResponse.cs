namespace Project1.ChatApi.Features.Email.DTOs
{
    public class EmailGenerationResponse
    {
        public string EmailId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string EmailType { get; set; } = string.Empty;
        public string Tone { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public EmailGenerationStats Stats { get; set; } = new();
    }
    public class EmailGenerationStats
    {
        public int TotalTokens { get; set; }
        public int WordCount { get; set; }
        public long GenerationTimeMs { get; set; }
    }
}
