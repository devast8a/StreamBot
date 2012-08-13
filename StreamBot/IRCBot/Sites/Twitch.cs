using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Twitch
    {
        public static readonly Regex LinkId = new Regex(@"^.+/(?<id>\S+)/*$", RegexOptions.Compiled);

        public static bool GetStatus(string link)
        {
            string url;
            Match m = LinkId.Match(link);
            if (m.Success)
            {
                url = String.Format("http://api.justin.tv/api/stream/list.xml?channel={0}", m.Groups["id"]);
            }
            else
            {
                Log.AddErrorMessage("Invalid Twitch link " + link);
                return false;
            }

            try
            {
                XDocument doc = XDocument.Load(url);
                if (doc.Descendants("streams").Elements("stream").Any())
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