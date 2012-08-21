namespace StreamBot.IRCBot.Commands
{
    public interface ICommand
    {
        string Parse(string sender, Permission permission, CommandArgs args);
    }
}