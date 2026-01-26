namespace July_Team.Helpers
{
    /// <summary>
    /// Simple console logging helper for development and debugging.
    /// Provides structured log output with timestamps and log levels.
    /// In production, this could be replaced with a full logging framework like Serilog.
    /// </summary>
    public static class LoggingHelper
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Logs an informational message.
        /// Use for general application flow information.
        /// </summary>
        public static void LogInfo(string source, string message)
        {
            Log("INFO", source, message, ConsoleColor.Cyan);
        }

        /// <summary>
        /// Logs a warning message.
        /// Use for recoverable issues that might need attention.
        /// </summary>
        public static void LogWarning(string source, string message)
        {
            Log("WARN", source, message, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Logs an error message.
        /// Use for exceptions and critical failures.
        /// </summary>
        public static void LogError(string source, string message, Exception? ex = null)
        {
            var fullMessage = ex != null ? $"{message} | Exception: {ex.Message}" : message;
            Log("ERROR", source, fullMessage, ConsoleColor.Red);
        }

        /// <summary>
        /// Logs a debug message.
        /// Use for detailed debugging information during development.
        /// </summary>
        public static void LogDebug(string source, string message)
        {
            Log("DEBUG", source, message, ConsoleColor.Gray);
        }

        /// <summary>
        /// Logs a success message.
        /// Use for successful operation completions.
        /// </summary>
        public static void LogSuccess(string source, string message)
        {
            Log("SUCCESS", source, message, ConsoleColor.Green);
        }

        private static void Log(string level, string source, string message, ConsoleColor color)
        {
            lock (_lock)
            {
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine($"[{timestamp}] [{level}] [{source}] {message}");
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
