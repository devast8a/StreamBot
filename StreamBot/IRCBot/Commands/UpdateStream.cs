namespace StreamBot.IRCBot.Commands
{
    public class UpdateStream : ICommand
    {
        private readonly StreamHandler _handler;

        public UpdateStream(StreamHandler handler)
        {
            _handler=handler;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            Stream stream;

            if(args.Args.Length != 0)
            {
                if(!permission.Operator)
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
                stream = _handler.GetStream(sender);

                if (stream == null)
                {
                    return "Sorry you don't seem to own a stream.";
                }
            }

            _handler.Logger.AddMessage("Manual update for " + stream.Name);
            stream.Update(_handler);
            return "Updating " + stream.Name;
        }
    }
}