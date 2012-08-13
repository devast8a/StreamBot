using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StreamBot.IRCBot
{
    public class Settings
    {
        private static readonly Regex SectionRegex = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

        public string       Server;
        public int          Port;
        public string       Name;
        public string       Nickname;
        public string       Password;
        public List<string> Channels = new List<string>();
        public List<string> PrimaryChannels = new List<string>();
        public List<string> SecondaryChannels = new List<string>();
        public List<string> SuperOperators = new List<string>();
        public List<string> Operators = new List<string>();

        public bool IsOperator (string name)
		{
			if (Operators.Any(person => person == name))
			{
			    return true;
			}

            return SuperOperators.Any(person => person == name);
		}

		public bool IsSuperOperator (string name)
		{
		    return SuperOperators.Any(person => person == name);
		}

        public void LoadConfig(string filename)
        {
            string[] file = File.ReadAllLines(filename);
            string section = String.Empty;

            foreach (var line in file)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (SectionRegex.IsMatch(line))
                {
                    section = SectionRegex.Match(line).Groups[1].Value;
                    continue;
                }

                if (section == "ConnectionSettings")
                {
                    string[] pair = line.Split(new char[] {'='}, 2);

                    switch (pair[0])
                    {
                        case "server":
                            Server = pair[1];
                            break;
                        case "port":
                            Port = Convert.ToInt32(pair[1]);
                            break;
                        case "name":
                            Name = pair[1];
                            break;
                        case "nickname":
                            Nickname = pair[1];
                            break;
                        case "password":
                            Password = pair[1];
                            break;
                    }
                }

                if (section == "PrimaryChannels")
                {
                    PrimaryChannels.Add(line);
                }

                if (section == "SecondaryChannels")
                {
                    SecondaryChannels.Add(line);
                }
            }

            Channels.AddRange(PrimaryChannels);
            Channels.AddRange(SecondaryChannels);
        }

        public IEnumerable<Stream> LoadStreams(string filename)
        {
            string[] file = File.ReadAllLines(filename);
            foreach (var line in file)
            {
                var text = line.Split(new char[] {':'}, 2);
                Stream stream = new Stream();
                stream.Name = text[0].Trim();
                stream.URL = text[1].Trim();
                stream.Status = 0;
                stream.Subject = String.Empty;
                yield return stream;
            }
        }

        public void SaveStreams(string filename, IEnumerable<Stream> streams)
        {
            TextWriter writer = new StreamWriter(filename);
            foreach (var stream in streams)
            {
                string msg = stream.Name + " : " + stream.URL;
                writer.WriteLine(msg);
            }
            writer.Close();
        }

		public void LoadOps(string filename)
		{
			string[] file = File.ReadAllLines(filename);
			string section = String.Empty;

			foreach (var line in file)
			{
				if (String.IsNullOrWhiteSpace(line))
					continue;

				if (SectionRegex.IsMatch(line))
				{
					section = SectionRegex.Match(line).Groups[1].Value;
					continue;
				}

				if (section == "SuperOperators")
				{
					SuperOperators.Add(line);
					Operators.Add(line);
				}

				if (section == "Operators")
					Operators.Add(line);
			}
		}

		public void SaveOps()
		{
			TextWriter writer = new StreamWriter("ops.txt");
			writer.WriteLine("[SuperOperators]");
			foreach (var supop in SuperOperators)
				writer.WriteLine(supop);
			writer.WriteLine();
			writer.WriteLine("[Operators]");
			foreach (var op in Operators)
			{
				if (!IsSuperOperator(op))
					writer.WriteLine(op);
			}
			writer.Close();
		}
    }
}

