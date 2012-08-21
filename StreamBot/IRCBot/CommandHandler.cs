using System.Collections.Generic;
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
            int firstSpace = message.IndexOf(' ');

            var args = new CommandArgs();

            if(firstSpace == -1){
                args.Name = message;
                args.Full = "";
                args.Args = new string[0];
            }else
            {
                args.Name = message.Substring(0, firstSpace);
                args.Full = message.Substring(firstSpace + 1).Trim();
                args.Args = args.Full.Split(' ');
            }

            ICommand command;

            if(_commands.TryGetValue(args.Name, out command))
            {
                return command.Parse(sender, permission, args);
            }

            return null;
        }
    }
}

