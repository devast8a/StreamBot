using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    class Streaming : ICommand
    {
        private readonly StreamHandler _handler;

        public Streaming(StreamHandler handler)
        {
            _handler = handler;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            var stream = _handler.GetStream(sender);

            if(stream == null)
            {
                return "Sorry you don't seem to own a stream.";
            }

            stream.Subject = args.Full;

            _handler.StreamSubjectChange(stream);

            if(string.IsNullOrWhiteSpace(args.Full))
            {
                return "Cleared streaming subject";
            }

            return null;
        }
    }
}
