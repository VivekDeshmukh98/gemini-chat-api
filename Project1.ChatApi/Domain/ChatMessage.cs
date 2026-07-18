using System.Xml;

namespace Project1.ChatApi.Domain
{
    //Domain entity representing a single message in a chat conversation.
    public class ChatMessage
    {
        //Unique identifier for each message.
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // Role          : Identifies the sender (User, Assistant, System).
        public string Role { get; set; }=string.Empty;
        // Content       : The actual message text.
        public string   Content{ get; set; }=string.Empty;
        // CreatedAt     : UTC timestamp when the message was created.
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;
        // ChatSessionId : Groups messages belonging to the same conversation/session.
        public string ChatSessionId { get; set; } = string.Empty;
    }
}
