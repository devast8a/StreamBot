namespace StreamBot.IRCBot.Commands
{
    internal class DelStream : ICommand
    {
        readonly StreamHandler _handler;
        private readonly SettingsInstance _settings;

        public DelStream(StreamHandler handler, SettingsInstance settings)
        {
            _handler=handler;
            _settings = settings;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            if (args.Args.Count == 1)
            {
                var name = args.Args[0];
                var resp = _handler.RemoveAll(a => a.Name == name) > 0 ? "Deleted " + name : "No streamer exists by that name";
                _settings.RemoveStream(name);
                return resp;
            }

            return string.Format("Error - Usage {0} <streamer-name>", args.Name);
        }
    }
}