namespace StreamBot.IRCBot.StreamPlugins
{
    internal interface IStreamPlugin
    {
        bool GetStatus(Stream stream);
        bool UseFor(Stream stream);
    }
}
