namespace StreamBot.IRCBot.Commands
{
    public class RemoveStream : ICommand
    {
        readonly StreamHandler _handler;
        private readonly Settings _settings;

        public RemoveStream(StreamHandler handler, Settings settings)
        {
            _handler=handler;
            _settings = settings;
        }

        public string Parse(string sender, Permission permission, string[] arguments)
        {
            if (arguments.Length == 2)
            {
                var resp = _handler.StreamList.RemoveAll((x) => x.Name == arguments[1]) > 0 ? "Deleted " + arguments[1] : "No streamer exists by that name";
                _settings.SaveStreams();
                return resp;
            }

            return "Error - Usage !remstream <streamer-name>";
        }
    }
}