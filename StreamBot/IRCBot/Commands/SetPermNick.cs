using StreamBot.IRCBot.Commands;

namespace StreamBot.IRCBot
{
    internal class SetPermNick : ICommand
    {
        private readonly SettingsInstance _settings;

        public SetPermNick(SettingsInstance settings)
        {
            _settings=settings;
        }

        public string Parse(MessageSource source, CommandArgs args)
        {
            string hostname = source.Hostname;

            if(args.Args.Count == 2 && source.Permission.IsSuperOperator)
            {
                hostname = args.Args[1];
            }else if(args.Args.Count != 1)
            {
                return "Usage - " + args.Name + " <nickname>";
            }


            return _settings.SetUserPermissionNick(hostname, args.Args[0]) ? "Changed nick to " + args.Args[0] : "You don't seem to have a permission set";
        }
    }
}