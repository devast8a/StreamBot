using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamBot.IRCBot
{
    internal class CommandDispatcher
    {
        private static readonly Regex StreamCommand = new Regex(@"^!stream\s+(\S+)\s*$", RegexOptions.Compiled);

        private static readonly Regex StreamAddCommand = new Regex(@"^!addstream\s+(.+)\s+(.+)\s*$",
            RegexOptions.Compiled);

        private static readonly Regex StreamDelCommand = new Regex(@"^!delstream\s+(.+)\s*$", RegexOptions.Compiled);
        private static readonly Regex StreamingCommand = new Regex(@"^!streaming\s+(.+)\s*$", RegexOptions.Compiled);
        private static readonly Regex AddOpCommand = new Regex(@"^!addop\s(\S+)\s*$", RegexOptions.Compiled);
        private static readonly Regex DelOpCommand = new Regex(@"^!delop\s(\S+)\s*$", RegexOptions.Compiled);

        private readonly ICommandHandler _handler;

        public CommandDispatcher(ICommandHandler handler)
        {
            _handler = handler;
        }

        public void Dispatch(CommandContext cc, string message)
        {
            if (StreamCommand.IsMatch(message))
            {
                _handler.Stream(cc);
            }
            else if (message.Trim() == "!streams")
            {
                _handler.Streams(cc);
            }
            else if (message.Trim() == "!about")
            {
                _handler.About(cc);
            }
            else if (message.Trim() == "!guide")
            {
                _handler.Guide(cc);
            }
            else if (message.Trim() == "!operators")
            {
                _handler.Operators(cc);
            }
            else if (message.Trim() == "!streamers")
            {
                _handler.Streamers(cc);
            }
            else if (StreamAddCommand.IsMatch(message))
            {
                _handler.AddStream(cc);
            }
            else if (StreamDelCommand.IsMatch(message))
            {
                _handler.DelStream(cc);
            }
            else if (StreamingCommand.IsMatch(message))
            {
                _handler.Streaming(cc);
            }
            else if (AddOpCommand.IsMatch(message))
            {
                _handler.AddOp(cc);
            }
            else if (DelOpCommand.IsMatch(message))
            {
                _handler.DelOp(cc);
            }
        }
    }
}