using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Owned : IStreamSite
    {
        private static readonly Regex LinkId = new Regex(@"^.+/(?<id>\d+)/*$", RegexOptions.Compiled);

        public bool GetStatus(string link)
        {
            string url;
            Match m = LinkId.Match(link);
            if (m.Success)
            {
                url = String.Format("http://api.own3d.tv/rest/live/status.xml?liveid={0}", m.Groups["id"].Value);
            }
            else
            {
                Log.AddErrorMessage("Invalid Owned link " + link);
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