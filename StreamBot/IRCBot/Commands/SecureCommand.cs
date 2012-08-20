using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    public class SecureCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly Func<Permission, bool> _filter;
        public string ErrorText;

        public SecureCommand(Func<Permission, bool> filter, ICommand command)
        {
            _filter = filter;
            _command = command;
        }

        public string Parse(string sender, Permission permission, string[] arguments)
        {
            if(_filter(permission))
            {
                return _command.Parse(sender, permission, arguments);
            }

            return ErrorText;
        }
    }
}
