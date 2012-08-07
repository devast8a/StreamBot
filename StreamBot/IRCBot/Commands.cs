using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace StreamBot.IRCBot
{
    public class Commands
    {
        public static string[] ParseCommands(string data)
        {
            string[] cmd = data.Split(new char[] {' '}, 4); 
            string[] rtn = new string[] {"", ""};

            if (cmd[1] == "PRIVMSG")
            {
                string sender = Regex.Match(cmd[0], @"^:(.+)!.*$").Groups[1].Value;
                string msg = Regex.Match(cmd[3], @"^:(.+)$").Groups[1].Value;
                string channel = cmd[2];

				Regex streamCheck = new Regex(@"^!stream\s+(\S+)\s*$");
				if (streamCheck.IsMatch(msg))
				{
					rtn[0] = channel;
					rtn[1] = sender + ": ";
					string subj = streamCheck.Match(msg).Groups[1].Value;
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
						}
						else
						{
							message += "Status: Offline";
						}

						rtn[1] += message;
						return rtn;
					}
					else
					{
						rtn[1] += "No such stream found.";
					}

					return rtn;
				}

                if (msg.Trim() == "!streams")
                {
                    rtn[0] = channel;
                    rtn[1] = sender + ": " + StreamCheck.GetOnlineStreams();
                    return rtn;
                }

                if (msg.Trim() == "!about")
                {
                    rtn[0] = channel;
                    rtn[1] = sender + ": ";
                    rtn[1] += "To see a list of currently live streams, write !streams. " +
                        "If you want to start streaming and don't know how, write !guide. " +
                        "To get your stream added, message one of the !operators. " +
                        "If you are a streamer and want to change the subject, use '!streaming new subj'.";
                    return rtn;
                }

                if (msg.Trim() == "!guide")
                {
                    rtn[0] = channel;
                    rtn[1] = sender + ": ";
                    rtn[1] += "A step by step guide on how to set up your own stream can be found here: " +
                        "http://vidyadev.com/wiki/A_guide_to_streaming";
                }

                if (msg.Trim() == "!operators")
                {
                    rtn[0] = channel;
                    rtn[1] = sender + ": ";
                    rtn[1] += "The operators of this bot are: ";
                    foreach (var op in Settings.Operators)
                        rtn[1] += op + ", ";
                    rtn[1] = rtn[1].Remove(rtn[1].Length - 2, 2) + ".";
                }
                  
                if (msg.Trim() == "!streamers" && StreamCheck.StreamList.Any())
                {
                    rtn[0] = channel;
                    rtn[1] = sender + ": Our current streamers are ";
                    foreach (var stream in StreamCheck.StreamList)
                    {
                        rtn[1] += stream.Name + ", ";
                    }
                    rtn[1] = rtn[1].Remove(rtn[1].Length-2, 2) + ".";
                    return rtn;
                }
                
                Regex addStream = new Regex(@"^!addstream\s+(.+)\s+(.+)\s*$");
                if (addStream.IsMatch(msg) && Settings.IsOperator(sender))
                {
                    string name = addStream.Match(msg).Groups[1].Value;
                    string url = addStream.Match(msg).Groups[2].Value;

                    rtn[0] = channel;  
                    rtn[1] = sender + ": ";
                    foreach (var stream in StreamCheck.StreamList)
                    {
                        if (stream.Name.ToLower() == name.ToLower())
                        {
                            rtn[1] += "A stream with that name already exists.";
                            return rtn;
                        }
                    }

                    Stream newStream = new Stream() { Name = name, URL = url, Subject = String.Empty, Status = 0 };
                    StreamCheck.StreamList.Add(newStream);

                    Settings.SaveStreams();

                    rtn[1] += "Stream added successfully.";
                    return rtn;
                }

                Regex delStream = new Regex(@"^!delstream\s+(.+)\s*$");
                if (delStream.IsMatch(msg) && Settings.IsOperator(sender))
                {
                    string name = delStream.Match(msg).Groups[1].Value;

                    rtn[0] = channel;
                    rtn[1] = sender + ": ";
                    for (int i = StreamCheck.StreamList.Count - 1; i >= 0; --i)
                    {
                        if (StreamCheck.StreamList[i].Name.ToLower() == name.ToLower())
                        {
                            StreamCheck.StreamList.RemoveAt(i);

                            Settings.SaveStreams();

                            rtn[1] += "Stream deleted successfully.";
                            return rtn;
                        }
                    }

                    rtn[1] += "No such stream found.";
                    return rtn;
                }

                Regex streamer = new Regex(@"^!streaming\s+(.+)\s*$");
                if (streamer.IsMatch(msg))
                {
                    foreach (var stream in StreamCheck.OnlineStreams)
                    {
                        if (stream.Name == sender)
                        {
                            string subj = streamer.Match(msg).Groups[1].Value;                                                                    
                            stream.Subject = subj;
                            rtn[0] = channel;
                            rtn[1] = sender + ": " + "Stream subject changed successfully.";

                            foreach (var chan in Settings.Channels.Where(x => x != channel))
                            {
                                 string subjmsg = stream.Name + " is now streaming " + subj +"!";                                    ;
                                 Connection.SendMessage(chan, subjmsg);
                                 Thread.Sleep(TimeSpan.FromSeconds(1));                              
                            }

                            return rtn;
                        }
                    }
                }

				Regex addOp = new Regex(@"^!addop\s(\S+)\s*$");
				if (addOp.IsMatch(msg) && Settings.IsOperator(sender))
				{
					rtn[0] = channel;
					rtn[1] = sender + ": ";
					string subj = addOp.Match(msg).Groups[1].Value;
					if (!Settings.IsOperator(subj))
					{
						Settings.Operators.Add(subj);
						Settings.SaveOps();
						rtn[1] += "Operator " + subj + " added successfully.";
					}
					else
					{
						rtn[1] += "An operator with that name already exists!";
					}

					return rtn;
				}

				Regex delOp = new Regex(@"^!delop\s(\S+)\s*$");
				if (delOp.IsMatch(msg) && Settings.IsOperator(sender))
				{
					rtn[0] = channel;
					rtn[1] = sender + ": ";
					string subj = delOp.Match(msg).Groups[1].Value;
					string op = Settings.Operators
						.Where(oper => oper.ToLower() == subj.ToLower())
							.FirstOrDefault();

					if (op != null)
					{
						if (Settings.IsSuperOperator(op))
						{
							rtn[1] += "You can't delete a super operator!";
						}
						else
						{
							Settings.Operators.Remove(op);
							Settings.SaveOps();
							rtn[1] += "Operator " + subj + " deleted successfully.";
						}
					}
					else
					{
						rtn[1] += "No such operator found.";
					}

					return rtn;
				}
            }

            return rtn;
        }
    }
}

