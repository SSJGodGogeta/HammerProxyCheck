using System.Text;
using System.Text.RegularExpressions;

namespace HammerProxyCheck
{
    //TODO: Decoration
    static class Program
    {
        #region Base

        private static readonly string
            Folder = Directory.GetCurrentDirectory() + @"\LogsForProxyCheck\";

        private static void DirectoryCheck()
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
            if (Directory.Exists(Folder) && Directory.GetFiles(Folder).Length > 0)
            {
                Console.WriteLine(
                    $"Going to PERMANENTLY delete old RagePluginHook.logs from: {Folder}\nPlease reply with Y if you want me to do that. Otherwise reply with N");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        foreach (var file in Directory.GetFiles(Folder))
                        {
                            if (!Regex.IsMatch(file, "RagePluginHook.*.log")) continue;
                            if (_detailMode)
                            {
                                Console.WriteLine($"Deleting {file}");
                            }

                            File.Delete(file);
                        }

                        Console.WriteLine(
                            $"Files deleted successfully!\nYou now have time to add your new RagePluginHook.logs into {Folder}\nOnce finished, press Enter to continue");
                        while (Console.ReadKey().Key != ConsoleKey.Enter)
                        {
                            Console.WriteLine("\nYou pressed the wrong key. Press 'Enter' key to continue...");
                        }

                        Console.WriteLine("\nYou pressed 'Enter' key. Continuing...");
                        break;
                    case ConsoleKey.N:
                        Console.WriteLine("No files were deleted");
                        break;
                    default:
                        Console.WriteLine("Invalid Input. Please answer with Y/N");
                        DirectoryCheck();
                        break;
                }
            }
        }

        private static bool _detailMode;

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
                Console.WriteLine($"\n✅ Directory {folder} exists");
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
            Console.WriteLine("\nDetail Mode? Answer with Y/N");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    _detailMode = true;
                    break;
                case ConsoleKey.N:
                    _detailMode = false;
                    break;
                default:
                    Console.WriteLine("Invalid Input. Please answer with Y/N");
                    Main();
                    break;
            }

            Console.OutputEncoding = Encoding.UTF8;
        }

        private static void Restart()
        {
            Console.WriteLine("Do you want to check again?");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    Main();
                    break;
                case ConsoleKey.N:
                    break;
                default:
                    Console.WriteLine("Invalid Input. Please answer with Y/N");
                    Restart();
                    break;
            }
        }

        #endregion

        #region Checks

        private static List<MatchedPattern> IsLogValid(string fileContent, string log)
        {
            var matchedPattern = GetDate(fileContent, log, 2);
            var matchedPattern1 = GetLogPath(fileContent, log);
            var matchedPattern2 = GetRagePluginHookVersion(fileContent, log);
            var matchedPattern3 = RegexMatch(log, fileContent, "Cleaning temp folder");
            var matchedPattern4 = RegexMatch(log, fileContent, "Detected Windows (.+)!", 1);
            var matchedPattern5 = RegexMatch(log, fileContent, "Checking game support");
            var matchedPattern6 = RegexMatch(log, fileContent, "Product name: Grand Theft Auto V");
            var matchedPattern7 = GetProductVersion(fileContent, log);
            var matchedPattern8 = GetIsSteamVersion(fileContent, log);

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

            if (_detailMode)
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

        #region DateFormat
        private static MatchedPattern GetDate(string fileContent, string log, int group)
        {
            return RegexMatch(log, fileContent, @"(Started new log on (\d+\W\d+\W\d+))", group);
        }
        static string? GetDateFormat(string logLine, List<(string pattern, string? format)> dateFormats)
        {
            string? closestFormat = null;
            // Iterate through each date format and try to find matches
            foreach ((string pattern, string? format) in dateFormats)
            {
                MatchCollection matches = Regex.Matches(logLine, pattern);
                foreach (Match unused in matches)
                {
                    closestFormat = format;
                }
            }

            return closestFormat;
        }

        private static void  DateFormatCheck(List<string> logs)
        {
            //Adding loglines to a list
            List<(string, string)> logLines = [];
            foreach (var log in logs)
            {
                var fileContent = ReadFile(log);
                logLines.Add((GetDate(fileContent, log, 1).LogMatch.First(), log));
            }

            // Adding possible dateformats
            List<(string pattern, string? format)> dateFormats =
            [
                (@"\d{4}-[0]?[1-9]-\d{1,2}", "yyyy-MM-dd"), // yyyy-MM-dd  2024-May-XX
                (@"\d{4}-[1][0-2]-\d{1,2}", "yyyy-MM-dd"), // yyyy-MM-dd  2024-Oct-Dez-XX

                (@"\d{4}/[0]?[1-9]/\d{1,2}", "yyyy/MM/dd"), // yyyy/MM/dd  2024/May/XX
                (@"\d{4}/[1][0-2]/\d{1,2}", "yyyy/MM/dd"), // yyyy/MM/dd  2024/Oct-Dez/XX

                (@"\d{4}\.[0]?[1-9]\.\d{1,2}", "yyyy.MM.dd"), // yyyy.MM.dd  2024.May.XX
                (@"\d{4}\.[1][0-2]\.\d{1,2}", "yyyy.MM.dd"), // yyyy.MM.dd  2024.Oct-Dez.XX

                (@"[0]?[1-9]-\d{1,2}-\d{4}", "MM-dd-yyyy"), // MM-dd-yyyy May-XX-2024
                (@"[1][0-2]-\d{1,2}-\d{4}", "MM-dd-yyyy"), // MM-dd-yyyy Oct-Dez-XX-2024

                (@"[0]?[1-9]/\d{1,2}/\d{4}", "MM/dd/yyyy"), // MM/dd/yyyy May/XX/2024
                (@"[1][0-2]/\d{1,2}/\d{4}", "MM/dd/yyyy"), // MM/dd/yyyy Oct/Dez/XX/2024

                (@"[0]?[1-9]\.\d{1,2}\.\d{4}", "MM.dd.yyyy"), // MM.dd.yyyy May.XX.2024
                (@"[1][0-2]\.\d{1,2}\.\d{4}", "MM.dd.yyyy"), // MM.dd.yyyy Oct.Dez.XX.2024

                (@"\d{1,2}\.[0]?[1-9]\.\d{4}", "dd.MM.yyyy"), // dd.MM.yyyy XX.May.2024
                (@"\d{1,2}\.[1][0-2]\.\d{4}", "dd.MM.yyyy"), // dd.MM.yyyy XX.Oct-Dez.2024

                // Add more standard date formats as needed

                // Day and month with textual representation
                (@"\d{1,2} (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}",
                    "d MMM yyyy"), // d MMM yyyy (e.g., 1 Jan 2023)

                (@"\d{1,2} (January|February|March|April|May|June|July|August|September|October|November|December) \d{4}",
                    "d MMMM yyyy"), // d MMMM yyyy (e.g., 1 January 2023)
                // Add more day and month with textual representation formats as needed

                (@"\d{2}/\d{2}/\d{4}", "dd/MM/yyyy") // dd/MM/yyyy

            ];

            // Add your additional date format with the same pattern
            List<(string, string)> detectedDateFormats = [];
            foreach (var line in logLines)
            {
                var closestFormat = GetDateFormat(line.Item1, dateFormats);
                var dateMatch = Regex.Match(line.Item1, "Started new log on (.+)");
                if (dateMatch.Success)
                {
                    var parsedDate = dateMatch.Groups[1].Value;
                    if (closestFormat != null)
                    {
                        if(_detailMode) 
                            Console.WriteLine($"Detected date format: {closestFormat}, Parsed date: {parsedDate}\nIn File: {line.Item2}");
                        detectedDateFormats.Add((closestFormat, line.Item2));
                    }
                    else
                    {
                        Console.WriteLine("No date format detected for: " + line);
                    }
                }
            }

            bool dateFormatChange = false;
            for (int i = 0; i < detectedDateFormats.Count - 1; i++)
            {
                if (detectedDateFormats[i].Item1 != detectedDateFormats[i + 1].Item1)
                {
                    dateFormatChange = true;
                    Console.WriteLine($"❗  Dateformat change found!\nFormat: {detectedDateFormats[i].Item1} in file: {detectedDateFormats[i].Item2}\nFormat: {detectedDateFormats[i+1].Item1} in file: {detectedDateFormats[i+1].Item2}\n");
                }
            }
            if(!dateFormatChange) Console.WriteLine(@"✅  No DateFormat changes found!");
        }
        
        #endregion

        #region Character

        private static void GetCharacters(string fileContent, string log)
        {
            var matchedPattern = RegexMatch(log, fileContent, "Adding (.+) as character", 1, true);
            if (_detailMode)
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
                    characterLogs[characterSet] = [pattern.Log];
                }
            }

            bool changesDetected = false;
            if (!_detailMode)
            {
                foreach (var entry in characterLogs)
                {
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

                    Console.WriteLine("In File:");
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

        #endregion

        #region LogPath

        private static void HasLogPathChanged(List<MatchedPattern> logPaths)
        {
            var logPathLogs = new Dictionary<string, List<string>>();
            foreach (var pattern in logPaths)
            {
                if (pattern.IsMatch)
                {
                    var logPath = pattern.LogMatch.First();

                    if (logPathLogs.TryGetValue(logPath, out var logs))
                    {
                        logs.Add(pattern.Log);
                    }
                    else
                    {
                        logPathLogs[logPath] = [pattern.Log];
                    }
                }
            }

            bool changesDetected = false;
            if (!_detailMode)
            {
                foreach (var entry in logPathLogs)
                {
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue; // Ignore log paths that appear in only one log
                    }

                    changesDetected = true;
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No log path changes detected across logs."
                    : "\n❗ Log path changes were detected");
            }
            else
            {
                foreach (var entry in logPathLogs)
                {
                    var logPath = entry.Key;
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue; // Ignore log paths that appear in only one log
                    }

                    changesDetected = true;
                    Console.WriteLine($"\n❗ Found different Log path:\n- {logPath}");

                    Console.WriteLine("In File:");
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"- {log}");
                    }
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No log path changes detected across logs."
                    : "\n❗ Log path changes were detected in the logs listed above.");
            }
        }

        private static MatchedPattern GetLogPath(string fileContent, string log)
        {
            var matchedPattern = RegexMatch(log, fileContent, @"Log path: (.+)", 1);
            return matchedPattern;
        }

        #endregion

        #region GTA and RPH Version

        private static MatchedPattern GetRagePluginHookVersion(string fileContent, string log)
        {
            return RegexMatch(log, fileContent, @"Version: RAGE Plugin Hook v(.+) for Grand Theft Auto V", 1);
        }

        private static MatchedPattern GetProductVersion(string fileContent, string log)
        {
            return RegexMatch(log, fileContent, @"Product version: (.+)", 1);
        }

        private static void HasRagePluginHookVersionChanged(List<MatchedPattern> versions)
        {
            var versionLogs = new Dictionary<string, List<string>>();
            foreach (var pattern in versions)
            {
                if (pattern.IsMatch)
                {
                    var version = pattern.LogMatch.First();
                    if (versionLogs.TryGetValue(version, out var logs))
                    {
                        logs.Add(pattern.Log);
                    }
                    else
                    {
                        versionLogs[version] = [pattern.Log];
                    }
                }
            }

            bool changesDetected = false;
            if (!_detailMode)
            {
                foreach (var entry in versionLogs)
                {
                    var logs = entry.Value;
                    if (logs.Count > 1)
                    {
                        continue;
                    }

                    changesDetected = true;
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No RAGE Plugin Hook version changes detected across logs."
                    : "\n❗ RAGE Plugin Hook version changes were detected");
            }
            else
            {
                foreach (var entry in versionLogs)
                {
                    var version = entry.Key;
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue;
                    }

                    changesDetected = true;
                    Console.WriteLine($"\n❗ Found different RAGE Plugin Hook version:\n- {version}");

                    Console.WriteLine("In File:");
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"- {log}");
                    }
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No RAGE Plugin Hook version changes detected across logs."
                    : "\n❗ RAGE Plugin Hook version changes were detected in the logs listed above.");
            }
        }

        private static void HasProductVersionChanged(List<MatchedPattern> versions)
        {
            var versionLogs = new Dictionary<string, List<string>>();
            foreach (var pattern in versions)
            {
                if (pattern.IsMatch)
                {
                    var version = pattern.LogMatch.First();
                    if (versionLogs.TryGetValue(version, out var logs))
                    {
                        logs.Add(pattern.Log);
                    }
                    else
                    {
                        versionLogs[version] = [pattern.Log];
                    }
                }
            }

            bool changesDetected = false;
            if (!_detailMode)
            {
                foreach (var entry in versionLogs)
                {
                    var logs = entry.Value;
                    if (logs.Count > 1)
                    {
                        continue;
                    }

                    changesDetected = true;
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No product version changes detected across logs."
                    : "\n❗ Product version changes were detected");
            }
            else
            {
                foreach (var entry in versionLogs)
                {
                    var version = entry.Key;
                    var logs = entry.Value;

                    if (logs.Count > 1)
                    {
                        continue;
                    }

                    changesDetected = true;
                    Console.WriteLine($"\n❗ Found different product version:\n- {version}");

                    Console.WriteLine("In File:");
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"- {log}");
                    }
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No product version changes detected across logs."
                    : "\n❗ Product version changes were detected in the logs listed above.");
            }
        }

        #endregion

        #region Platform

        private static MatchedPattern GetIsSteamVersion(string fileContent, string log)
        {
            return RegexMatch(log, fileContent, @"Is steam version: (True|False)", 1);
        }

        private enum Platform
        {
            Steam,
            EpicGames,
            Retail,
            Unknown
        }

        private static Platform GetPlatform(string fileContent, string log)
        {
            MatchedPattern logpath = GetLogPath(fileContent, log);
            MatchedPattern isSteamVersion = GetIsSteamVersion(fileContent, log);
            foreach (var path in logpath.LogMatch)
            {
                if (isSteamVersion.LogMatch.Contains("True") &&
                    (path.Contains(@"\steamapps\common\Grand Theft Auto V\RagePluginHook.log") ||
                     path.Contains(@"/steamapps/common/Grand Theft Auto V/RagePluginHook.log"))) return Platform.Steam;
                if (path.Contains(@"Grand Theft Auto V\RagePluginHook.log") ||
                    path.Contains("Grand Theft Auto V/RagePluginHook.log")) return Platform.Retail;
                if (path.Contains(@"GTAV\RagePluginHook.log") ||
                    path.Contains("GTAV/RagePluginHook.log")) return Platform.EpicGames;
            }

            return Platform.Unknown;
        }

        private static void HasPlatformChanged(List<Platform> platforms, List<string> logs)
        {
            var platformLogs = new Dictionary<Platform, List<string>>();

            for (int i = 0; i < platforms.Count; i++)
            {
                var platform = platforms[i];
                var log = logs[i];

                if (platformLogs.TryGetValue(platform, out var logList))
                {
                    logList.Add(log);
                }
                else
                {
                    platformLogs[platform] = [log];
                }
            }

            bool changesDetected = platformLogs.Count > 1;
            if (!_detailMode)
            {
                Console.WriteLine(!changesDetected
                    ? "\n✅ No platform changes detected across logs."
                    : "\n❗ Platform changes were detected");
            }
            else
            {
                foreach (var entry in platformLogs)
                {
                    var platform = entry.Key;
                    var logsForPlatform = entry.Value;

                    Console.WriteLine($"\nPlatform: {platform}");
                    foreach (var log in logsForPlatform)
                    {
                        Console.WriteLine($"- {log}");
                    }
                }

                Console.WriteLine(!changesDetected
                    ? "\n✅ No platform changes detected across logs."
                    : "\n❗ Platform changes were detected in the logs listed above.");
            }
        }

        #endregion

        #endregion

private static void Main()
{
    ConsoleSettings();
    DirectoryCheck();
    var logs = GetLogs(Folder);
    var allCharacters = new List<MatchedPattern>();
    var logPaths = new List<MatchedPattern>();
    var platforms = new List<Platform>();
    var ragePluginHookVersions = new List<MatchedPattern>();
    var productVersions = new List<MatchedPattern>();

    if (logs != null)
    {
        if (logs.Count == 0)
        {
            Console.WriteLine($"❌ There are no Logs in the Directory: {Folder}\nJumping to the end...");
            goto Restarting;
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
                int isLogValid = 0;
                foreach (var matchedPattern in matchedPatterns)
                {
                    if (matchedPattern is { IsMatch: true, LogMatch.Count: 1 }) isLogValid++;
                }

                GetCharacters(fileContent, log);
                Console.WriteLine(isLogValid == matchedPatterns.Count
                    ? $"✅ Log: {log} has not been modified\n"
                    : $"❌ Log {log} has been modified\n");

                // LSPDFR CHARACTERS CHECK
                ////////////////////////////////////////////////////////////////////////////////////////////////
                allCharacters.AddRange(GetCharacters([log]));

                // LOG PATH CHECK
                ////////////////////////////////////////////////////////////////////////////////////////////////
                var logPath = GetLogPath(fileContent, log);
                logPaths.Add(logPath);

                // RAGE PLUGIN HOOK VERSION CHECK
                ////////////////////////////////////////////////////////////////////////////////////////////////
                var ragePluginHookVersion = GetRagePluginHookVersion(fileContent, log);
                ragePluginHookVersions.Add(ragePluginHookVersion);

                // PRODUCT VERSION CHECK
                ////////////////////////////////////////////////////////////////////////////////////////////////
                var productVersion = GetProductVersion(fileContent, log);
                productVersions.Add(productVersion);

                // PLATFORM CHECK
                ////////////////////////////////////////////////////////////////////////////////////////////////
                var platform = GetPlatform(fileContent, log);
                platforms.Add(platform);
                
            }
            else
            {
                Console.WriteLine($"❌ Failed to read file: {log}\nfileContent was null");
            }
        }

        // Check if characters have changed
        HasCharacterChanged(allCharacters);

        // Check if log paths have changed
        HasLogPathChanged(logPaths);

        // Check if RAGE Plugin Hook versions have changed
        HasRagePluginHookVersionChanged(ragePluginHookVersions);

        // Check if product versions have changed
        HasProductVersionChanged(productVersions);

        // Check if platforms have changed
        HasPlatformChanged(platforms, logs);
        
        // DATEFORMAT CHECK
        ////////////////////////////////////////////////////////////////////////////////////////////////
        DateFormatCheck(logs);
    }
    else
    {
        Console.WriteLine("❌ GetLogs returned null");
    }

    Restarting:
    Restart();
}

    }
}