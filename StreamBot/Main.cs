using System;
using System.Threading;
using StreamBot.IRCBot;
using StreamBot.IRCBot.Commands;
using log4net;
using log4net.Config;

namespace StreamBot
{
    internal class Program
    {
        private static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(30);
        private static Timer _settingsTimer;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UpdateStream));

        private static void SaveSettings(object state)
        {
            var settings = (SettingsFile)state;
            
            if(settings == null)
            {
                Logger.Fatal("Settings could not be saved. settings is null.");
                return;
            }

            settings.Save();
        }

        public static void Main(string[] args)
        {
            // Configure log4net from app.config
            XmlConfigurator.Configure();


            var settings = new SettingsFile("Settings.xml");
            _settingsTimer = new Timer(SaveSettings, settings, SaveInterval, SaveInterval);

            Console.WriteLine("Press Shift+Q to quit.");

            foreach (var instance in settings.Instances)
            {
                var bot = new Bot(instance);
                ThreadPool.QueueUserWorkItem(a => bot.Connect());
            }

            while(true)
            {
                var key = Console.ReadKey(true);

                if(key.Modifiers.HasFlag(ConsoleModifiers.Shift) && key.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            settings.Save();
        }
    }
}
