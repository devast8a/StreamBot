using System;
using StreamBot.IRCBot.StreamPlugins;

namespace StreamBot.IRCBot
{
    public class Stream
    {
        public string Name { get; private set; }
        public string URL { get; private set; }
        public string Subject { get; set; }
        public bool Online { get; set; }
        public IStreamPlugin Plugin { get; set; }

        public Stream(string name, string url)
        {
            Name = name;
            URL = url;
        }

        public void Update(StreamHandler handler)
        {
            try
            {
                if (Plugin == null)
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
                }
                else
                {
                    if (Online)
                    {
                        handler.NewOfflineStream(this);
                    }
                    Online = false;
                }
            }
            catch (Exception e)
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