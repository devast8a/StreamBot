﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamBot.IRCBot.Commands
{
    class AddStream : ICommand
    {
        readonly StreamHandler _handler;
        private readonly SettingsInstance _settings;

        public AddStream(StreamHandler handler, SettingsInstance settings)
        {
            _handler=handler;
            _settings = settings;
        }

        public string Parse(string sender, Permission permission, CommandArgs args)
        {
            if(args.Args.Length != 2)
            {
                return String.Format("Error - Usage {0} <stream-name> <stream-url>", args.Name);
            }

            var name = args.Args[0];
            var url = args.Args[1];

            _handler.Logger.AddMessage(string.Format("{0} is adding a new stream. {1} - {2}", sender, name, url));

            if(_handler.StreamList.Any(x => x.Name == name)){
                return "Error - A streamer already exists with that name";
            }

            if( !_handler.AddStream(name, url) )
            {
                // Yeah yeah, it's hacky.
                _handler.StreamList.RemoveAll(x => x.Name == name);

                return "Error - The URL provided can not be handled by any stream plugins.";
            }

            _settings.AddStream(name, url);
            _handler.Logger.AddMessage(string.Format("{0} added a new stream. {1} - {2}", sender, name, url));
            return string.Format("Success, {0} added as a new streamer", name);
        }
    }
}
