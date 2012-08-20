using System;
using System.Linq;
using System.Threading;
using Meebey.SmartIrc4net;
using StreamBot.IRCBot.Commands;

namespace StreamBot.IRCBot
{
    public class Bot
    {
        private readonly IrcClient _irc;
        private readonly CommandHandler _commandHandler;
        private readonly StreamHandler _streamHandler;

        private readonly Permission _channelOperatorPermission;
        private readonly Permission _normalUserPermission;

        public SettingsInstance Settings;
        public Log Logger;

        public Bot(SettingsInstance settings)
        {
            Logger = new Log();
            _streamHandler = new StreamHandler(this);
            _irc = new IrcClient();
            _commandHandler = new CommandHandler();
            Settings = settings;

            foreach (var stream in Settings.GetStreams())
            {
                _streamHandler.AddStream(stream.Name, stream.URL);
            }

            // Load up all the commands
            _commandHandler.Add("!streamers", new ListStreams(_streamHandler)
            {
                ErrorMessage = "Sorry, there are no streamers.",
            });

            _commandHandler.Add("!streams", new ListStreams(_streamHandler, x => x.Online)
            {
                ErrorMessage = "Sorry, no one is currently streaming.",
                Format = "Current online streamers: {0}",
            });

            _commandHandler.Add("!stream", new StreamInfo(_streamHandler));

            _commandHandler.Add("!about", new Respond(
                "To see a list of currently live streams, write !streams. " +
                "If you want to start streaming and don't know how, write !guide. " +
                "To get your stream added, message one of the !operators. "));

            _commandHandler.Add("!guide", new Respond(
                "A step by step guide on how to set up your own stream can be found here: " +
                "http://vidyadev.com/wiki/A_guide_to_streaming"));

            _commandHandler.Add("!addstream", new SecureCommand(x => x.Operator,
                new AddStream(_streamHandler, Settings)
                ));

            _commandHandler.Add("!remstream", new SecureCommand(x => x.Operator,
                new RemoveStream(_streamHandler, Settings)
                ));

            // Setup permissions
            _channelOperatorPermission = new Permission() {Operator = true};
            _normalUserPermission = new Permission();
        }


        public void Connect()
        {
            try
            {
                _irc.Encoding = System.Text.Encoding.UTF8;
                _irc.SendDelay = 500;
                _irc.ActiveChannelSyncing = true;
                _irc.AutoReconnect = true;
                _irc.AutoRejoinOnKick = true;
                _irc.AutoRetry = true;
                _irc.AutoRetryDelay = 10000;

                _irc.OnQueryMessage += OnQueryMessage;
                _irc.OnError += OnError;
                _irc.OnChannelMessage += OnChannelMessage;

                _irc.Connect(Settings.Server, Settings.Port);
            }
            catch (Exception e)
            {
                // Log this shit
                Logger.AddErrorMessage(e.ToString());
            }

            try
            {
                _irc.Login(Settings.Nickname, "Name");

                if (!String.IsNullOrWhiteSpace(Settings.Password))
                {
                    _irc.SendMessage(SendType.Message, "NickServ", "identify " + Settings.Password);
                }

                foreach (var channel in Settings.GetPrimaryChannels().Union(Settings.GetSecondaryChannels()))
                {
                    _irc.RfcJoin(channel);
                }

                new Timer(StreamTimer, null, TimeSpan.Zero, Settings.Period);

                _irc.Listen();
                _irc.Disconnect();
            }
            catch (Exception e)
            {
                Logger.AddErrorMessage(e.ToString());
            }
        }

        public void SendMessage(string message)
        {
            foreach (var channel in Settings.GetPrimaryChannels())
            {
                _irc.SendMessage(SendType.Message, channel, message);
            }

            foreach (var channel in Settings.GetSecondaryChannels())
            {
                _irc.SendMessage(SendType.Message, channel, message);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.AddErrorMessage(e.ErrorMessage);
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            Permission permission = _channelOperatorPermission;

            // If there is no permission record associated with this hostname
            // then treat this user as a normal user
            if (Settings.GetPermission(e.Data.Host) == null)
            {
                permission = _normalUserPermission;
            }

            string msg = _commandHandler.ParseCommand(e.Data.Nick, permission, e.Data.Message);

            if (msg != null)
            {
                _irc.SendMessage(SendType.Message, e.Data.Nick, msg);
            }
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            var user = _irc.GetChannelUser(e.Data.Channel, e.Data.Nick);

            Permission permission = _normalUserPermission;
            
            // If there is no permission record associated with this hostname
            // see if he's an op on a primary channel
            if (Settings.GetPermission(e.Data.Host) != null)
            {
                if (user.IsOp &&
                    Settings.GetPrimaryChannels().Any(a => a.Equals(e.Data.Channel, StringComparison.OrdinalIgnoreCase)))
                {
                    permission = _channelOperatorPermission;
                }
            }

            string msg = _commandHandler.ParseCommand(e.Data.Nick, permission, e.Data.Message);

            if (msg != null)
            {
                _irc.SendMessage(SendType.Message, e.Data.Channel, string.Format("{0}: {1}", e.Data.Nick, msg));
            }
        }

        private void StreamTimer(object sender)
        {
            _streamHandler.UpdateStreams();
        }
    }
}