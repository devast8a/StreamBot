using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    class Respond : ICommand
    {
        private string _text;

        public Respond(string text)
        {
            _text=text;
        }

        public string Parse(string sender, string[] arguments)
        {
            return _text;
        }
    }
}
