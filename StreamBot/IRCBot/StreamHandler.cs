using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StreamBot.IRCBot.StreamPlugins;

namespace StreamBot.IRCBot
{
    internal class StreamHandler
    {
        private readonly Bot _bot;

        private readonly IStreamPlugin[] _streamPlugins;

        public Log Logger;
        public readonly List<Stream> StreamList;

        public StreamHandler(Bot bot)
        {
            _bot = bot;
            Logger = bot.Logger;
            StreamList = new List<Stream>();

            _streamPlugins = new IStreamPlugin[]
            {
                new Livestream(),
                new Owned(),
                new Twitch()
            };
        }

        public bool AddStream(string name, string url)
        {
            var stream = new Stream(name, url);

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
            _bot.BroadcastMessage(string.Format("{0} has stopped streaming.", stream.Name));
        }

        public void NewOnlineStream(Stream stream)
        {
            _bot.BroadcastMessage(string.Format("{0} has started streaming over at {1}", stream.Name, stream.Url));
        }

        public Stream GetStream(string streamName)
        {
            return StreamList.FirstOrDefault(x => x.Name.Equals(streamName, StringComparison.OrdinalIgnoreCase));
        }

        public void StreamSubjectChange(Stream stream)
        {
            if(string.IsNullOrWhiteSpace(stream.Subject))
            {
                return;
            }

            _bot.BroadcastMessage(string.Format("{0} - {1} - Is now streaming {2}", stream.Name, stream.Url, stream.Subject));
        }
    }
}

