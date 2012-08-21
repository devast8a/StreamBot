using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    class Respond : ICommand
    {
        private readonly string _text;

        public Respond(string text)
        {
            _text=text;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            return _text;
        }
    }
}
