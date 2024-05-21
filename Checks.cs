using System.Text.RegularExpressions;

namespace HammerProxyCheck;

public class Checks
{
    internal static bool IsLogValid(string fileContent, string log)
    {
        var matchedPattern = GetDate(fileContent, log, 2);
        var matchedPattern1 = GetLogPath(fileContent, log);
        var matchedPattern2 = GetRagePluginHookVersion(fileContent, log);
        var matchedPattern3 = Base.RegexMatch(log, fileContent, "Cleaning temp folder");
        var matchedPattern4 = Base.RegexMatch(log, fileContent, "Detected Windows (.+)!", 1);
        var matchedPattern5 = Base.RegexMatch(log, fileContent, "Checking game support");
        var matchedPattern6 = Base.RegexMatch(log, fileContent, "Product name: Grand Theft Auto V");
        var matchedPattern7 = GetProductVersion(fileContent, log);
        var matchedPattern8 = GetIsSteamVersion(fileContent, log);

        List<Base.MatchedPattern> listOfMatchedPatterns =
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

        if (Base.DetailMode)
        {
            if (matchedPattern is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var s in matchedPattern.LogMatch)
                {
                    Console.WriteLine($"Timestamp exists?:	\t{matchedPattern.IsMatch}\t\tValue:\t{s}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern.IsMatch == false
                    ? "Reason: Line \"Started new log on ...\" has been removed!"
                    : "Reason: Line \"Started new log on ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern1 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var s in matchedPattern1.LogMatch)
                {
                    Console.WriteLine($"Log path exists?:	\t{matchedPattern1.IsMatch}\t\tPath:\t{s}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern1.IsMatch == false
                    ? "Reason: Line \"Log path ...\" has been removed!"
                    : "Reason: Line \"Log path ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern2 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var s in matchedPattern2.LogMatch)
                {
                    Console.WriteLine(
                        $"RPH Version exists?:	\t{matchedPattern2.IsMatch}\t\tValue:\t{s}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern2.IsMatch == false
                    ? "Reason: Line \"Version: RAGE Plugin Hook ...\" has been removed!"
                    : "Reason: Line \"Version: RAGE Plugin Hook ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern3 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var unused in matchedPattern3.LogMatch)
                {
                    Console.WriteLine($"Cleaning temp folder exists?:	{matchedPattern3.IsMatch}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern3.IsMatch == false
                    ? "Reason: Line \"Cleaning temp folder ...\" has been removed!"
                    : "Reason: Line \"Cleaning temp folder ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern4 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var s in matchedPattern4.LogMatch)
                {
                    Console.WriteLine(
                        $"Detected Windows exists?:	{matchedPattern4.IsMatch}\t\tValue:\t{s}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern4.IsMatch == false
                    ? "Reason: Line \"Detected Windows ...\" has been removed!"
                    : "Reason: Line \"Detected Windows ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern5 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var unused in matchedPattern5.LogMatch)
                {
                    Console.WriteLine($"Checking game support exists?:	{matchedPattern5.IsMatch}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern5.IsMatch == false
                    ? "Reason: Line \"Checking game support ...\" has been removed!"
                    : "Reason: Line \"Checking game support ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern6 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var unused in matchedPattern6.LogMatch)
                {
                    Console.WriteLine(
                        $"Product name: exists?:	\t{matchedPattern6.IsMatch}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern6.IsMatch == false
                    ? "Reason: Line \"Product name: Grand Theft Auto V ...\" has been removed!"
                    : "Reason: Line \"Product name: Grand Theft Auto V ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern7 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var s in matchedPattern7.LogMatch)
                {
                    Console.WriteLine($"Product version: exists?:	{matchedPattern7.IsMatch}\t\tValue:\t{s}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Log {log} has been modified!");
                Console.WriteLine(matchedPattern7.IsMatch == false
                    ? "Reason: Line \"Product version: ...\" has been removed!"
                    : "Reason: Line \"Product version: ...\" appeared more than once!");
                return false;
            }

            if (matchedPattern8 is { IsMatch: true, LogMatch.Count: 1 })
            {
                foreach (var unused in matchedPattern8.LogMatch)
                {
                    Console.WriteLine($"Is steam version exists?:	{matchedPattern8.IsMatch}");
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
                    return false;
            }
        }

        int isLogValid = 0;
        foreach (var unused in listOfMatchedPatterns)
        {
            if (matchedPattern is { IsMatch: true, LogMatch.Count: 1 }) isLogValid++;
        }

        GetCharacters(fileContent, log);
        return (isLogValid == listOfMatchedPatterns.Count);
    }

    #region DateFormat

    private static Base.MatchedPattern GetDate(string fileContent, string log, int group)
    {
        return Base.RegexMatch(log, fileContent, @"(Started new log on (\d+\W\d+\W\d+))", group);
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

    internal static bool DateFormatCheck(List<string> logs)
    {
        //Adding loglines to a list
        List<(string, string)> logLines = [];
        foreach (var log in logs)
        {
            var fileContent = Base.ReadFile(log);
            List<string> dates = GetDate(fileContent, log, 1).LogMatch;
            foreach (var date in dates)
            {
                logLines.Add((date, log));
            }
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
                    var filename = Regex.Match(line.Item2, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    if (Base.DetailMode)
                        Console.WriteLine(
                            $"Detected date format: {closestFormat},\tParsed date: {parsedDate},\tIn File: {filename}");
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
                var filename = Regex.Match(detectedDateFormats[i].Item2, $@"\\LogsForProxyCheck\\(.+)").Groups[1]
                    .Value;
                var filenameNext = Regex.Match(detectedDateFormats[i + 1].Item2, $@"\\LogsForProxyCheck\\(.+)")
                    .Groups[1].Value;
                Console.WriteLine(
                    $"❗  Dateformat change found!\nFormat: {detectedDateFormats[i].Item1} in file: {filename}\nFormat: {detectedDateFormats[i + 1].Item1} in file: {filenameNext}\n");
            }
        }

        if (!dateFormatChange) Console.WriteLine(@"✅  No DateFormat changes found!");
        return dateFormatChange;
    }

    #endregion

    #region Character

    private static void GetCharacters(string fileContent, string log)
    {
        var matchedPattern = Base.RegexMatch(log, fileContent, "Adding (.+) as character", 1, true);
        if (Base.DetailMode)
        {
            Console.WriteLine(matchedPattern.LogMatch.Count > 0
                ? $"✅  Found the following characters in log:"
                : $"❌  No Characters found in log");
            foreach (var character in matchedPattern.LogMatch)
            {
                if (character.Equals("Ben J.") || character.Equals("Michelle Meto")) continue;
                Console.WriteLine(character);
            }
        }
    }

    internal static List<Base.MatchedPattern> GetCharacters(List<string> logs)
    {
        var matchedPatterns = new List<Base.MatchedPattern>();

        foreach (var log in logs)
        {
            var content = Base.ReadFile(log);
            if (content != null)
            {
                var characterMatches = Base.RegexMatch(log, content, "Adding (.+) as character", 1, true);
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

    internal static bool HasCharacterChanged(List<Base.MatchedPattern> matchedPatterns)
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
        if (!Base.DetailMode)
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
                    var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    Console.WriteLine($"- {filename}");
                }
            }

            Console.WriteLine(!changesDetected
                ? "\n✅ No character changes detected across logs."
                : "\n❗ Character changes were detected");
        }

        return changesDetected;
    }

    #endregion

    #region LogPath

    internal static bool HasLogPathChanged(List<Base.MatchedPattern> logPaths)
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
        if (!Base.DetailMode)
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
                ? "✅ No log path changes detected across logs."
                : "❗ Log path changes were detected");
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
                    var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    Console.WriteLine($"- {filename}");
                }
            }

            Console.WriteLine(!changesDetected
                ? "\n✅ No log path changes detected across logs."
                : "\n❗ Log path changes were detected in the logs listed above.");
        }

        return changesDetected;
    }

    internal static Base.MatchedPattern GetLogPath(string fileContent, string log)
    {
        var matchedPattern = Base.RegexMatch(log, fileContent, @"Log path: (.+)", 1);
        return matchedPattern;
    }

    #endregion

    #region GTA and RPH Version

    internal static Base.MatchedPattern GetRagePluginHookVersion(string fileContent, string log)
    {
        return Base.RegexMatch(log, fileContent, @"Version: RAGE Plugin Hook v(.+) for Grand Theft Auto V", 1);
    }

    internal static Base.MatchedPattern GetProductVersion(string fileContent, string log)
    {
        return Base.RegexMatch(log, fileContent, @"Product version: (.+)", 1);
    }

    internal static bool HasRagePluginHookVersionChanged(List<Base.MatchedPattern> versions)
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
        if (!Base.DetailMode)
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
                ? "✅ No RAGE Plugin Hook version changes detected across logs."
                : "❗ RAGE Plugin Hook version changes were detected");
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
                    var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    Console.WriteLine($"- {filename}");
                }
            }

            Console.WriteLine(!changesDetected
                ? "\n✅ No RAGE Plugin Hook version changes detected across logs."
                : "\n❗ RAGE Plugin Hook version changes were detected in the logs listed above.");
        }

        return changesDetected;
    }

    internal static bool HasProductVersionChanged(List<Base.MatchedPattern> versions)
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
        if (!Base.DetailMode)
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
                ? "✅ No product version changes detected across logs."
                : "❗ Product version changes were detected");
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
                    var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    Console.WriteLine($"- {filename}");
                }
            }

            Console.WriteLine(!changesDetected
                ? "\n✅ No product version changes detected across logs."
                : "\n❗ Product version changes were detected in the logs listed above.");
        }

        return changesDetected;
    }

    #endregion

    #region Platform

    private static Base.MatchedPattern GetIsSteamVersion(string fileContent, string log)
    {
        return Base.RegexMatch(log, fileContent, @"Is steam version: (True|False)", 1);
    }

    internal enum Platform
    {
        Steam,
        EpicGames,
        Retail,
        Unknown
    }

    internal static Platform GetPlatform(string fileContent, string log)
    {
        Base.MatchedPattern logpath = GetLogPath(fileContent, log);
        Base.MatchedPattern isSteamVersion = GetIsSteamVersion(fileContent, log);
        foreach (var path in logpath.LogMatch)
        {
            if (isSteamVersion.LogMatch.Contains("True") &&
                (path.Contains(@"\steamapps\common\Grand Theft Auto V\RagePluginHook.log") ||
                 path.Contains(@"/steamapps/common/Grand Theft Auto V/RagePluginHook.log"))) return Platform.Steam;
            if (isSteamVersion.LogMatch.Contains("True") && (path.Contains(@"Grand Theft Auto V\RagePluginHook.log") ||
                                                             path.Contains("Grand Theft Auto V/RagePluginHook.log")))
                return Platform.Retail;
            if (path.Contains(@"GTAV\RagePluginHook.log") ||
                path.Contains("GTAV/RagePluginHook.log")) return Platform.EpicGames;
        }

        return Platform.Unknown;
    }

    internal static bool HasPlatformChanged(List<Platform> platforms, List<string> logs)
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
        if (!Base.DetailMode)
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
                    var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                    Console.WriteLine($"- {filename}");
                }
            }

            Console.WriteLine(!changesDetected
                ? "\n✅ No platform changes detected across logs.\n"
                : "\n❗ Platform changes were detected in the logs listed above.\n");
        }

        return changesDetected;
    }

    internal static bool IsPossiblyPirated(List<string> logs)
    {
        var isPirated = false;
        var isPiratedReturn = false;
        foreach (var log in logs)
        {
            var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
            var fileContent = Base.ReadFile(log);
            if (GetPlatform(fileContent, log) == Platform.Unknown)
            {
                isPirated = true;
                isPiratedReturn = true;
            }

            if (Base.DetailMode)
            {
                Console.WriteLine(isPirated
                    ? $"\n⚠️ ❗  Possible piracy case detected in log:	{filename}  ❗ ⚠️"
                    : $"\n✅  No piracy case detected in log:\t{filename}");
            }

            isPirated = false;
        }

        if (!Base.DetailMode)
        {
            Console.WriteLine(isPiratedReturn
                ? $@"⚠️ ❗  Possible piracy case detected!  ❗ ⚠️"
                : $"✅  No piracy case detected!");
        }

        Console.WriteLine();
        return isPiratedReturn;
    }

    #endregion
    
}