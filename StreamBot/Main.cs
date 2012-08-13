using System;
using StreamBot.IRCBot;

namespace StreamBot
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
            	new Bot().Start();
	        }
        	catch (Exception e)
        	{
            	Log.AddErrorMessage(e.Message);  
            }
        }
    }
}
