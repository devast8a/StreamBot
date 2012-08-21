namespace StreamBot.IRCBot
{
    internal class Permission
    {
        public string Name { get; private set; }
        public bool Operator { get; private set; }
        // Can't be deleted
        public bool SuperOperator { get; private set; }

        public Permission(string name, bool op, bool superop)
        {
            Name = name;
            Operator = op;
            SuperOperator = superop;
        }
    }
}
