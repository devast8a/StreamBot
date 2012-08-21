namespace StreamBot.IRCBot
{
    internal class Permission
    {
        public string Name { get; private set; }
        public bool IsOperator { get; private set; }
        // Can't be deleted
        public bool IsSuperOperator { get; private set; }

        public Permission(string name, bool op, bool superop)
        {
            Name = name;
            IsOperator = op;
            IsSuperOperator = superop;
        }
    }
}
