using Meebey.SmartIrc4net;

namespace StreamBot.IRCBot.Commands
{
    public interface ICommand
    {
        string Parse(string sender, Permission permission, string[] arguments);
    }
}