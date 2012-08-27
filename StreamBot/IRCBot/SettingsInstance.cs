using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace StreamBot.IRCBot
{
    internal class SettingsInstance
    {
        private readonly XElement _source;
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Nickname { get; private set; }
        public string Password { get; private set; }
        public TimeSpan Period { get; private set; }

        public SettingsInstance(XElement root)
        {
            _source = root;
            Server = (string)_source.Attribute("Server");
            Port = (int?)_source.Attribute("Port") ?? 6667;
            Nickname = (string)_source.Attribute("Nickname");
            Password = (string)_source.Attribute("Password");
            Period = (TimeSpan?)_source.Attribute("Period") ?? TimeSpan.FromSeconds(180);
        }

        public void AddUserPermission(string name, Permission type)
        {
            var permissions = EnsureExists("Permissions");
            var elem = new XElement(type.PermissionName);
            permissions.Add(elem);
            elem.Add(new XAttribute("Hostname", name));
        }

        public bool RemoveUserPermission(string host)
        {
            var permissions = EnsureExists("Permissions");

            var target = permissions.Elements().FirstOrDefault(x => ((string)x.Attribute("Hostname")).Equals(host, StringComparison.OrdinalIgnoreCase));

            if(target != null)
            {
                target.Remove();
                return true;
            }

            return false;
        }

        public Permission GetUserPermission(string host)
        {
            var permissions = EnsureExists("Permissions");

            var target = permissions.Elements().FirstOrDefault(x => ((string)x.Attribute("Hostname")).Equals(host, StringComparison.OrdinalIgnoreCase));

            if(target != null)
            {
                return Permission.GetPermission(target.Name.LocalName);
            }

            return null;
        }

        public IEnumerable<string> GetPrimaryChannels()
        {
            var channels = EnsureExists("Channels");
            return channels.Elements("PrimaryChannel").Select(chan => (string)chan.Attribute("Name"));
        }

        public IEnumerable<string> GetSecondaryChannels()
        {
            var channels = EnsureExists("Channels");
            return channels.Elements("SecondaryChannel").Select(chan => (string)chan.Attribute("Name"));
        }

        public IEnumerable<string> GetAllChannels()
        {
            return GetPrimaryChannels().Union(GetSecondaryChannels());
        }

        public IEnumerable<Stream> GetStreams()
        {
            var streams = EnsureExists("Streams");
            return streams.Elements("Streamer").Select(
                streamer => new Stream(
                                (string)streamer.Attribute("Nickname"),
                                (string)streamer.Attribute("Location")));
        }

        public void AddStream(string nickname, string location)
        {
            var streams = EnsureExists("Streams");
            // If a stream exists for this user let's remove it first
            RemoveStream(nickname);
            streams.Add(
                new XElement("Streamer",
                    new XAttribute("Nickname", nickname),
                    new XAttribute("Location", location)));
        }

        public void RemoveStream(string nickname)
        {
            var streams = EnsureExists("Streams");

            var target =
                streams.Elements("Streamer").FirstOrDefault(a => ((string)a.Attribute("Nickname")).Equals(nickname));
            if (target != null)
            {
                target.Remove();
            }
        }

        private XElement EnsureExists(string section)
        {
            return _source.Element(section) ?? new XElement(section);
        }

        public IEnumerable<MessageSource> GetPermissions()
        {
            var permissions = EnsureExists("Permissions");

            return permissions.Elements().
                Select( x =>
                    new MessageSource(
                        (string)x.Attribute("Hostname"),
                        (string)x.Attribute("Nickname"),
                        Permission.GetPermission(x.Name.LocalName)
                    )
                );
        }

        public bool SetUserPermissionNick(string hostname, string nick)
        {
            var permissions = EnsureExists("Permissions");

            var target =
                permissions.Elements().FirstOrDefault(a => ((string)a.Attribute("Hostname")).Equals(hostname, StringComparison.OrdinalIgnoreCase));

            if (target != null)
            {
                var res = target.Attribute("Nickname");
            
                if(res != null)
                {
                    res.Remove();
                }
                
                target.Add(new XAttribute("Nickname", nick));

                return true;
            }

            return false;
        }
    }
}