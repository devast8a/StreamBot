using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace StreamBot.IRCBot
{
    public class Commands
    {
        private static readonly Regex StreamCommand = new Regex(@"^!stream\s+(\S+)\s*$", RegexOptions.Compiled);
        private static readonly Regex StreamAddCommand = new Regex(@"^!addstream\s+(.+)\s+(.+)\s*$", RegexOptions.Compiled);
        private static readonly Regex StreamDelCommand = new Regex(@"^!delstream\s+(.+)\s*$", RegexOptions.Compiled);
        private static readonly Regex StreamingCommand = new Regex(@"^!streaming\s+(.+)\s*$", RegexOptions.Compiled);
        private static readonly Regex AddOpCommand = new Regex(@"^!addop\s(\S+)\s*$", RegexOptions.Compiled);
        private static readonly Regex DelOpCommand = new Regex(@"^!delop\s(\S+)\s*$", RegexOptions.Compiled);

        private readonly Settings _settings;
        private readonly StreamChecker _checker;
        private readonly IrcClient _client;

        public Commands(Settings settings, StreamChecker checker, IrcClient client)
        {
            _settings = settings;
            _checker = checker;
            _client = client;
        }

        public string ParseCommand(string sender, string command, bool isPrivMsg)
        {
            string rtn = String.Empty;

            if (!isPrivMsg)
                rtn = sender + ": ";

            if (StreamCommand.IsMatch(command))
            {
                string subj = StreamCommand.Match(command).Groups[1].Value;
                if (_checker.StreamExists(subj))
                {
                    Stream stream = _checker.StreamList.FirstOrDefault(str => str.Name.Equals(subj, StringComparison.OrdinalIgnoreCase));
    
                    string message = "Stream - " + stream.Name + ", ";
                    message += "URL: " + stream.URL + ", ";
                    if (stream.Status != StreamStatus.Offline)
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
                rtn += _checker.GetOnlineStreams();
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
                rtn += "The operators of this bot are: " + String.Join(", ", _settings.Operators);
                return rtn;
            }

            if (command.Trim() == "!streamers" && _checker.StreamList.Any())
            {
                rtn += "Our current streamers are " + String.Join(", ", _checker.StreamList);
                return rtn;
            }

            if (StreamAddCommand.IsMatch(command) && _settings.IsOperator(sender))
            {
                string name = StreamAddCommand.Match(command).Groups[1].Value;
                string url = StreamAddCommand.Match(command).Groups[2].Value;
                if (_checker.StreamList.Any(stream => stream.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    rtn += "A stream with that name already exists.";
                    return rtn;
                }

                Stream newStream = new Stream { Name = name, URL = url, Subject = String.Empty, Status = 0 };
                _checker.StreamList.Add(newStream);

                _settings.SaveStreams("streams.txt", _checker.StreamList);

                rtn += "Stream added successfully.";
                return rtn;
            }

            if (StreamDelCommand.IsMatch(command) && _settings.IsOperator(sender))
            {
                string name = StreamDelCommand.Match(command).Groups[1].Value;
                for (int i = _checker.StreamList.Count - 1; i >= 0; --i)
                {
                    if (_checker.StreamList[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        _checker.StreamList.RemoveAt(i);

                        _settings.SaveStreams("streams.txt", _checker.StreamList);

                        rtn += "Stream deleted successfully.";
                        return rtn;
                    }
                }

                rtn += "No such stream found.";
                return rtn;
            }

            if (StreamingCommand.IsMatch(command))
            {
                foreach (var stream in _checker.OnlineStreams)
                {
                    if (stream.Name == sender)
                    {
                        string subj = StreamingCommand.Match(command).Groups[1].Value;                                                                    
                        stream.Subject = subj;
                        rtn += "Stream subject changed successfully.";

                        foreach (var chan in _settings.Channels)
                        {
                             string subjmsg = stream.Name + " is now streaming " + subj +"!";                                    ;
                             _client.SendMessage(SendType.Message, chan, subjmsg);                    
                        }
                        
                        return rtn;
                    }
                }
            }

            if (AddOpCommand.IsMatch(command) && _settings.IsOperator(sender))
            {
            	string subj = AddOpCommand.Match(command).Groups[1].Value;
                if (!_settings.IsOperator(subj))
            	{
                    _settings.Operators.Add(subj);
                    _settings.SaveOps();
            		rtn += "Operator " + subj + " added successfully.";
            	}
            	else
            	{
            		rtn += "An operator with that name already exists!";
            	}

            	return rtn;
            }

            if (DelOpCommand.IsMatch(command) && _settings.IsOperator(sender))
            {
                string subj = DelOpCommand.Match(command).Groups[1].Value;
                string op = _settings.Operators.FirstOrDefault(oper => oper.Equals(subj, StringComparison.OrdinalIgnoreCase));

            	if (op != null)
            	{
                    if (_settings.IsSuperOperator(op))
            		{
            			rtn += "You can't delete a super operator!";
            		}
            		else
            		{
                        _settings.Operators.Remove(op);
            			_settings.SaveOps();
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

