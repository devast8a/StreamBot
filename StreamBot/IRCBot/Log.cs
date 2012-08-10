using System;
using System.IO;
using System.Linq;

namespace StreamBot.IRCBot
{
    public class Log
    {
        public static string filename;

        public static void StartLogging(string filename)
        {
            Log.filename = filename;
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
            int maxlen = 500;
            int len = File.ReadAllLines(filename).Length;
            if (len > maxlen)
                File.WriteAllLines(filename, File.ReadAllLines(filename).Skip(len - maxlen).ToArray());
        }
    }
}

