namespace StreamBot.IRCBot
{
    public class UpdateStreamResult
    {
        public string Topic { get; private set; }
        public string Message { get; private set; }

        public UpdateStreamResult(string topic, string message)
        {
            Topic = topic;
            Message = message;
        }
    }
}