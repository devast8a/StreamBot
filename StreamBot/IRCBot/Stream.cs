using System;
using StreamBot.IRCBot.StreamPlugins;

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
        public bool Online;
        public IStreamPlugin Plugin;

        public void Update(StreamHandler handler)
        {
            try
            {
                if(Plugin == null)
                {
                    return;
                }

                if (Plugin.GetStatus(this))
                {
                    if (!Online)
                    {
                        handler.NewOnlineStream(this);
                    }
                    Online = true;
                }else
                {
                    if(Online)
                    {
                        handler.NewOfflineStream(this);
                    }
                    Online = false;
                }
            }
            catch(Exception e)
            {
                if (Online)
                {
                    handler.NewOfflineStream(this);
                }
                Online = false;

                handler.Logger.AddErrorMessage(string.Format("While processing stream {0}({1})\n{2}", Name, URL, e));
            }
        }
    }
}

