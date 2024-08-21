namespace Cadenza.Services
{
    public class MessageService
    {
        private readonly List<string> _messages = new List<string>();

        public IReadOnlyList<string> Messages => _messages;

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }
    }
}