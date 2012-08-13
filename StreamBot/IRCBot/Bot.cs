using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamBot.IRCBot
{
    public class Bot
    {
        private readonly Settings _settings = new Settings();
        private Connection _connection;
        private StreamChecker _checker;

        public Settings Settings { get { return _settings; } }

        public void Start()
        {
            List<Stream> streams = _settings.LoadStreams("streams.txt").ToList();
            _settings.LoadConfig("settings.txt");
            _settings.LoadOps("ops.txt");
            Log.StartLogging("streambot.log", 1000);

            _checker = new StreamChecker(_settings) {StreamList = streams};
            _connection = new Connection(_settings, _checker);
        }
    }
}

