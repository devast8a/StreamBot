using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Owned
    {
        private static readonly Regex LinkId = new Regex(@"^.+/(?<id>\d+)/*$", RegexOptions.Compiled);

        public static bool GetStatus(string link)
        {
            string url;
            Match m = LinkId.Match(link);
            if (m.Success)
            {
                url = String.Format("http://api.own3d.tv/rest/live/status.xml?liveid={0}", m.Groups["id"].Value);
            }
            else
            {
                return false;
            }

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