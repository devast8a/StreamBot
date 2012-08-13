using System;

namespace StreamBot.IRCBot
{
    public enum StreamStatus
    {
        // Offline
        Offline,
        // Online now, but was offline last tick
        NowOnline,
        // Online now, and was online last tick
        StillOnline,
    }

    public class Stream
    {
        public string Name;
        public string URL;
        public string Subject;
        public StreamStatus Status;

        // status
        //   0 = offline
        //   1 = online and was offline last tick
        //   2 = online and was online last tick
    }
}

