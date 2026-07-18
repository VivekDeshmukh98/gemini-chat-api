using Project1.ChatApi.Domain;

namespace Project1.ChatApi.Application.Interfaces
{
    //Contract for chat operations.
    public interface IChatService
    {
        //Send a message to the AI and get a response.
        /// <summary>
        /// Sends a message to the AI and retrieves its response.
        /// </summary>
        /// <param name="userMessage">The message sent by the user.</param>
        /// <param name="chatSessionId">The ID of the current conversation.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The AI's reply.</returns>
        Task<ChatMessage> SendChatMessageAsync(string userMessage,string chatSessionId, CancellationToken cancellationToken=default);

        /// <summary>
        /// Get the conversation history for a specific chat session.
        /// </summary>
        /// <param name="chatSessionId">Which conversation to retrieve</param>
        /// <returns>All messages in the conversation, in order</returns>
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string chatSessionId);

        /// <summary>
        /// Clear the conversation history for a session.
        /// </summary>
        /// <param name="chatSessionId">Which conversation to clear</param>
        Task ClearChatHistoryAsync(string chatSessionId);

    }
}
