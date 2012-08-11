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
            Log.StartLogging("streambot.log", 1000);
            Connection.Create();
        }
    }
}

