using ConsoleInfo;
using DocumentReader.Components;
using System.Data;
using System.Text.RegularExpressions;

namespace DocumentReader.Core
{
    /// <summary>
    /// Main program class providing console-based interface for document processing operations.
    /// Handles user input, file path validation, and coordinates document processing workflow.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Current version identifier for the application.
        /// Used for version tracking and display in the console interface.
        /// </summary>
        public static readonly string VERSION = "V02";

        /// <summary>
        /// Singleton instance of the text document processor for handling file analysis.
        /// Reused across multiple processing operations during the application lifecycle.
        /// </summary>
        private static TextDocumentProcessor processor = new();

        /// <summary>
        /// Logging utility instance for consistent error and information logging throughout the application.
        /// </summary>
        private static LogUtility log = LogUtility.Current;

        /// <summary>
        /// Application entry point that initializes the console interface and begins user interaction.
        /// Displays version information and prompts user for file path input.
        /// </summary>
        public static void Main()
        {
            // Display application version for user reference
            Console.WriteLine($"Version: {VERSION}\n");

            // Prompt user for file path input
            Console.WriteLine("Enter your books file path");
            var userEntry = Console.ReadLine();

            // Clean user input by removing quote characters that may interfere with path processing
            userEntry = Regex.Replace(userEntry, "\"", "");

            // Begin validation and processing workflow
            ValidateInput(userEntry);
        }

        /// <summary>
        /// Validates user-provided file path and extracts directory and filename components.
        /// Performs comprehensive validation with user-friendly error handling and retry functionality.
        /// </summary>
        /// <param name="userEntry">
        /// • Raw file path input from user (may include quotes or formatting issues)
        /// • Should be a complete file path including directory and filename
        /// • Example: "C:\Documents\textfile.txt"
        /// </param>
        private static void ValidateInput(string userEntry)
        {
            // Verify the complete file path exists in the filesystem
            if (Path.Exists(userEntry) == false)
            {
                log.LogMessage(LogUtility.MessageType.Error, "Invalid file path detected.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();

                // Clear console and restart application for retry
                Console.Clear();
                Main();
            }

            Console.WriteLine($"valid path detected: {userEntry}");

            // Extract directory path component for processing
            var filePath = Path.GetDirectoryName(userEntry);
            if (string.IsNullOrEmpty(filePath))
            {
                log.LogMessage(LogUtility.MessageType.Error, "Invalid file path detected.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();

                // Clear console and restart application for retry
                Console.Clear();
                Main();
            }

            // Extract filename component for processing
            var fileName = Path.GetFileName(userEntry);
            if (string.IsNullOrEmpty(fileName))
            {
                log.LogMessage(LogUtility.MessageType.Error, "Invalid file name detected.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();

                // Clear console and restart application for retry
                Console.Clear();
                Main();
            }

            // All validation passed, proceed to document processing
            ProcessData(filePath, fileName);
        }

        /// <summary>
        /// Initiates asynchronous document processing with validated file path components.
        /// Serves as a bridge between the validation layer and the document processor.
        /// </summary>
        /// <param name="filePath">
        /// • Validated directory path containing the target file
        /// • Guaranteed to be non-null and valid after validation
        /// • Example: "C:\Documents"
        /// </param>
        /// <param name="fileName">
        /// • Validated filename including extension
        /// • Guaranteed to be non-null and valid after validation
        /// • Example: "textfile.txt"
        /// </param>
        private async static Task ProcessData(string filePath, string fileName)
        {
            // Final safety check to ensure parameters are valid before processing
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Execute document processing asynchronously using the text processor
            await processor.ProcessData(filePath, fileName);
        }
    }
}