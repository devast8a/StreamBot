using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Meebey.SmartIrc4net;
using StreamBot.IRCBot.Commands;

namespace StreamBot.IRCBot
{
    class CommandHandler
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public void Add(string text, ICommand command)
        {
            _commands.Add(text, command);
        }
 
        public string ParseCommand(string sender, Permission permission, string message)
        {
            var split = message.Split(' ');

            ICommand command;

            if(_commands.TryGetValue(split[0], out command))
            {
                return command.Parse(sender, permission, split);
            }

            return null;
        }
    }
}

