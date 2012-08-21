using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace StreamBot.IRCBot.StreamPlugins
{
    public class Livestream : IStreamPlugin
    {
        private readonly Regex _regex = new Regex(@"^.*livestream\.com.+$", RegexOptions.Compiled);

        public bool UseFor(Stream stream)
        {
            return _regex.Match(stream.Url).Success;
        }

        public bool GetStatus(Stream stream)
        {
            var web = new HtmlWeb();
            var doc = web.Load(stream.Url);
            return doc.DocumentNode.SelectSingleNode("//span[@id='isLive']").InnerText == "true";
        }
    }
}