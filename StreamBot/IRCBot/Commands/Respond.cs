namespace StreamBot.IRCBot.Commands
{
    internal class Respond : ICommand
    {
        private readonly string _text;

        public Respond(string text)
        {
            _text=text;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            return _text;
        }
    }
}
