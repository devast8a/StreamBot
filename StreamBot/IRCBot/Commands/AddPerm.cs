using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    internal class AddPerm : ICommand
    {
        private readonly SettingsInstance _settings;

        public AddPerm(SettingsInstance settings)
        {
            _settings = settings;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            Console.WriteLine(args.Args.Count);
            if (args.Args.Count < 1 || args.Args.Count > 2)
            {
                return String.Format("Error - Usage {0} <hostname> [permission]", args.Name);
            }

            string permission = "Operator";

            if(args.Args.Count == 2)
            {
                permission = args.Args[1];
            }

            var targetPermission = _settings.GetUserPermission(args.Args[0]);

            // A permission already exists for the target user
            if (targetPermission != null)
            {
                return "This hostname already has permissions attached.";
            }

            var perm = Permission.GetPermission(permission);

            if(perm == null)
            {
                return "No permission exists by that name";
            }

            _settings.AddUserPermission(args.Args[0], perm);

            return String.Format("Added permissions for {0}.", args.Args[0]);
        }
    }
}