namespace Project1.ChatApi.Features.Dtos
{
    /// <summary>
    /// Response returned by the chat API.
    /// </summary>
    public class ChatResponse
    {
        /// <summary>
        /// Conversation ID.
        /// </summary>
        public string ChatSessionId { get; set; } = string.Empty;
        /// <summary>
        /// User's message.
        /// </summary>
        public string UserMessage { get; set; } = string.Empty;
        /// <summary>
        /// AI's response.
        /// </summary>
        public string AssistantMessage { get; set; } = string.Empty;

        /// <summary>
        /// Response creation time (UTC).
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
