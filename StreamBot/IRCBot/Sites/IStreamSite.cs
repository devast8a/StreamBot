namespace StreamBot.IRCBot.Sites
{
    internal interface IStreamSite
    {
        bool GetStatus(string link);
    }
}