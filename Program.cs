﻿using System.Text;
using System.Text.RegularExpressions;

namespace HammerProxyCheck
{
    static class Program
    {
        #region Base

        private static readonly string
            Folder = Path.Combine("C:", "Users", "arman", "Downloads", "LogsToCheckForProxy");

        private static bool _debug;

        private static string? ReadFile(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            return null;
        }

        /// <summary>
        /// Gets All Logs from a Directory
        /// </summary>
        /// <param name="folder">Path of the Directory to get all Logs from</param>
        /// <returns>A list of all filenames+extensions in the Directory</returns>
        private static List<string>? GetLogs(string folder)
        {
            if (Directory.Exists(folder))
            {
                Console.WriteLine($"✅ Directory {folder} exists");
                List<string> files = new List<string>();
                foreach (var file in Directory.GetFiles(folder))
                {
                    if (file.Contains(".log"))
                    {
                        if (!files.Contains(file))
                        {
                            files.Add(file);
                            Console.WriteLine($"✅ Added File: {file} to list");
                        }
                    }
                    else
                        Console.WriteLine(
                            $"❌ {file} does not have a .log extension and therefore wont be checked by the bot and this application");
                }

                Console.WriteLine();
                return files;
            }

            Console.WriteLine($"❌ Directory {folder} does not exist");
            return null;
        }

        private class MatchedPattern
        {
            public bool IsMatch { get; set; }
            public List<string> LogMatch { get; set; } = new List<string>();
            public string? Log { get; init; }
        }

        /// <summary>
        /// Matches a Regex to a string
        /// </summary>
        /// <param name="log">Log path + name</param>
        /// <param name="logContent">The String to match a regex</param>
        /// <param name="regex">The Regex</param>
        /// <param name="i">Group Value to be returned</param>
        /// <param name="removeDupes">If true, removes duplicate entries in the MatchedPattern.LogMatch List</param>
        /// <returns>returns if the Regex matches and the group value at index i</returns>
        private static MatchedPattern RegexMatch(string log, string logContent, string regex, int i = 0,
            bool removeDupes = false)
        {
            var mp = new MatchedPattern
            {
                IsMatch = false,
                Log = log
            };
            foreach (Match match in Regex.Matches(logContent, regex))
            {
                if (!removeDupes)
                {
                    mp.IsMatch = true;
                    mp.LogMatch.Add(match.Groups[i].Value);
                }
                else
                {
                    mp.IsMatch = true;
                    if (!mp.LogMatch.Contains(match.Groups[i].Value))
                    {
                        mp.LogMatch.Add(match.Groups[i].Value);
                    }
                }
            }

            return mp;
        }

        private static void ConsoleSettings()
        {
            Console.WriteLine("DEBUG Mode? Answer with Y/N");
            switch (Console.ReadLine())
            {
                case "Y" or "y":
                    _debug = true;
                    break;
                case "N" or "n":
                    _debug = false;
                    break;
                default:
                    Console.WriteLine("Invalid Input. Please answer with Y/N");
                    Main();
                    break;
            }

            Console.OutputEncoding = Encoding.UTF8;
        }

        #endregion

        private static List<MatchedPattern> IsLogValid(string fileContent, string log)
        {
            var matchedPattern = RegexMatch(log, fileContent, "Started new log on (\\d+\\W\\d+\\W\\d+)", 1);
            var matchedPattern1 = RegexMatch(log, fileContent, "Log path: (.+)");
            var matchedPattern2 = RegexMatch(log, fileContent, "Version: RAGE Plugin Hook v(.+) for Grand Theft Auto V",
                1);
            var matchedPattern3 = RegexMatch(log, fileContent, "Cleaning temp folder");
            var matchedPattern4 = RegexMatch(log, fileContent, "Detected Windows (.+)!", 1);
            var matchedPattern5 = RegexMatch(log, fileContent, "Checking game support");
            var matchedPattern6 = RegexMatch(log, fileContent, "Product name: Grand Theft Auto V");
            var matchedPattern7 = RegexMatch(log, fileContent, "Product version: (.+)", 1);
            var matchedPattern8 = RegexMatch(log, fileContent, "Is steam version: (.+)", 1);

            List<MatchedPattern> listOfMatchedPatterns =
            [
                matchedPattern,
                matchedPattern1,
                matchedPattern2,
                matchedPattern3,
                matchedPattern4,
                matchedPattern5,
                matchedPattern6,
                matchedPattern7,
                matchedPattern8
            ];

            if (_debug)
            {
                if (matchedPattern is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern.LogMatch)
                    {
                        Console.WriteLine($"✅ Timestamp exists?:	{matchedPattern.IsMatch}\nMatched Timestamp:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern.IsMatch == false
                        ? "Reason: Line \"Started new log on ...\" has been removed!"
                        : "Reason: Line \"Started new log on ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern1 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern1.LogMatch)
                    {
                        Console.WriteLine($"✅ Log path exists?:	{matchedPattern1.IsMatch}\nMatched Log path:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern1.IsMatch == false
                        ? "Reason: Line \"Log path ...\" has been removed!"
                        : "Reason: Line \"Log path ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern2 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern2.LogMatch)
                    {
                        Console.WriteLine(
                            $"✅ RPH Version exists?:	{matchedPattern2.IsMatch}\nMatched RPH Version:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern2.IsMatch == false
                        ? "Reason: Line \"Version: RAGE Plugin Hook ...\" has been removed!"
                        : "Reason: Line \"Version: RAGE Plugin Hook ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern3 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern3.LogMatch)
                    {
                        Console.WriteLine($"✅ Cleaning temp folder exists?:	{matchedPattern3.IsMatch}\nMatched:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern3.IsMatch == false
                        ? "Reason: Line \"Cleaning temp folder ...\" has been removed!"
                        : "Reason: Line \"Cleaning temp folder ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern4 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern4.LogMatch)
                    {
                        Console.WriteLine(
                            $"✅ Detected Windows exists?:	{matchedPattern4.IsMatch}\nMatched Detected Windows:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern4.IsMatch == false
                        ? "Reason: Line \"Detected Windows ...\" has been removed!"
                        : "Reason: Line \"Detected Windows ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern5 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern5.LogMatch)
                    {
                        Console.WriteLine($"✅ Checking game support exists?:	{matchedPattern5.IsMatch}\nMatched:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern5.IsMatch == false
                        ? "Reason: Line \"Checking game support ...\" has been removed!"
                        : "Reason: Line \"Checking game support ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern6 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern6.LogMatch)
                    {
                        Console.WriteLine(
                            $"✅ Product name: Grand Theft Auto V exists?:	{matchedPattern6.IsMatch}\nMatched:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern6.IsMatch == false
                        ? "Reason: Line \"Product name: Grand Theft Auto V ...\" has been removed!"
                        : "Reason: Line \"Product name: Grand Theft Auto V ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern7 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern7.LogMatch)
                    {
                        Console.WriteLine($"✅ Product version: exists?:	{matchedPattern7.IsMatch}\nMatched:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern7.IsMatch == false
                        ? "Reason: Line \"Product version: ...\" has been removed!"
                        : "Reason: Line \"Product version: ...\" appeared more than once!");
                    return listOfMatchedPatterns;
                }

                if (matchedPattern8 is { IsMatch: true, LogMatch.Count: 1 })
                {
                    foreach (var s in matchedPattern8.LogMatch)
                    {
                        Console.WriteLine($"✅ Is steam version exists?:	{matchedPattern8.IsMatch}\nMatched:\t{s}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Log {log} has been modified!");
                    Console.WriteLine(matchedPattern8.IsMatch == false
                        ? "Reason: Line \"Is steam version: ...\" has been removed!"
                        : "Reason: Line \"Is steam version: ...\" appeared more than once!");
                }
            }
            else
            {
                foreach (var matchedPatterns in listOfMatchedPatterns)
                {
                    if (!matchedPatterns.IsMatch || matchedPatterns.LogMatch.Count > 1)
                        return listOfMatchedPatterns;
                }
            }

            return listOfMatchedPatterns;
        }

        private static void GetCharacters(string fileContent, string log)
        {
            var matchedPattern = RegexMatch(log, fileContent, "Adding (.+) as character", 1, true);
            if (_debug)
            {
                Console.WriteLine(matchedPattern.LogMatch.Count > 0
                    ? $"✅ Found the following characters in log:"
                    : $"❌ No Characters found in log:");
                foreach (var character in matchedPattern.LogMatch)
                {
                    if (character.Equals("Ben J.") || character.Equals("Michelle Meto")) continue;
                    Console.WriteLine(character);
                }
            }

            Console.WriteLine();
        }

        private static List<MatchedPattern> GetCharacters(List<string> logs)
        {
            var matchedPatterns = new List<MatchedPattern>();

            foreach (var log in logs)
            {
                var content = ReadFile(log);
                if (content != null)
                {
                    var characterMatches = RegexMatch(log, content, "Adding (.+) as character", 1, true);
                    if (characterMatches.IsMatch)
                    {
                        // Filter out default characters
                        characterMatches.LogMatch = characterMatches.LogMatch
                            .Where(character => character != "Ben J." && character != "Michelle Meto")
                            .ToList();
                        matchedPatterns.Add(characterMatches);
                    }
                }
            }

            return matchedPatterns;
        }

        private static void HasCharacterChanged(List<MatchedPattern> matchedPatterns)
        {
            var characterLogs = new Dictionary<HashSet<string>, List<string>>(HashSet<string>.CreateSetComparer());

            foreach (var pattern in matchedPatterns)
            {
                var characterSet = new HashSet<string>(pattern.LogMatch);

                if (characterLogs.TryGetValue(characterSet, out var logs))
                {
                    logs.Add(pattern.Log);
                }
                else
                {
                    characterLogs[characterSet] = new List<string> { pattern.Log };
                }
            }

            bool changesDetected = false;
            
            if (!_debug)
            {
                foreach (var entry in characterLogs)
                {
                    var characterSet = entry.Key;
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue; // Ignore character sets that appear in only one log
                    }

                    changesDetected = true;
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No character changes detected across logs."
                    : "\n❗ Character changes were detected");
            }
            else
            {
                foreach (var entry in characterLogs)
                {
                    var characterSet = entry.Key;
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue; // Ignore character sets that appear in only one log
                    }

                    changesDetected = true;
                    Console.WriteLine("\n❗❗ The following custom characters were found in different logs:");
                    foreach (var character in characterSet)
                    {
                        Console.WriteLine($"- {character}");
                    }

                    Console.WriteLine("Affected logs:");
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"- {log}");
                    }
                }
                Console.WriteLine(!changesDetected
                    ? "\n✅ No character changes detected across logs."
                    : "\n❗ Character changes were detected");
            }
        }


        private static void Main()
            {
                ConsoleSettings();
                var logs = GetLogs(Folder);
                var allCharacters = new List<MatchedPattern>();

                if (logs != null)
                {
                    if (logs.Count == 0)
                    {
                        Console.WriteLine($"❌ There are no Logs in the Directory: {Folder}");
                        return;
                    }

                    foreach (var log in logs)
                    {
                        var fileContent = ReadFile(log);
                        if (fileContent != null)
                        {
                            // IS LOG VALID CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            Console.WriteLine("Checking Log file:\t" + log);
                            var matchedPatterns = IsLogValid(fileContent, log);
                            var isLogValid = 0;
                            foreach (var matchedPattern in matchedPatterns)
                            {
                                if (matchedPattern is { IsMatch: true, LogMatch.Count: 1 }) isLogValid++;
                            }

                            Console.WriteLine(isLogValid == matchedPatterns.Count
                                ? $"✅ Log: {log} has not been modified"
                                : $"❌ Log {log} has been modified");

                            // LSPDFR CHARACTERS CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            GetCharacters(fileContent, log);
                            allCharacters.AddRange(GetCharacters(new List<string> { log }));
                        }
                        else Console.WriteLine($"❌ Failed to read file: {log}\nfileContent was null");
                    }
                    // Check if characters have changed
                    HasCharacterChanged(allCharacters);
                }
                else
                {
                    Console.WriteLine("❌ GetLogs returned null");
                }
            }
        }
    }