using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot
{
    internal class MessageSource
    {
        public string Hostname { get; private set; }
        public string Nickname { get; private set; }

        public MessageSource(string hostname, string nickname)
        {
            Hostname = hostname;
            Nickname = nickname;
        }
    }
}