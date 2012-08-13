using System.Text.RegularExpressions;

namespace StreamBot.IRCBot.Sites
{
    internal class SiteProvider
    {
        private static readonly Regex Livestream = new Regex(@"^.*Livestream\.com.+$", RegexOptions.Compiled);
        private static readonly Regex Twitch = new Regex(@"^.*Twitch\.tv.+$", RegexOptions.Compiled);
        private static readonly Regex Owned = new Regex(@"^.*own3d\.tv.+$", RegexOptions.Compiled);

        private static readonly Livestream LivestreamSite = new Livestream();
        private static readonly Twitch TwitchSite = new Twitch();
        private static readonly Owned OwnedSite = new Owned();

        public static IStreamSite GetSite(string url)
        {
            if (Livestream.IsMatch(url))
                return LivestreamSite;
            
            if (Twitch.IsMatch(url))
                return TwitchSite;
            
            if (Owned.IsMatch(url))
                return OwnedSite;

            throw new UnsupportedSiteException(url);
        }
    }
}