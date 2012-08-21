namespace StreamBot.IRCBot.Commands
{
    internal interface ICommand
    {
        string Parse(MessageSource source, CommandArgs args);
    }
}