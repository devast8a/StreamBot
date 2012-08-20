using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StreamBot.IRCBot
{
    public class Settings
    {
        private static readonly Regex SectionRegex = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

        public string       Server;
        public int          Port;
        public string       Name;
        public string       Nickname;
        public string       Password;
        public int          CheckPeriod = 3 * 60;
        public List<string> Channels;
        public List<string> PrimaryChannels;
        public List<string> SecondaryChannels;
        public Dictionary<string, Permission> Permissions;
        readonly StreamHandler _handler;

        public Settings(StreamHandler handler)
        {
            PrimaryChannels = new List<string>();
            SecondaryChannels = new List<string>();
            Channels = new List<string>();
            Permissions = new Dictionary<string, Permission>();

            _handler = handler;
        }

        public void LoadConfig()
        {
            string[] file = File.ReadAllLines("settings.txt");
            string section = String.Empty;

            foreach (var line in file)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (SectionRegex.IsMatch(line))
                {
                    section = SectionRegex.Match(line).Groups[1].Value;
                    continue;
                }

                if (section == "ConnectionSettings")
                {
                    string[] pair = line.Split(new[] {'='}, 2);

                    switch (pair[0])
                    {
                        case "server":
                            Server = pair[1];
                            break;
                        case "port":
                            Port = Convert.ToInt32(pair[1]);
                            break;
                        case "name":
                            Name = pair[1];
                            break;
                        case "nickname":
                            Nickname = pair[1];
                            break;
                        case "password":
                            Password = pair[1];
                            break;
                    }
                }

                if(section == "General")
                {
                    string[] pair = line.Split(new[] { '=' }, 2);

                    switch(pair[0])
                    {
                        case "period":
                            CheckPeriod = Convert.ToInt32(pair[1]);
                            break;
                    }
                }

                if (section == "PrimaryChannels")
                {
                    PrimaryChannels.Add(line);
                }

                if (section == "SecondaryChannels")
                {
                    SecondaryChannels.Add(line);
                }
            }

            Channels.AddRange(PrimaryChannels);
            Channels.AddRange(SecondaryChannels);
        }

        public void LoadStreams()
        {
            string[] file = File.ReadAllLines("streams.txt");
            foreach (var line in file)
            {
                var text = line.Split(new[]{':'}, 2);
                
                if(!_handler.AddStream(text[0].Trim(), text[1].Trim()))
                {
                    _handler.Logger.AddErrorMessage(string.Format("{0} can not be handled by any stream status plugins", text[1].Trim()));
                }
            }
        }

        public void SaveStreams()
        {
            TextWriter writer = new StreamWriter("streams.txt");
            foreach (var stream in _handler.StreamList)
            {
                writer.WriteLine("{0} : {1}", stream.Name, stream.URL);
            }
            writer.Close();
        }

        public void LoadOps()
        {
            string[] file = File.ReadAllLines("ops.txt");

            foreach (var line in file)
            {
                var parts = line.Split(new[]{':'}, 2);

                switch(parts[0])
                {
                    case "SuperOperator":
                        Permissions.Add(parts[1], new Permission(){Operator = true, SuperOperator = true});
                        break;

                    case "Operator":
                        Permissions.Add(parts[1], new Permission(){Operator = true});
                        break;

                    default:
                        _handler.Logger.AddErrorMessage("Unknown permission: " + parts[0]);
                        break;
                }
            }
        }

        public void SaveOps()
        {
            TextWriter writer = new StreamWriter("ops.txt");
            foreach(var permission in Permissions){
                if (permission.Value.Name != null)
                {
                    writer.WriteLine("{0}:{1}", permission.Value.Name, permission.Key);
                }
            }
            writer.Close();
        }
    }
}

