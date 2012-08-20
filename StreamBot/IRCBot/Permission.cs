using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot
{
    public class Permission
    {
        public string Name;

        public bool Operator;

        // Can't be deleted
        public bool SuperOperator;
    }
}
