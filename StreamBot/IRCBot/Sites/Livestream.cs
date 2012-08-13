using System;
using HtmlAgilityPack;

namespace StreamBot.IRCBot.Sites
{
    public class Livestream : IStreamSite
    {
        public bool GetStatus(string link)
        {
            HtmlWeb web = new HtmlWeb();

            try
            {
                HtmlDocument doc = web.Load(link);
                if (doc.DocumentNode.SelectSingleNode("//span[@id='isLive']").InnerText == "true")
                {
                    Log.AddMessage("Stream " + link + " found online!");
                    return true;
                }

                Log.AddMessage("Stream " + link + " found offline.");
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
            }

            return false;
        }
    }
}