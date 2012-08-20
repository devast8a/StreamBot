using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot {
    interface ICommandHandler
    {
        // !stream
        void Stream(CommandContext cc);

        // !streams
        void Streams(CommandContext cc);

        // !about
        void About(CommandContext cc);

        // !guide
        void Guide(CommandContext cc);

        // !operators
        void Operators(CommandContext cc);

        // !streamers
        void Streamers(CommandContext cc);

        // !addstream
        void AddStream(CommandContext cc);

        // !delstream
        void DelStream(CommandContext cc);

        // !streaming
        void Streaming(CommandContext cc);

        // !addop
        void AddOp(CommandContext cc);

        // !delop
        void DelOp(CommandContext cc);
    }
}
