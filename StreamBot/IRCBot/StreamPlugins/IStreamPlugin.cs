namespace StreamBot.IRCBot.StreamPlugins
{
    public interface IStreamPlugin
    {
        bool GetStatus(Stream stream);
        bool UseFor(Stream stream);
    }
}
