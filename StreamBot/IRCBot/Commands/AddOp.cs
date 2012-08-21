using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    internal class AddOp : ICommand
    {
        private readonly SettingsInstance _settings;

        public AddOp(SettingsInstance settings)
        {
            _settings = settings;
        }

        public string Parse(MessageSource sender, Permission permission, CommandArgs args)
        {
            if (args.Args.Count != 1)
            {
                return String.Format("Error - Usage {0} <hostname>", args.Name);
            }

            var hostPermission = _settings.GetPermission(sender.Hostname);

            // No permission setting exists for the sender
            if (hostPermission == null)
            {
                return "You are not authorized to do this.";
            }

            if (hostPermission.Value != PermissionType.SuperOperator)
            {
                return "You must be a super operator to use this command.";
            }

            var targetPermission = _settings.GetPermission(args.Args[0]);

            // A permission already exists for the target user
            if (targetPermission != null)
            {
                return "This hostname already has operator permissions.";
            }

            _settings.AddPermission(args.Args[0], PermissionType.Operator);

            return String.Format("Added operator permissions for {0}.", args.Args[0]);
        }
    }
}