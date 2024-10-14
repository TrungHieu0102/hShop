

namespace Application.Common.Shared
{
    public class Message
    {
        public List<string> To { get; }
        public string Subject { get; }
        public string Content { get; }
        public List<string>? Attachments { get; }

        public Message(List<string> to, string subject, string content, List<string>? attachments = null)
        {
            To = to;
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
