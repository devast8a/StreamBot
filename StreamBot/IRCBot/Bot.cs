using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Meebey.SmartIrc4net;
using StreamBot.IRCBot.Commands;
using log4net;

namespace StreamBot.IRCBot
{
    internal class Bot
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Bot));

        private IrcClient _irc;
        private readonly CommandHandler _commandHandler;
        private readonly StreamHandler _streamHandler;

        private readonly Timer _checkTimer;

        private readonly SettingsInstance _settings;

        public Bot(SettingsInstance settings)
        {
            _streamHandler = new StreamHandler(this);
            _commandHandler = new CommandHandler();
            _settings = settings;

            Logger.Info("StreamBot Version " + Assembly.GetCallingAssembly().GetName().Version);

            foreach (var stream in _settings.GetStreams())
            {
                _streamHandler.AddStream(stream.Name, stream.Url);
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

            _commandHandler.Add("!addstream", new SecureCommand(x => x.Permission.IsOperator,
                new AddStream(_streamHandler, _settings)
                ));

            _commandHandler.Add("!version", new Respond(Assembly.GetCallingAssembly().GetName().Version.ToString()));

            _commandHandler.Add("!delstream", new SecureCommand(x => x.Permission.IsOperator,
                new DelStream(_streamHandler, _settings)
                ));

            _commandHandler.Add("!streaming", new Streaming(_streamHandler));

            _commandHandler.Add("!update", new UpdateStream(_streamHandler));

            _commandHandler.Add("!addperm", new SecureCommand(x => x.Permission.IsSuperOperator,
                new AddPerm(_settings)
                ));

            _commandHandler.Add("!delperm", new SecureCommand(x => x.Permission.IsSuperOperator,
                new DelPerm(_settings)
                ));

            // Create a suspended stream-check timer
            _checkTimer = new Timer(StreamTimer, null,
                TimeSpan.FromMilliseconds(-1),
                TimeSpan.FromMilliseconds(-1));

            Logger.Info("Bot loaded, ready to connect.");
        }


        public void Connect()
        {
            while (true)
            {
                _irc = new IrcClient();

                Logger.InfoFormat("Connecting to: {0}:{1}", _settings.Server, _settings.Port);
                try
                {
                    _irc.Encoding = System.Text.Encoding.UTF8;
                    _irc.SendDelay = 500;
                    _irc.ActiveChannelSyncing = true;
                    _irc.AutoReconnect = false;
                    _irc.AutoRejoinOnKick = true;
                    _irc.AutoRetry = true;
                    _irc.AutoRetryDelay = 10000;

                    _irc.OnQueryMessage += OnQueryMessage;
                    _irc.OnError += OnError;
                    _irc.OnChannelMessage += OnChannelMessage;
                    _irc.OnRawMessage += (x, e) => Logger.Info(e.Data.RawMessage);

                    _irc.Connect(_settings.Server, _settings.Port);
                }
                catch (Exception e)
                {
                    // Log this shit
                    Logger.Error(e.ToString());
                }

                Logger.Info("Connected! Joining channels");

                try
                {
                    _irc.Login(_settings.Nickname, "Name");

                    if (!String.IsNullOrWhiteSpace(_settings.Password))
                    {
                        if (_irc.Nickname != _settings.Nickname)
                        {
                            _irc.RfcPrivmsg("NickServ", "GHOST " + _settings.Nickname + " " + _settings.Password);
                            _irc.RfcNick(_settings.Nickname);
                        }

                        _irc.RfcPrivmsg("NickServ", "identify " + _settings.Password);
                    }

                    foreach (var channel in _settings.GetAllChannels())
                    {
                        _irc.RfcJoin(channel);
                    }

                    _checkTimer.Change(TimeSpan.Zero, _settings.Period);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }

                try
                {
                    _irc.Listen();
                    Logger.Error("Lost connection to IRC (Automatically reconnecting in 10 seconds)");
                    Thread.Sleep(10000);

                    if (_irc.IsConnected)
                    {
                        _irc.Disconnect();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
        }

        public void BroadcastMessage(string message)
        {
            try
            {
                foreach (var channel in _settings.GetAllChannels())
                {
                    _irc.RfcPrivmsg(channel, message);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Fatal error while sending message to IRC: " + exception);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.Error(e.ErrorMessage);
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            try
            {
                string msg = _commandHandler.ParseCommand(
                    () => GetMessageSource(e),
                    e.Data.Message);

                if (msg != null)
                {
                    _irc.SendMessage(SendType.Message, e.Data.Nick, msg);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private MessageSource GetMessageSource(IrcEventArgs e)
        {
            // Get the permission for this hostname
            var permission = _settings.GetUserPermission(e.Data.Host);

            if (permission == null && !string.IsNullOrWhiteSpace(e.Data.Channel))
            {
                var user = _irc.GetChannelUser(e.Data.Channel, e.Data.Nick);
                // If there's no permission record, see if he's an op on a primary channel
                if (user.IsOp && _settings.GetPrimaryChannels().Any(
                    a => a.Equals(e.Data.Channel, StringComparison.OrdinalIgnoreCase)))
                {
                    if (user.IsOp &&
                        _settings.GetPrimaryChannels().Any(
                            a => a.Equals(e.Data.Channel, StringComparison.OrdinalIgnoreCase)))
                    {
                        permission = Permission.ChannelOperator;
                    }
                }
            }

            return new MessageSource(e.Data.Host, e.Data.Nick, permission ?? Permission.NormalUser);
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            string msg = null;

            try
            {
                msg = _commandHandler.ParseCommand(
                    () => GetMessageSource(e),
                    e.Data.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (msg != null)
            {
                _irc.RfcPrivmsg(e.Data.Channel, string.Format("{0}: {1}", e.Data.Nick, msg));
            }
        }

        private void StreamTimer(object sender)
        {
            try
            {
                _streamHandler.UpdateStreams();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
            }
        }
    }
}