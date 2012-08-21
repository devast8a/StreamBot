namespace StreamBot.IRCBot.Commands
{
    public class RemoveStream : ICommand
    {
        readonly StreamHandler _handler;
        private readonly SettingsInstance _settings;

        public RemoveStream(StreamHandler handler, SettingsInstance settings)
        {
            _handler=handler;
            _settings = settings;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            if (args.Args.Length == 1)
            {
                var name = args.Args[0];
                var resp = _handler.StreamList.RemoveAll((x) => x.Name == name) > 0 ? "Deleted " + name : "No streamer exists by that name";
                _settings.RemoveStream(name);
                return resp;
            }

            return "Error - Usage !remstream <streamer-name>";
        }
    }
}