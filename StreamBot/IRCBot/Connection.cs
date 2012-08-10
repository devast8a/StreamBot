using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace StreamBot.IRCBot
{
    public class Connection
    {
        private static TcpClient     irc = null;
        private static NetworkStream ns = null;
        private static StreamReader  sr = null;
        private static StreamWriter  sw = null;

        public static bool running  = false;

        public static void Create()
        {
            try
            {
                irc = new TcpClient(Settings.Server, Settings.Port);
            } catch
            {
                Console.WriteLine("Error occured while connecting.");
            }

            try
            {
                ns = irc.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
            }
            catch
            {
                Console.WriteLine("Error occured while communicating.");
            }
            finally
            {
                sendData("USER", Settings.Nickname + " argo.com " + "argo.com " + ":" + Settings.Name);
                sendData("NICK", Settings.Nickname);
                if (Settings.Password != String.Empty)
                {
                    if (Settings.Server.Contains("freenode"))
                        SendMessage("nickserv", "identify " + Settings.Password);
                }
                foreach (var channel in Settings.Channels)
                {
                    sendData("JOIN", channel);
                }
                start();
            }
        }

        private static void sendData(string command, string args)
        {
            if (args == null)
            {
                sw.WriteLine(command);
                sw.Flush();
                Console.WriteLine(command);
            }
            else
            {
                sw.WriteLine(command + " " + args);
                sw.Flush();
                Console.WriteLine(command + " " + args);
            }
        }

        public static void SendMessage(string who, string msg)
        {
            sendData("PRIVMSG", who + " :" + msg);
        }

        private static void setTopic(string channel, string topic)
        {
            sendData("TOPIC", channel + " :" + topic);
        }

        private static void start()
        {
            string[] ex;
            string data;

            var newStreamTimer = new Timer(streamTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));

            running = true;
            while (running)
            {
                data = sr.ReadLine();
                Console.WriteLine(data);
                char[] sep = new char[] { ' ' };
                ex = data.Split(sep, 4);

                if (ex[0] == "PING")
                {
                    sendData("PONG", null);
                }

                string[] cmd = Commands.ParseCommands(data);
                if (cmd[0] != String.Empty && cmd[1] != String.Empty)
                {
                    SendMessage(cmd[0], cmd[1]);
                }
            }
        }

        private static string prevtopic = "null";
        private static void streamTimer(object sender)
        {
            string[] msg = StreamCheck.UpdateStreams();
            if (msg [0] != String.Empty)
            {
                foreach (var channel in Settings.Channels)
                {
                    SendMessage(channel, msg [0]);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            if (prevtopic == "null")
                Thread.Sleep(TimeSpan.FromSeconds(30));

            if (msg [1] != String.Empty && msg[1] != prevtopic)
            {
                foreach (var channel in Settings.PrimaryChannels)
                {
                    setTopic(channel, msg[1]);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            prevtopic = msg[1];
        }
    }
}

