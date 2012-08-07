using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Owned
    {
        public static bool GetStatus(string link)
        {
            string url = "http://api.own3d.tv/rest/live/status.xml?liveid=";
            url += Regex.Match(link, @"^.+/(\d+)/*$").Groups [1].Value;

            XDocument doc = XDocument.Load(url);
            if (doc.Elements("lives").Elements("live_is_live").FirstOrDefault().Value == "1")
                return true;

            return false;
        }
    }
}

