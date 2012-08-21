namespace StreamBot.IRCBot.Commands
{
    public class StreamInfo : ICommand
    {
        private readonly StreamHandler _handler;

        public StreamInfo(StreamHandler handler)
        {
            _handler = handler;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            if(args.Args.Length != 1)
            {
                return string.Format("Error - Usage {0} <stream-name>", args.Name);
            }

            var stream = _handler.GetStream(args.Args[0]);

            if(stream == null)
            {
                return "No streamer by that name exists";
            }

            var subject = "";

            if(!string.IsNullOrWhiteSpace(stream.Subject))
            {
                subject = " - Streaming: " + stream.Subject;
            }

            return string.Format("{0} - {1} - {2}{3}", stream.Name, stream.Url, stream.Online ? "Online" : "Offline", subject);
        }
    }
}
