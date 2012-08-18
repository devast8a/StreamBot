using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    public class StreamInfo : ICommand
    {
        private readonly StreamHandler _handler;

        public StreamInfo(StreamHandler handler)
        {
            _handler = handler;
        }

        public string Parse(string sender, string[] arguments)
        {
            var stream = _handler.GetStream(arguments[1]);

            if(stream == null)
            {
                return "No streamer by that name exists";
            }

            return string.Format("{0} - {1} - {2}", stream.Name, stream.URL, stream.Online ? "Online" : "Offline");
        }
    }
}
