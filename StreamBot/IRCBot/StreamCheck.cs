using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using StreamBot.IRCBot.Sites;

namespace StreamBot.IRCBot
{
    public class StreamCheck
    {
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

        public static UpdateStreamResult UpdateStreams()
        {
            List<Stream> tempStreams = new List<Stream>();
            Log.AddMessage("Checking streams.");
            Log.AddMessage("Streams to check: " + StreamList.Count + ".");

            foreach (var stream in StreamList)
            {
                string link = stream.URL;
                bool status;

                Log.AddMessage("Checking stream " + link + "...");

                try
                {
                    status = SiteProvider.GetSite(link).GetStatus(link);
                }
                catch (UnsupportedSiteException ex)
                {
                    Log.AddErrorMessage("Unsupported site: " + ex.Message);
                    continue;
                }

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
                        msg = msg.Remove(msg.Length - 2, 2) + "!";
                    else
                        msg = String.Empty;

                    if (topic != "[LIVE] Online streams: ")
                        topic = topic.Remove(topic.Length - 2, 2) + "!";
                    else
                        topic = String.Empty;
                }

                return new UpdateStreamResult(topic, msg);
            }

            return new UpdateStreamResult(String.Empty, "No streams are currently online.");
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
                msg += "come chat with us to " + Settings.PrimaryChannels[0] + " !";
                return msg;
            }
            else
            {
                return "No streams are currently online.";
            }
        }
    }
}