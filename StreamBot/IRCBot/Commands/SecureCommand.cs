using System;

namespace StreamBot.IRCBot.Commands
{
    internal class SecureCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly Func<MessageSource, bool> _filter;
        public string ErrorText = "You do not have permission to use this command.";

        public SecureCommand(Func<MessageSource, bool> filter, ICommand command)
        {
            _filter = filter;
            _command = command;
        }

        public string Parse(MessageSource source, CommandArgs args)
        {
            if (_filter(source))
            {
                return _command.Parse(source, args);
            }

            return ErrorText;
        }
    }
}
