using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace StreamBot.IRCBot.StreamPlugins
{
    internal class Twitch : IStreamPlugin
    {
        private readonly Regex _regex = new Regex(@"^.*twitch\.tv.+$", RegexOptions.Compiled);
        
        public bool UseFor(Stream stream)
        {
            return _regex.Match(stream.Url).Success;
        }

        private readonly Regex _linkId = new Regex(@"^.+/(?<id>\S+)/*$", RegexOptions.Compiled);

        public bool GetStatus(Stream stream)
        {
            Match m = _linkId.Match(stream.Url);

            if (!m.Success)
            {
                throw new Exception("Invalid Owned link " + stream.Url);
            }

            var url = String.Format("http://api.justin.tv/api/stream/list.xml?channel={0}", m.Groups["id"]);
            var doc = XDocument.Load(url);
            return doc.Descendants("streams").Elements("stream").Any();
        }
    }
}