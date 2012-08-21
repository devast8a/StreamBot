using System;
using System.Collections.Generic;
using StreamBot.IRCBot.Commands;

namespace StreamBot.IRCBot
{
    internal class CommandHandler
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public void Add(string text, ICommand command)
        {
            _commands.Add(text, command);
        }

        public string ParseCommand(string sender, Permission permission, string message)
        {
            int firstSpace = message.IndexOf(' ');

            CommandArgs args;

            if (firstSpace == -1)
            {
                // TODO: Revisit this new string[0] thing; preferrably should pass null or something
                args = new CommandArgs(message, new string[0], String.Empty);
            }
            else
            {
                string full = message.Substring(firstSpace + 1).Trim();
                args = new CommandArgs(message.Substring(0, firstSpace), full.Split(), full);
            }

            ICommand command;

            if (_commands.TryGetValue(args.Name, out command))
            {
                return command.Parse(sender, permission, args);
            }

            return null;
        }
    }
}