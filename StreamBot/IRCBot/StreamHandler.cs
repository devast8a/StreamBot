using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using StreamBot.IRCBot.StreamPlugins;

namespace StreamBot.IRCBot
{
    public class StreamHandler
    {
        private readonly Bot _bot;

        private readonly IStreamPlugin[] _streamPlugins;

        public Log Logger;
        public List<Stream> OnlineStreams;
        public readonly List<Stream> StreamList;

        public StreamHandler(Bot bot)
        {
            _bot = bot;
            Logger = bot.Logger;
            StreamList = new List<Stream>();
            OnlineStreams = new List<Stream>();

            _streamPlugins = new IStreamPlugin[]
            {
                new Livestream(),
                new Owned(),
                new Twitch(),
            };
        }

        public bool AddStream(string name, string url)
        {
            var stream = new Stream();

            stream.Name = name;
            stream.URL = url;
            
            stream.Plugin = _streamPlugins.FirstOrDefault(x => x.UseFor(stream));

            StreamList.Add(stream);

            return stream.Plugin != null;
        }

        public bool StreamExists(string name)
        {
            return StreamList.Any(stream => stream.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateStreams()
        {
            Logger.AddMessage(string.Format("Checking {0} streams.", StreamList.Count));

            foreach (var stream in StreamList)
            {
                stream.Update(this);
                Logger.AddMessage(string.Format("Checked {0} - {1}", stream.Name, stream.Online ? "Online" : "Offline"));
            }

            Logger.AddMessage("Stream checking done.");
        }

        public void NewOfflineStream(Stream stream)
        {
            _bot.SendMessage(string.Format("{0} has stopped streaming.", stream.Name));
        }

        public void NewOnlineStream(Stream stream)
        {
            _bot.SendMessage(string.Format("{0} has started streaming over at {1}", stream.Name, stream.URL));
        }

        public Stream GetStream(string streamName)
        {
            return StreamList.FirstOrDefault(x => x.Name.Equals(streamName, StringComparison.OrdinalIgnoreCase));
        }
    }
}

