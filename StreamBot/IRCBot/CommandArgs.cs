using System;
using System.Collections.ObjectModel;

namespace StreamBot.IRCBot
{
    internal class CommandArgs
    {
        private readonly string[] _args;

        public string Name { get; private set; }
        public string Full { get; private set; }
        public ReadOnlyCollection<string> Args { get { return Array.AsReadOnly(_args); } }

        public CommandArgs(string name, string[] args, string full)
        {
            Name = name;
            _args = args;
            Full = full;
        }
    }
}