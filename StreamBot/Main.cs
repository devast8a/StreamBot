using System;

namespace StreamBot
{
	class MainClass
	{
        public static void Main(string[] args)
        {
            try
            {
                IRCBot.Bot.Start();
            }
            catch (Exception e)
            {
                IRCBot.Log.AddErrorMessage(e.Message);  
            }
        }
	}
}
