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
        private readonly FileStream _file;
        private readonly TextWriter _writer;

        public Log()
        {
            if(!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            _file = new FileStream("logs/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", FileMode.Append);
            _writer = new StreamWriter(_file);
        }
        
        public void AddMessage(string message)
        {
            var msg = DateTime.Now.ToString("[HH:mm] ") + message;

            _writer.WriteLine(msg);
            _writer.Flush();
            Console.WriteLine(msg);
        }

        public void AddErrorMessage(string message)
        {
            var msg = DateTime.Now.ToString("[HH:mm] ") + " **ERROR** " + message;

            _writer.WriteLine(msg);
            _writer.Flush();
            Console.WriteLine(msg);
        }
    }
}

