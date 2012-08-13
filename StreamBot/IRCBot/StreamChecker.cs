using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using StreamBot.IRCBot.Sites;

namespace StreamBot.IRCBot
{
    public class StreamChecker
    {
        public List<Stream> StreamList = new List<Stream>();
        public List<Stream> OnlineStreams = new List<Stream>();

        private readonly Settings _settings;

        public StreamChecker(Settings settings)
        {
            _settings = settings;
        }

        public bool StreamExists(string name)
        {
            return StreamList.Any(stream => stream.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // TODO: Clean this up some more. It's pretty ugly.
        public UpdateStreamResult UpdateStreams()
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
                    if (stream.Status == StreamStatus.NowOnline)
                        stream.Status = StreamStatus.StillOnline;

                    if (stream.Status == StreamStatus.Offline)
                        stream.Status = StreamStatus.NowOnline;

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

            if (OnlineStreams.Count == 1)
            {
                Stream streamOne = OnlineStreams.FirstOrDefault(item => item.Status == StreamStatus.NowOnline);
                if (streamOne == null)
                {
                    // The only online stream didn't have a status code of 1, so do nothing.
                    return new UpdateStreamResult(String.Empty, "No streams are currently online.");
                }
                return new UpdateStreamResult(
                    String.Format("[LIVE] {0} streaming at {1} !", streamOne.Name, streamOne.URL),
                    String.Format("{0} is now streaming live at {1} !", streamOne.Name, streamOne.URL)
                    );
            }

            if (OnlineStreams.Count > 1)
            {
                //"Streams now online: "
                StringBuilder msg = new StringBuilder();
                // "[LIVE] Online streams: "
                StringBuilder topic = new StringBuilder();

                foreach (var stream in OnlineStreams)
                {
                    if (stream.Status == StreamStatus.NowOnline)
                        msg.AppendFormat("{0} ( {1} ), ", stream.Name, stream.URL);

                    if (stream.Status != StreamStatus.Offline)
                        topic.AppendFormat("{0} ( {1} ), ", stream.Name, stream.URL);
                }

                // Remove the trailing two characters only if we added data to the StringBuilders
                if (msg.Length > 0) msg.Remove(msg.Length - 2, 2);
                if (topic.Length > 0) topic.Remove(topic.Length - 2, 2);

                string actualMsg = msg.Length > 0 ? "Streams now online: " + msg : "No streams are currently online.";
                string actualTopic = topic.Length > 0 ? "[LIVE] Online streams: " + msg : String.Empty;

                return new UpdateStreamResult(actualTopic, actualMsg);
            }

            return new UpdateStreamResult(String.Empty, "No streams are currently online.");
        }

        public string GetOnlineStreams()
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
                msg += "come chat with us to " + _settings.PrimaryChannels[0] + " !";
                return msg;
            }
            else
            {
                return "No streams are currently online.";
            }
        }
    }
}