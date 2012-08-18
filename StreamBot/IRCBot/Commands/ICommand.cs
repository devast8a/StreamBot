namespace StreamBot.IRCBot.Commands
{
    public interface ICommand
    {
        string Parse(string sender, string[] arguments);
    }
}