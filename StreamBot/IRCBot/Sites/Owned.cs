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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace StreamBot.IRCBot.Sites
{
    public class Owned
    {
        private static readonly Regex LinkId = new Regex(@"^.+/(?<id>\d+)/*$", RegexOptions.Compiled);

        public static bool GetStatus(string link)
        {
            string url;
            Match m = LinkId.Match(link);
            if (m.Success)
            {
                url = String.Format("http://api.own3d.tv/rest/live/status.xml?liveid={0}", m.Groups["id"].Value);
            }
            else
            {
                Log.AddErrorMessage("Invalid Owned link " + link);
                return false;
            }

            try
            {
                XDocument doc = XDocument.Load(url);
                if (doc.Elements("lives").Elements("live_is_live").FirstOrDefault().Value == "1")
                {
                    Log.AddMessage("Stream " + url + " found online!");
                    return true;
                }

                Log.AddMessage("Stream " + url + " found offline.");
            }
            catch (Exception e)
            {
                Log.AddErrorMessage(e.Message);
            }

            return false;
        }
    }
}