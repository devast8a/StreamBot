using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace StreamBot.IRCBot
{
    public class Commands
    {
        public static string ParseCommand(string sender, string command, bool isPrivMsg)
        {
            string rtn = String.Empty;

            if (!isPrivMsg)
                rtn = sender + ": ";

            Regex streamCheck = new Regex(@"^!stream\s+(\S+)\s*$");
            if (streamCheck.IsMatch(command))
            {
                string subj = streamCheck.Match(command).Groups [1].Value;
                if (StreamCheck.StreamExists(subj))
                {
                    Stream stream = StreamCheck.StreamList
                        .Where(str => str.Name.ToLower() == subj.ToLower())
                            .FirstOrDefault();
    
                    string message = "Stream - " + stream.Name + ", ";
                    message += "URL: " + stream.URL + ", ";
                    if (stream.Status != 0)
                    {
                        message += "Status: Online";
                        if (stream.Subject != String.Empty)
                        {
                            message += ", Streaming: " + stream.Subject;
                        } 
                        else
                        {
                            message += "Status: Offline";
                        }

                        rtn += message;
                    } 
                    else
                    {
                        rtn += "No such stream found.";
                    }

                    return rtn;
                }
            }

            if (command.Trim() == "!streams")
            {
                rtn += StreamCheck.GetOnlineStreams();
                return rtn;
            }

            if (command.Trim() == "!about")
            {
                rtn += "To see a list of currently live streams, write !streams. " +
                       "If you want to start streaming and don't know how, write !guide. " +
                       "To get your stream added, message one of the !operators. " +
                       "If you are a streamer and want to change the subject, use '!streaming new subj'.";

                return rtn;
            }

            if (command.Trim() == "!guide")
            {
                rtn += "A step by step guide on how to set up your own stream can be found here: " +
                          "http://vidyadev.com/wiki/A_guide_to_streaming";

                return rtn;
            }

            if (command.Trim() == "!operators")
            {
                rtn += "The operators of this bot are: ";
                foreach (var op in Settings.Operators)
                    rtn += op + ", ";
                rtn = rtn.Remove(rtn.Length - 2, 2) + ".";

                return rtn;
            }
                  
            if (command.Trim() == "!streamers" && StreamCheck.StreamList.Any())
            {
                rtn += "Our current streamers are ";
                foreach (var stream in StreamCheck.StreamList)
                {
                    rtn += stream.Name + ", ";
                }
                rtn = rtn.Remove(rtn.Length-2, 2) + ".";
                return rtn;
            }

            Regex addStream = new Regex(@"^!addstream\s+(.+)\s+(.+)\s*$");
            if (addStream.IsMatch(command) && Settings.IsOperator(sender))
            {
                string name = addStream.Match(command).Groups[1].Value;
                string url = addStream.Match(command).Groups[2].Value;
                foreach (var stream in StreamCheck.StreamList)
                {
                    if (stream.Name.ToLower() == name.ToLower())
                    {
                        rtn += "A stream with that name already exists.";
                        return rtn;
                    }
                }

                Stream newStream = new Stream() { Name = name, URL = url, Subject = String.Empty, Status = 0 };
                StreamCheck.StreamList.Add(newStream);

                Settings.SaveStreams();

                rtn += "Stream added successfully.";
                return rtn;
            }

            Regex delStream = new Regex(@"^!delstream\s+(.+)\s*$");
            if (delStream.IsMatch(command) && Settings.IsOperator(sender))
            {
                string name = delStream.Match(command).Groups[1].Value;
                for (int i = StreamCheck.StreamList.Count - 1; i >= 0; --i)
                {
                    if (StreamCheck.StreamList[i].Name.ToLower() == name.ToLower())
                    {
                        StreamCheck.StreamList.RemoveAt(i);

                        Settings.SaveStreams();

                        rtn += "Stream deleted successfully.";
                        return rtn;
                    }
                }

                rtn += "No such stream found.";
                return rtn;
            }

            Regex streamer = new Regex(@"^!streaming\s+(.+)\s*$");
            if (streamer.IsMatch(command))
            {
                foreach (var stream in StreamCheck.OnlineStreams)
                {
                    if (stream.Name == sender)
                    {
                        string subj = streamer.Match(command).Groups[1].Value;                                                                    
                        stream.Subject = subj;
                        rtn += "Stream subject changed successfully.";

                        foreach (var chan in Settings.Channels)
                        {
                             string subjmsg = stream.Name + " is now streaming " + subj +"!";                                    ;
                             Connection.irc.SendMessage(Meebey.SmartIrc4net.SendType.Message, chan, subjmsg);                    
                        }

                        return rtn;
                    }
                }
            }

            Regex addOp = new Regex(@"^!addop\s(\S+)\s*$");
            if (addOp.IsMatch(command) && Settings.IsOperator(sender))
            {
            	string subj = addOp.Match(command).Groups[1].Value;
                if (!Settings.IsOperator(subj))
            	{
            		Settings.Operators.Add(subj);
            		Settings.SaveOps();
            		rtn += "Operator " + subj + " added successfully.";
            	}
            	else
            	{
            		rtn += "An operator with that name already exists!";
            	}

            	return rtn;
            }

            Regex delOp = new Regex(@"^!delop\s(\S+)\s*$");
            if (delOp.IsMatch(command) && Settings.IsOperator(sender))
            {
            	string subj = delOp.Match(command).Groups[1].Value;
            	string op = Settings.Operators
            		.Where(oper => oper.ToLower() == subj.ToLower())
            			.FirstOrDefault();

            	if (op != null)
            	{
            		if (Settings.IsSuperOperator(op))
            		{
            			rtn += "You can't delete a super operator!";
            		}
            		else
            		{
            			Settings.Operators.Remove(op);
            			Settings.SaveOps();
            			rtn += "Operator " + subj + " deleted successfully.";
            		}
            	}
            	else
            	{
            		rtn += "No such operator found.";
            	}

            	return rtn;
            }

            return String.Empty;
        }
    }
}

