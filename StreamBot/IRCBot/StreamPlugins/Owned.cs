using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.StreamPlugins
{
    public class Owned : IStreamPlugin
    {
        private readonly Regex _regex = new Regex(@"^.*own3d\.tv.+$", RegexOptions.Compiled);

        public bool UseFor(Stream stream)
        {
            return _regex.Match(stream.URL).Success;
        }

        private readonly Regex _linkId = new Regex(@"^.+/(?<id>\d+)/*$", RegexOptions.Compiled);

        public bool GetStatus(Stream stream)
        {
            var m = _linkId.Match(stream.URL);

            if (!m.Success)
            {
                throw new Exception("Invalid Owned link " + stream.URL);
            }
            
            var url = String.Format("http://api.own3d.tv/rest/live/status.xml?liveid={0}", m.Groups["id"].Value);

            var doc = XDocument.Load(url);
            var value = doc.Elements("lives").Elements("live_is_live").FirstOrDefault();
            return value != null && value.Value == "1";
        }
    }
}