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
using System.IO;
using System.Linq;

namespace StreamBot.IRCBot
{
    public class Log
    {
        public static string filename;
        public static int maxlen;

        public static void StartLogging(string filename, int maxlen)
        {
            Log.filename = filename;
            Log.maxlen = maxlen;
            if (File.Exists(filename))
                File.Delete(filename);
        }

        public static void AddMessage(string message)
        {
            string msg = DateTime.Now.ToString("[HH:mm] ") + message;
            File.AppendAllText(Log.filename, msg + "\n");
            CleanFile();
        }

        public static void AddErrorMessage(string message)
        {
            string msg = DateTime.Now.ToString("[HH:mm]") + " *ERROR* " + message;
            File.AppendAllText(Log.filename, msg + "\n");
            CleanFile();
        }

        private static void CleanFile()
        {
            int len = File.ReadAllLines(filename).Length;
            if (len > maxlen)
                File.WriteAllLines(filename, File.ReadAllLines(filename).Skip(len - maxlen).ToArray());
        }
    }
}

