using System.Text.RegularExpressions;

namespace HammerProxyCheck
{
    static class Program
    {
        internal static readonly Logger Logger = new Logger(Path.Combine(Directory.GetCurrentDirectory(), "ProxyCheck.log"));
        internal static void Main()
        {
            try
            {
                Logger.ClearLog();
                Logger.Info("Started Proxy check");
                Logger.Info("Initialized Console");
                Base.ConsoleSettings();
                Logger.Info("Checking if Directory exists...");
                Base.DirectoryCheck();
                var logs = Base.GetLogs(Base.Folder);
                var allCharacters = new List<Base.MatchedPattern>();
                var logPaths = new List<Base.MatchedPattern>();
                var platforms = new List<Checks.Platform>();
                var ragePluginHookVersions = new List<Base.MatchedPattern>();
                var productVersions = new List<Base.MatchedPattern>();
                int proxyProbability = 0;
                if (logs != null)
                {
                    if (logs.Count == 0)
                    {
                        Console.WriteLine(
                            $"❌ There are no Logs in the Directory: {Base.Folder}\nJumping to the end...");
                        goto Restarting;
                    }
                    Logger.Info("Checking logs");
                    foreach (var log in logs)
                    {
                        var fileContent = Base.ReadFile(log);
                        if (fileContent != null)
                        {
                            var filename = Regex.Match(log, $@"\\LogsForProxyCheck\\(.+)").Groups[1].Value;
                            Console.WriteLine("⚠️  Checking Log file:\t" + filename);
                            // IS LOG VALID CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            var isLogValid = Checks.IsLogValid(fileContent, log);
                            Console.WriteLine(isLogValid
                                ? $"✅  Log: {filename} has not been modified\n"
                                : $"❌  Log {filename} has been modified\n");

                            // LSPDFR CHARACTERS CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            allCharacters.AddRange(Checks.GetCharacters([log]));

                            // LOG PATH CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            var logPath = Checks.GetLogPath(fileContent, log);
                            logPaths.Add(logPath);

                            // RAGE PLUGIN HOOK VERSION CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            var ragePluginHookVersion = Checks.GetRagePluginHookVersion(fileContent, log);
                            ragePluginHookVersions.Add(ragePluginHookVersion);

                            // PRODUCT VERSION CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            var productVersion = Checks.GetProductVersion(fileContent, log);
                            productVersions.Add(productVersion);

                            // PLATFORM CHECK
                            ////////////////////////////////////////////////////////////////////////////////////////////////
                            var platform = Checks.GetPlatform(fileContent, log);
                            platforms.Add(platform);
                        }
                        else
                        {
                            Console.WriteLine($"❌ Failed to read file: {log}\nfileContent was null");
                        }
                    }

                    // Check if characters have changed
                    if (Checks.HasCharacterChanged(allCharacters)) proxyProbability += 5;

                    // Check if log paths have changed
                    if (Checks.HasLogPathChanged(logPaths)) proxyProbability += 25;

                    // Check if RAGE Plugin Hook versions have changed
                    if (Checks.HasRagePluginHookVersionChanged(ragePluginHookVersions)) proxyProbability += 5;

                    // Check if product versions have changed
                    if (Checks.HasProductVersionChanged(productVersions)) proxyProbability += 5;

                    // Check if platforms have changed
                    if (Checks.HasPlatformChanged(platforms, logs)) proxyProbability += 20;
                    if (Checks.IsPossiblyPirated(logs)) proxyProbability += 15;
                    // DATEFORMAT CHECK
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    if (Checks.DateFormatCheck(logs)) proxyProbability += 25;
                    Base.ProxyProbability(proxyProbability);
                }
                else
                {
                    Console.WriteLine("❌ GetLogs returned null");
                }
                Logger.Info("END?");
                Base.ClearConsole();
                Restarting:
                Base.Restart();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}