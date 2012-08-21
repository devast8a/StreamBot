using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    internal class DelPerm : ICommand
    {
        private readonly SettingsInstance _settings;

        public DelPerm(SettingsInstance settings)
        {
            _settings = settings;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            if (args.Args.Count != 1)
            {
                return String.Format("Error - Usage {0} <hostname>", args.Name);
            }

            _settings.RemoveUserPermission(args.Args[0]);

            return String.Format("Removed permissions for {0}.", args.Args[0]);
        }
    }
}