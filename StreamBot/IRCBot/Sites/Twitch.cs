using System;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Twitch
    {
        public static bool GetStatus(string link)
        {
            string url = "http://api.justin.tv/api/stream/list.xml?channel=";
            url += Regex.Match(link, @"^.+/(\S+)/*$").Groups [1].Value;

            XDocument doc = XDocument.Load(url);
            if (doc.Descendants("streams").Elements("stream").Any() == true)
            {
                Log.AddMessage("Stream " + url + " found online!");
                return true;
            }

            Log.AddMessage("Stream " + url + " found offline.");

            return false;
        }
    }
}

