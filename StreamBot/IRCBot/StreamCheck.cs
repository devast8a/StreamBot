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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace StreamBot.IRCBot
{
    public class StreamCheck
    {
        private static readonly Regex Livestream = new Regex(@"^.*livestream\.com.+$", RegexOptions.Compiled);
        private static readonly Regex Twitch = new Regex(@"^.*twitch\.tv.+$", RegexOptions.Compiled);
        private static readonly Regex Owned = new Regex(@"^.*own3d\.tv.+$", RegexOptions.Compiled);

        public static List<Stream> StreamList;
        public static List<Stream> OnlineStreams;

        static StreamCheck()
        {
            StreamList = new List<Stream>();
            OnlineStreams = new List<Stream>();
        }

		public static bool StreamExists(string name)
		{
		    return StreamList.Any(stream => stream.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

        public static string[] UpdateStreams()
        {
			List<Stream> tempStreams = new List<Stream>();
            Log.AddMessage("Checking streams.");
            Log.AddMessage("Streams to check: " + StreamList.Count + ".");

            foreach (var stream in StreamList)
            {
                string link = stream.URL;
                bool status = false;

                Log.AddMessage("Checking stream " + link + "...");

                if (Livestream.IsMatch(link))
                    status = Sites.Livestream.GetStatus(link);
                else if (Twitch.IsMatch(link))
                    status = Sites.Twitch.GetStatus(link);
                else if (Owned.IsMatch(link))
                    status = Sites.Owned.GetStatus(link);

                if (status)
                {
                    if (stream.Status == 1)
                        stream.Status = 2;

                    if (stream.Status == 0)
                        stream.Status = 1;

                    tempStreams.Add(stream);
                } 
                else
                {
                    stream.Status = 0;
                    stream.Subject = String.Empty;
                }
            }

            Log.AddMessage("Stream checking done.");

			OnlineStreams.Clear();
			OnlineStreams.AddRange(tempStreams);
			tempStreams.Clear();

            if (OnlineStreams.Count > 0)
            {
                string msg = String.Empty;
                string topic = String.Empty;

                if (OnlineStreams.Count == 1 || OnlineStreams.Count(stream => stream.Status == 1) == 1)
                {
                    Stream streamOne = (from item in OnlineStreams
                            where item.Status == 1
                            select item).FirstOrDefault();

                    if (streamOne != null)
                    {
                        msg += streamOne.Name + " is now streaming live at " + streamOne.URL + " !";
                    }

                    if (OnlineStreams[0].Status != 0)
                        topic += "[LIVE] " + OnlineStreams[0].Name + " streaming at " + OnlineStreams[0].URL + " !";
                }
                else
                {
                    msg = "Streams now online: ";
                    topic = "[LIVE] Online streams: ";
                    foreach (var stream in OnlineStreams)
                    {
                        if (stream.Status == 1)
                            msg += stream.Name + " ( " + stream.URL + " ), ";

                        if (stream.Status != 0)
                            topic += stream.Name + " ( " + stream.URL + " ), ";
                    }
                    if (msg != "Streams now online: ")
                        msg = msg.Remove(msg.Length-2, 2) + "!";
                    else
                        msg = String.Empty;

                    if (topic != "[LIVE] Online streams: ")
                        topic = topic.Remove(topic.Length-2, 2) + "!";
                    else
                        topic = String.Empty;
                }

                string[] rtn = new string[] {msg, topic};
                return rtn;
            }

            string[] nothing = new string[] {String.Empty, "No streams are currently online."};
            return nothing;
        }

        public static string GetOnlineStreams()
        {
            if (OnlineStreams.Count > 0)
            {
                string msg = "Currently streaming live: ";
                foreach (var stream in OnlineStreams)
                {
                    msg += stream.Name;
                    if (stream.Subject != String.Empty)
                        msg += " (" + stream.Subject + ")";
                    msg += ", ";  
                }
                msg += "come chat with us to " + Settings.PrimaryChannels [0] + " !";
                return msg;
            } 
            else
            {
                return "No streams are currently online.";
            }
        }
    }
}

