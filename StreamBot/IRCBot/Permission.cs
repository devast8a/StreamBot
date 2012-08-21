using System.Collections.Generic;

namespace StreamBot.IRCBot
{
    internal class Permission
    {
        public string Name { get; private set; }
        public bool IsOperator { get; private set; }
        public bool IsSuperOperator { get; private set; }

        public Permission(string name, bool op, bool superop)
        {
            Name = name;
            IsOperator = op;
            IsSuperOperator = superop;
        }

        private static readonly Dictionary<string, Permission> Permissions;

        static void AddPermission(Permission permission)
        {
            Permissions.Add(permission.Name, permission);
        }

        public static Permission GetPermission(string name)
        {
            Permission output; return Permissions.TryGetValue(name, out output) ? output : null;
        }

        static Permission()
        {
            Permissions = new Dictionary<string, Permission>();

            AddPermission(new Permission("SuperOperator", true, true));
            AddPermission(new Permission("Operator", true, false));
            AddPermission(ChannelOperator);
            AddPermission(NormalUser);
        }

        public static Permission ChannelOperator = new Permission("ChannelOperator", true, false);
        public static Permission NormalUser = new Permission("NormalUser", true, false);
    }
}
