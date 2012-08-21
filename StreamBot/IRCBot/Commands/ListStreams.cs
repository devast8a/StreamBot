using System;
using System.Linq;

namespace StreamBot.IRCBot.Commands
{
    internal class ListStreams : ICommand
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

        public string Parse(MessageSource sender, CommandArgs args)
        {
            var ret = string.Join(", ", _handler.Matching(_predicate).Select(x => x.Name));

            if(string.IsNullOrWhiteSpace(ret))
            {
                return ErrorMessage;
            }

            return String.Format(Format, ret);
        }
    }
}
