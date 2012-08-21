namespace StreamBot.IRCBot.Commands
{
    internal interface ICommand
    {
        string Parse(string sender, Permission permission, CommandArgs args);
    }
}