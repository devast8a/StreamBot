using System;
using System.Threading;
using StreamBot.IRCBot;

namespace StreamBot
{
    class MainClass
    {
        private static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(300);
        private static readonly Timer SettingsTimer = new Timer(SaveSettings, SettingsTimer, SaveInterval, SaveInterval);

        private static void SaveSettings(object state)
        {
            var settings = (SettingsFile)state;
            settings.Save();
        }

        public static void Main(string[] args)
        {
            var settings = new SettingsFile("Settings.xml");

            Console.WriteLine("Press <ENTER> to quit.");

            foreach (var instance in settings.Instances)
            {
                var bot = new Bot(instance);
                ThreadPool.QueueUserWorkItem(a => bot.Connect());
            }

            Console.ReadLine();

            settings.Save();
        }
    }
}
