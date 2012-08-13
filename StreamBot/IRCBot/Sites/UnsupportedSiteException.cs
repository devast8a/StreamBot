using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Sites
{
    internal class UnsupportedSiteException : Exception
    {
        public UnsupportedSiteException()
        {
        }

        public UnsupportedSiteException(string message) : base(message)
        {
        }
    }
}