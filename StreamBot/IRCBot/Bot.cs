using System;

namespace StreamBot.IRCBot
{
    public class Bot
    {
        public static void Start()
        {
            Settings.LoadConfig();
            Settings.LoadStreams();
            Settings.LoadOps();
            Connection.Create();
        }
    }
}

