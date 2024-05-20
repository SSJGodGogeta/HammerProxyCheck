namespace HammerProxyCheck
{
    public enum LogLevel
    {
        Info,
        Error
    }
   
    public class Logger(string logFilePath)
    {
        private readonly object _lockObj = new object();

        private void Log(string message, LogLevel level = LogLevel.Info)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";

            // Log to file
            lock (_lockObj) // Ensure thread-safety for file writing
            {
                try
                {
                    File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
        }
   
        public void Info(string message)
        {
            Log(message);
        }
   
        public void Error(string message)
        {
            Log(message, LogLevel.Error);
        }

        public void ClearLog()
        {
            lock (_lockObj) // Ensure thread-safety for file writing
            {
                try
                {
                    File.WriteAllText(logFilePath, string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to clear log file: {ex.Message}");
                }
            }
        }
    } 
}