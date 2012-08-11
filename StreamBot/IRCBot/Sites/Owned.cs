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

            try
            {
                XDocument doc = XDocument.Load(url);
                if (doc.Elements("lives").Elements("live_is_live").FirstOrDefault().Value == "1")
                {
                    Log.AddMessage("Stream " + url + " found online!");
                    return true;
                }

                Log.AddMessage("Stream " + url + " found offline.");
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
            }

            return false;
        }
    }
}

