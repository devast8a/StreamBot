using System;
using StreamBot.IRCBot;

namespace StreamBot
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var bot = new Bot();
            bot.Connect();
        }
    }
}
