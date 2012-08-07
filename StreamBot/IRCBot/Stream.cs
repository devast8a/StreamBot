using System;

namespace StreamBot.IRCBot
{
    public class Stream
    {
        public string Name;
        public string URL;
        public string Subject;
        public int Status;

        // status
        //   0 = offline
        //   1 = online and was offline last tick
        //   2 = online and was online last tick
    }
}

