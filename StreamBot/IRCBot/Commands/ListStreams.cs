using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    class ListStreams : ICommand
    {
        readonly StreamHandler _handler;

        public string Format = "{0}";
        public string ErrorMessage = "Sorry, no streamers matched.";
        private readonly Func<Stream, bool> _predicate;

        public ListStreams(StreamHandler handler) : this(handler, x => true){}
        public ListStreams(StreamHandler handler, Func<Stream, bool> predicate)
        {
            _handler = handler;
            _predicate=predicate;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            var ret = string.Join(", ", _handler.StreamList.Where(_predicate).Select(x => x.Name));

            if(string.IsNullOrWhiteSpace(ret))
            {
                return ErrorMessage;
            }

            return String.Format(Format, ret);
        }
    }
}
