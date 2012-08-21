using System;
using System.Collections.Generic;
using System.Linq;
using StreamBot.IRCBot.StreamPlugins;
using log4net;

namespace StreamBot.IRCBot
{
    internal class StreamHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StreamHandler));

        private readonly Bot _bot;
        private readonly IStreamPlugin[] _streamPlugins;
        private readonly List<Stream> _streams;

        public StreamHandler(Bot bot)
        {
            _bot = bot;
            _streams = new List<Stream>();

            _streamPlugins = new IStreamPlugin[]
            {
                new Livestream(),
                new Owned(),
                new Twitch()
            };
        }

        public int RemoveAll(Predicate<Stream> predicate)
        {
            return _streams.RemoveAll(predicate);
        }

        public IEnumerable<Stream> Matching(Func<Stream, bool> predicate)
        {
            return _streams.Where(predicate);
        }

        public bool AddStream(string name, string url)
        {
            var stream = new Stream(name, url);

            stream.Plugin = _streamPlugins.FirstOrDefault(x => x.UseFor(stream));

            _streams.Add(stream);

            return stream.Plugin != null;
        }

        public bool StreamExists(string name)
        {
            return _streams.Any(stream => stream.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateStreams()
        {
            Logger.Info(string.Format("Checking {0} streams.", _streams.Count));

            foreach (var stream in _streams)
            {
                stream.Update(this);
                Logger.Info(string.Format("Checked {0} - {1}", stream.Name, stream.Online ? "Online" : "Offline"));
            }

            Logger.Info("Stream checking done.");
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
            return _streams.FirstOrDefault(x => x.Name.Equals(streamName, StringComparison.OrdinalIgnoreCase));
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

