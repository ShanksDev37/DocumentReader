namespace ConsoleInfo
{
    /// <summary>
    /// Provides logging functionality for the notification system using a singleton pattern.
    /// Supports different message types including log, warning, and error messages.
    /// </summary>
    public class LogUtility
    {
        /// <summary>
        /// Gets the singleton instance of the Logger class.
        /// Creates a new instance if one doesn't exist, otherwise returns the existing instance.
        /// </summary>
        /// <value>The current singleton Logger instance.</value>
        public static LogUtility Current => current.Value;

        /// <summary>
        /// Thread-safe lazy initialization of the singleton Logger instance.
        /// </summary>
        private static readonly Lazy<LogUtility> current = new(() => new LogUtility());

        /// <summary>
        /// Enumeration defining the different types of messages that can be logged.
        /// Used to categorize log entries for better filtering and debugging.
        /// </summary>
        public enum MessageType
        {
            /// <summary>General informational messages for normal operations.</summary>
            Log,
            /// <summary>Warning messages indicating potential issues that don't prevent execution.</summary>
            Warning,
            /// <summary>Error messages indicating failures or critical issues.</summary>
            Error
        }

        /// <summary>
        /// Gets or sets the current message type for the logger instance.
        /// This field can be used to track or modify the logger's current state.
        /// </summary>
        /// <remarks>
        /// Note: This field doesn't affect the actual logging behavior in LogMessage method,
        /// which uses the type parameter instead.
        /// </remarks>
        public MessageType messageType;

        /// <summary>
        /// Logs a message to the console with the specified message type and timestamp.
        /// Outputs messages in the format: "{MessageType}: {Message} at {Current DateTime}".
        /// </summary>
        /// <param name="type">The type of message being logged (log, Warning, or Error).</param>
        /// <param name="message">The message content to be logged. Cannot be null.</param>
        /// <example>
        /// <code>
        /// Logger.Current.LogMessage(Logger.MessageType.Error, "Connection failed");
        /// // Output: "Error: Connection failed at 12/25/2024 10:30:15 AM"
        /// </code>
        /// </example>
        public void LogMessage ( MessageType type, string message )
        {
            try
            {
                // Write timestamped message to console with type prefix
                Console.WriteLine($"\n{type}: {message} at {DateTime.Now}\n");
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }
    }
}