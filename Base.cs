using System.Text;
using System.Text.RegularExpressions;

namespace HammerProxyCheck;

public class Base
{
    internal static readonly string
        Folder = Path.Combine(Directory.GetCurrentDirectory(), "LogsForProxyCheck");

    internal static void DirectoryCheck()
    {
        if (!Directory.Exists(Folder))
        {
            Directory.CreateDirectory(Folder);
            Program.Logger.Info("Created non existing Directory");
        }
        if (Directory.Exists(Folder) && Directory.GetFiles(Folder).Length > 0)
        {
            Console.WriteLine(
                $"\nGoing to PERMANENTLY delete old RagePluginHook.logs from: {Folder}\nPlease reply with Y if you want me to do that. Otherwise reply with N");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    foreach (var file in Directory.GetFiles(Folder))
                    {
                        if (!Regex.IsMatch(file, "RagePluginHook.*.log")) continue;
                        if (DetailMode)
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

    internal static bool DetailMode;

    internal static string? ReadFile(string filePath)
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
    internal static List<string>? GetLogs(string folder)
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
                        var filename = Regex.Match(file, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                        Console.WriteLine($"✅ Added File: {filename} to list");
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

    internal class MatchedPattern
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
    internal static MatchedPattern RegexMatch(string log, string logContent, string regex, int i = 0,
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

    internal static void ConsoleSettings()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "[ULSS]Proxy Checker";
        Console.WriteLine("\nDetail Mode? Answer with Y/N");
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.Y:
                DetailMode = true;
                Program.Logger.Info("Enabled Detail mode");
                break;
            case ConsoleKey.N:
                DetailMode = false;
                Program.Logger.Info("Disabled Detail mode");
                break;
            default:
                Console.WriteLine("Invalid Input. Please answer with Y/N");
                Program.Main();
                break;
        }
    }

    internal static void Restart()
    {
        Console.WriteLine("Do you want to check again?");
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.Y:
                Program.Main();
                break;
            case ConsoleKey.N:
                Program.Logger.Info("Ending");
                break;
            default:
                Console.WriteLine("Invalid Input. Please answer with Y/N");
                Restart();
                break;
        }
    }

    internal static void ProxyProbability(int probability)
    {
        switch (probability)
        {
            case > 80:
                Console.WriteLine($@"💀  {probability}% chance of proxy support. Slam/Kill the user  💀");
                break;
            case > 50 and < 80:
                Console.WriteLine(
                    $"🔍 {probability}% chance of proxy support. Enable detailed mode on the analysis and question the user. High chance!! 🔍");
                break;
            case > 25 and < 50:
                Console.WriteLine(
                    $@"🕵️  {probability}% chance of proxy support. Have a detailed look on the analysis but go easy on the user  🕵️");
                break;
            case > 15 and < 25:
                Console.WriteLine(
                    $"{probability}% chance of proxy support. Have a simple look. Could be a false positive but also due to piracy");
                break;
            case < 15:
                Console.WriteLine(
                    $"{probability}% chance of proxy support. Could be a false positive. Can be triggered easily if character or/and gta or/and rph version changed");
                break;
        }
    }
    
}