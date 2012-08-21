using System;
using System.IO;

namespace StreamBot.IRCBot
{
    internal class Log
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

