using System;
using System.Net.Sockets;
using System.Threading;
using Meebey.SmartIrc4net;

namespace StreamBot.IRCBot
{
    public class Connection
    {
        private readonly Settings _settings;
        private readonly Commands _commands;
        private readonly StreamChecker _checker;

        public IrcClient irc = new IrcClient();

        public Connection(Settings settings, StreamChecker checker)
        {
            _commands = new Commands(settings, checker, irc);
            _settings = settings;
            _checker = checker;

            irc.Encoding = System.Text.Encoding.UTF8;
            irc.SendDelay = 500;
            irc.ActiveChannelSyncing = true;
            irc.AutoReconnect = true;
            irc.AutoRejoinOnKick = true;
            irc.AutoRetry = true;
            irc.AutoRetryDelay = 10000;

            irc.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
            irc.OnError += new ErrorEventHandler(OnError);
            irc.OnRawMessage += new IrcEventHandler(OnRawMessage);
            irc.OnChannelMessage += new IrcEventHandler(OnChannelMessage);

            try
            {
                irc.Connect(_settings.Server, _settings.Port);
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
                System.Environment.Exit(1);
            }

            try
            {
                irc.Login(_settings.Nickname, _settings.Name);
                if (_settings.Server.Contains("freenode"))
                    irc.SendMessage(SendType.Message, "NickServ", "identify " + _settings.Password);

                foreach (var channel in _settings.Channels)
                    irc.RfcJoin(channel);

                new Timer(streamTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));

                irc.Listen();
                irc.Disconnect();
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
                System.Environment.Exit(2);
            }
        }

        private void OnRawMessage(object sender, IrcEventArgs e)
        {
            Log.AddMessage(e.Data.RawMessage);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Log.AddErrorMessage(e.ErrorMessage);
            System.Environment.Exit(3);
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            string msg = _commands.ParseCommand(e.Data.Nick, e.Data.Message, true);
            irc.SendMessage(SendType.Message, e.Data.Nick, msg);
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            string msg = _commands.ParseCommand(e.Data.Nick, e.Data.Message, false);
            irc.SendMessage(SendType.Message, e.Data.Channel, msg);
        }

        private static string prevtopic = "null";
        private void streamTimer(object sender)
        {
            UpdateStreamResult res = _checker.UpdateStreams();
            if (!String.IsNullOrWhiteSpace(res.Message))
            {
                foreach (var channel in _settings.Channels)
                {
                    irc.SendMessage(SendType.Message, channel, res.Message);
                }
            }

            if (prevtopic == "null")
                Thread.Sleep(TimeSpan.FromSeconds(30));

            if (!String.IsNullOrWhiteSpace(res.Topic) && res.Topic != prevtopic)
            {
                foreach (var channel in _settings.PrimaryChannels)
                {
                    irc.RfcTopic(channel, res.Topic);
                }
            }

            prevtopic = res.Topic;
        }
    }
}

