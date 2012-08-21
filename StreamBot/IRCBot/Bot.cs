using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Meebey.SmartIrc4net;
using StreamBot.IRCBot.Commands;

namespace StreamBot.IRCBot
{
    internal class Bot
    {
        private readonly IrcClient _irc;
        private readonly CommandHandler _commandHandler;
        private readonly StreamHandler _streamHandler;

        private readonly Permission _channelOperatorPermission;
        private readonly Permission _normalUserPermission;

        private readonly Timer _checkTimer;

        private SettingsInstance _settings;
        public Log Logger;

        public Bot(SettingsInstance settings)
        {
            Logger = new Log();
            _streamHandler = new StreamHandler(this);
            _irc = new IrcClient();
            _commandHandler = new CommandHandler();
            _settings = settings;

            Logger.AddMessage("StreamBot Version " + Assembly.GetCallingAssembly().GetName().Version);

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

            _commandHandler.Add("!addstream", new SecureCommand(x => x.IsOperator,
                new AddStream(_streamHandler, _settings)
                ));

            _commandHandler.Add("!version", new Respond(Assembly.GetCallingAssembly().GetName().Version.ToString()));

            _commandHandler.Add("!remstream", new SecureCommand(x => x.IsOperator,
                new RemoveStream(_streamHandler, _settings)
                ));

            _commandHandler.Add("!streaming", new Streaming(_streamHandler));

            _commandHandler.Add("!update", new UpdateStream(_streamHandler));

            // Create a suspended stream-check timer
            _checkTimer = new Timer(StreamTimer, null,
                TimeSpan.FromMilliseconds(-1),
                TimeSpan.FromMilliseconds(-1));

            // Setup permissions
            _channelOperatorPermission = new Permission(String.Empty, true, false);
            _normalUserPermission = new Permission(String.Empty, false, false);

            Logger.AddMessage("Bot loaded, ready to connect.");
        }


        public void Connect()
        {
            Logger.AddMessage(string.Format("Connecting to: {0}:{1}", _settings.Server, _settings.Port));
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

                _irc.Connect(_settings.Server, _settings.Port);
            }
            catch (Exception e)
            {
                // Log this shit
                Logger.AddErrorMessage(e.ToString());
            }

            Logger.AddMessage("Connected! Joining channels");

            try
            {
                _irc.Login(_settings.Nickname, "Name");

                if (!String.IsNullOrWhiteSpace(_settings.Password))
                {
                    _irc.SendMessage(SendType.Message, "NickServ", "identify " + _settings.Password);
                }

                foreach (var channel in _settings.GetAllChannels())
                {
                    _irc.RfcJoin(channel);
                }

                _checkTimer.Change(TimeSpan.Zero, _settings.Period);

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
            try
            {
                foreach (var channel in _settings.GetAllChannels())
                {
                    _irc.RfcPrivmsg(channel, message);
                }
            }
            catch (Exception exception)
            {
                Logger.AddErrorMessage("Fatal error while sending message to IRC: " + exception);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.AddErrorMessage(e.ErrorMessage);
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            try
            {
                Permission permission = _channelOperatorPermission;

                // If there is no permission record associated with this hostname
                // then treat this user as a normal user
                if (_settings.GetPermission(e.Data.Host) == null)
                {
                    permission = _normalUserPermission;
                }

                string msg = _commandHandler.ParseCommand(e.Data.Nick, permission, e.Data.Message);

                if (msg != null)
                {
                    _irc.SendMessage(SendType.Message, e.Data.Nick, msg);
                }
            }
            catch (Exception exception)
            {
                Logger.AddErrorMessage(exception.ToString());
            }
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            var user = _irc.GetChannelUser(e.Data.Channel, e.Data.Nick);

            Permission permission = _normalUserPermission;

            // Get the permission for this hostname
            var hostPermission = _settings.GetPermission(e.Data.Host);

            // If there's a permission record for this user, he's an op
            if (hostPermission != null)
            {
                // TODO: In the future we may need to distinguish between super op/channel op
                // hostPermission.Value has this information but it is not currently used.
                permission = _channelOperatorPermission;
            }
            else
            {
                // If there's no permission record, see if he's an op on a primary channel
                if (user.IsOp && _settings.GetPrimaryChannels().Any(
                    a => a.Equals(e.Data.Channel, StringComparison.OrdinalIgnoreCase)))
                {
                    if (user.IsOp &&
                        _settings.GetPrimaryChannels().Any(
                            a => a.Equals(e.Data.Channel, StringComparison.OrdinalIgnoreCase)))
                    {
                        permission = _channelOperatorPermission;
                    }
                }
            }

            string msg = null;
            try
            {
                msg = _commandHandler.ParseCommand(e.Data.Nick, permission, e.Data.Message);
            }
            catch (Exception ex)
            {
                Logger.AddErrorMessage(ex.Message);
            }

            if (msg != null)
            {
                _irc.SendMessage(SendType.Message, e.Data.Channel, string.Format("{0}: {1}", e.Data.Nick, msg));
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
                Logger.AddErrorMessage(exception.ToString());
            }
        }
    }
}