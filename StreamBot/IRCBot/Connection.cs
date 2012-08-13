using System;
using System.Net.Sockets;
using System.Threading;
using Meebey.SmartIrc4net;

namespace StreamBot.IRCBot
{
    public class Connection
    {
        public static IrcClient irc = new IrcClient();

        public static void Create()
        {
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
                irc.Connect(Settings.Server, Settings.Port);
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
                System.Environment.Exit(1);
            }

            try
            {
                irc.Login(Settings.Nickname, Settings.Name);
                if (Settings.Server.Contains("freenode"))
                    irc.SendMessage(SendType.Message, "NickServ", "identify " + Settings.Password);

                foreach (var channel in Settings.Channels)
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

        private static void OnRawMessage(object sender, IrcEventArgs e)
        {
            Log.AddMessage(e.Data.RawMessage);
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Log.AddErrorMessage(e.ErrorMessage);
            System.Environment.Exit(3);
        }

        private static void OnQueryMessage(object sender, IrcEventArgs e)
        {
            string msg = Commands.ParseCommand(e.Data.Nick, e.Data.Message, true);
            irc.SendMessage(SendType.Message, e.Data.Nick, msg);
        }

        private static void OnChannelMessage(object sender, IrcEventArgs e)
        {
            string msg = Commands.ParseCommand(e.Data.Nick, e.Data.Message, false);
            irc.SendMessage(SendType.Message, e.Data.Channel, msg);
        }

        private static string prevtopic = "null";
        private static void streamTimer(object sender)
        {
            string[] msg = StreamCheck.UpdateStreams();
            if (msg [0] != String.Empty)
            {
                foreach (var channel in Settings.Channels)
                {
                    irc.SendMessage(SendType.Message, channel, msg [0]);
                }
            }

            if (prevtopic == "null")
                Thread.Sleep(TimeSpan.FromSeconds(30));

            if (msg [1] != String.Empty && msg[1] != prevtopic)
            {
                foreach (var channel in Settings.PrimaryChannels)
                {
                    irc.RfcTopic(channel, msg[1]);
                }
            }

            prevtopic = msg[1];
        }
    }
}

