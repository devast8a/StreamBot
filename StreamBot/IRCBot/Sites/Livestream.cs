using System;
using HtmlAgilityPack;

namespace StreamBot.IRCBot.Sites
{
    public class Livestream
    {
        public static bool GetStatus(string link)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);

            if (doc.DocumentNode.SelectSingleNode("//span[@id='isLive']").InnerText == "true")
                return true;

            return false;
        }
    }
}