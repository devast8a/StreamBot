using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamBot.IRCBot.Commands
{
    internal class ListPerms : ICommand
    {
        readonly SettingsInstance _instance;

        public string Format = "{0}";
        public string ErrorMessage = "Sorry, no users with permissions matched.";
        private readonly Func<MessageSource, bool> _predicate;

        public ListPerms(SettingsInstance instance) : this(instance, x => true) { }
        public ListPerms(SettingsInstance instance, Func<MessageSource, bool> predicate)
        {
            _instance = instance;
            _predicate=predicate;
        }

        public string Parse(MessageSource sender, CommandArgs args)
        {
            var ret = string.Join(", ", _instance.GetPermissions().Where(_predicate).Select(x => x.Nickname ?? ("@" + x.Hostname)));

            if(string.IsNullOrWhiteSpace(ret))
            {
                return ErrorMessage;
            }

            return String.Format(Format, ret);
        }
    }
}
