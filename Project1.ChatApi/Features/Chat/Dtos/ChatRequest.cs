using System.Security.Principal;

namespace Project1.ChatApi.Features.Chat.Dtos
{
    /// <summary>
    /// Request sent by the user to the chat API.
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// The user's message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Conversation ID. Leave empty to start a new chat.
        /// </summary>
        public string? ChatSessionId { get; set; }
    }
}
