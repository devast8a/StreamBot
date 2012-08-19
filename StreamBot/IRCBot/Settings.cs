/*
 * Copyright (C) 2012 Tomas Drbota
 *
 * This file is part of StreamBot.
 * StreamBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * StreamBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with StreamBot.  If not, see <http://www.gnu.org/licenses/>.
 */

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

        public static string       Server;
        public static int          Port;
        public static string       Name;
        public static string       Nickname;
        public static string       Password;
        public static List<string> Channels;
        public static List<string> PrimaryChannels;
        public static List<string> SecondaryChannels;
        public static List<string> SuperOperators;
        public static List<string> Operators;

        static Settings()
        {
            PrimaryChannels = new List<string>();
            SecondaryChannels = new List<string>();
            Channels = new List<string>();
			SuperOperators = new List<string>();
            Operators = new List<string>();
        }

        public static bool IsOperator (string name)
		{
			if (Operators.Any(person => person == name))
			{
			    return true;
			}

            return SuperOperators.Any(person => person == name);
		}

		public static bool IsSuperOperator (string name)
		{
		    return SuperOperators.Any(person => person == name);
		}

        public static void LoadConfig()
        {
            string[] file = File.ReadAllLines("settings.txt");
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

        public static void LoadStreams()
        {
            string[] file = File.ReadAllLines("streams.txt");
            foreach (var line in file)
            {
                var text = line.Split(new char[] {':'}, 2);
                Stream stream = new Stream();
                stream.Name = text[0].Trim();
                stream.URL = text[1].Trim();
                stream.Status = 0;
                stream.Subject = String.Empty;
                StreamCheck.StreamList.Add(stream);
            }
        }

        public static void SaveStreams()
        {
            TextWriter writer = new StreamWriter("streams.txt");
            foreach (var stream in StreamCheck.StreamList)
            {
                string msg = stream.Name + " : " + stream.URL;
                writer.WriteLine(msg);
            }
            writer.Close();
        }

		public static void LoadOps()
		{
			string[] file = File.ReadAllLines("ops.txt");
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

		public static void SaveOps()
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

