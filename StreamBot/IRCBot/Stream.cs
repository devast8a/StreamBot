/*
 * Copyright (C) 2012 Tomas Drbota
 *
 * This file is part of StreamBot.
 * StreamBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * StreamBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with StreamBot.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using StreamBot.IRCBot.StreamPlugins;

namespace StreamBot.IRCBot
{
    public enum StreamStatus
    {
        // Offline
        Offline,
        // Online now, but was offline last tick
        NowOnline,
        // Online now, and was online last tick
        StillOnline,
    }

    public class Stream
    {
        public string Name;
        public string URL;
        public string Subject;
        public bool Online;
        public IStreamPlugin Plugin;

        public void Update(StreamHandler handler)
        {
            try
            {
                if(Plugin == null)
                {
                    return;
                }

                if (Plugin.GetStatus(this))
                {
                    if (!Online)
                    {
                        handler.NewOnlineStream(this);
                    }
                    Online = true;
                }else
                {
                    if(Online)
                    {
                        handler.NewOfflineStream(this);
                    }
                    Online = false;
                }
            }
            catch(Exception e)
            {
                if (Online)
                {
                    handler.NewOfflineStream(this);
                }
                Online = false;

                handler.Logger.AddErrorMessage(string.Format("While processing stream {0}({1})\n{2}", Name, URL, e));
            }
        }
    }
}

