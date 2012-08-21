using log4net;

namespace StreamBot.IRCBot.Commands
{
    internal class UpdateStream : ICommand
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UpdateStream));

        private readonly StreamHandler _handler;

        public UpdateStream(StreamHandler handler)
        {
            _handler=handler;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            Stream stream;

            if(args.Args.Count != 0)
            {
                if(!sender.Permission.IsOperator)
                {
                    return string.Format("Error - usage {0}. Operators may force updates of other streamers", args.Name);
                }

                stream = _handler.GetStream(args.Args[0]);

                if (stream == null)
                {
                    return "Sorry no streamer exists with that name.";
                }
            }
            else
            {
                stream = _handler.GetStream(sender.Nickname);

                if (stream == null)
                {
                    return "Sorry you don't seem to own a stream.";
                }
            }

            Logger.Info("Manual update for " + stream.Name);
            stream.Update(_handler);
            return "Updating " + stream.Name;
        }
    }
}