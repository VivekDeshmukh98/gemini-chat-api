namespace Project1.ChatApi.Features.Dtos
{
    /// <summary>
    /// Chat history response.
    /// </summary>
    public class ChatHistoryResponse
    {
        /// <summary>
        /// Conversation ID.
        /// </summary>
        public string ChatSessionId { get; set; } = string.Empty;
        /// <summary>
        /// List of chat messages.
        /// </summary>
        public List<ChatMessageDto> Messages { get; set; } = new();

    }
    /// <summary>
    /// Individual message in a chat history.
    /// </summary>
    public class ChatMessageDto
    {
        /// <summary>
        /// Message identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// "user", "assistant", or "system"
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// The message content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// When it was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

}
