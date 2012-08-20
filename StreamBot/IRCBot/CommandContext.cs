using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meebey.SmartIrc4net;

namespace StreamBot.IRCBot
{
    public class CommandContext
    {
        private readonly IrcClient _client;

        public string Sender { get; private set; }

        public string Channel { get; private set; }
        public bool IsPrivate { get; private set; }

        public CommandContext(IrcClient client, string sender, string source)
        {
            _client = client;
            Sender = sender;

            if (source.StartsWith("#"))
            {
                Channel = source;
                IsPrivate = false;
            }
            else
            {
                Channel = String.Empty;
                IsPrivate = true;
            }
        }

        public void Reply(string message)
        {
            if (IsPrivate)
            {
                _client.RfcPrivmsg(Sender, message);
            }
            else
            {
                _client.RfcPrivmsg(Channel, String.Format("{0}: {1}", Sender, message));
            }
        }
    }
}