namespace StreamBot.IRCBot.Commands
{
    internal interface ICommand
    {
        string Parse(MessageSource sender, Permission permission, CommandArgs args);
    }
}