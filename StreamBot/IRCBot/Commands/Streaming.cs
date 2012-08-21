namespace StreamBot.IRCBot.Commands
{
    internal class Streaming : ICommand
    {
        private readonly StreamHandler _handler;

        public Streaming(StreamHandler handler)
        {
            _handler = handler;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            var stream = _handler.GetStream(sender.Nickname);

            if(stream == null)
            {
                return "Sorry you don't seem to own a stream.";
            }

            stream.Subject = args.Full;

            _handler.StreamSubjectChange(stream);

            if(string.IsNullOrWhiteSpace(args.Full))
            {
                return "Cleared streaming subject";
            }

            return null;
        }
    }
}
