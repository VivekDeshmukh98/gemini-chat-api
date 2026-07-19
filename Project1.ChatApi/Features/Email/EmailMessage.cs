namespace Project1.ChatApi.Features.Email
{
    /// <summary>
    /// Domain entity representing a generated email message.
    /// </summary>
    public class EmailMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; }= string.Empty;// "professional", "marketing", "personal"
        public string Tone { get; set; } = string.Empty; // "formal", "friendly", "urgent"
        public string GenerationPrompt { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }=DateTime.UtcNow;
        public int TokensUsed { get; set; }
    }
}
