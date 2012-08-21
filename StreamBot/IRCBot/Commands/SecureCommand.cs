using System;

namespace StreamBot.IRCBot.Commands
{
    internal class SecureCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly Func<Permission, bool> _filter;
        public string ErrorText;

        public SecureCommand(Func<Permission, bool> filter, ICommand command)
        {
            _filter = filter;
            _command = command;
        }

        public string Parse(MessageSource sender, Permission permission, CommandArgs args)
        {
            if(_filter(permission))
            {
                return _command.Parse(sender, permission, args);
            }

            return ErrorText;
        }
    }
}
