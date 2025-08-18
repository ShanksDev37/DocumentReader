using ConsoleInfo;
using System.Text.RegularExpressions;
using DocumentReader.Core;
using System.Reflection;
using System.Diagnostics;

namespace DocumentReader
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
        public static readonly string VERSION = "0.4";
        public static readonly string SIGNATURE = "Created by Shannon King.";
        public static readonly string COPYRIGHT = "Copyright: CC-BY 2025";

        private static readonly string debugKey = "DEBUG";

        /// <summary>
        /// Logging utility instance for consistent error and information logging throughout the application.
        /// </summary>
        private static readonly LogUtility log = LogUtility.Current;

        /// <summary>
        /// Singleton instance of the text document processor for handling file analysis.
        /// Reused across multiple processing operations during the application lifecycle.
        /// </summary>
        private static DocReader docReader = new();

        /// <summary>
        /// Application entry point that initializes the console interface and begins user interaction.
        /// Displays version information and prompts user for file path input.
        /// </summary>
        public static void Main()
        {
            // Display application version for user reference
            Console.WriteLine($"Version: {VERSION}\n");
            Console.WriteLine($"{SIGNATURE}\n");
            Console.WriteLine($"{COPYRIGHT}\n");

            // Prompt user for file path input
            Console.WriteLine("Enter your books file path");

            var userEntry = Console.ReadLine();

            // Begin validation and processing workflow
            ValidateInput(userEntry, false, 0);
        }

        public static void DebugMain()
        {
            Console.Clear();

            // Display application version for user reference
            Console.WriteLine($"Version: {VERSION}\n");
            Console.WriteLine($"{debugKey}\n");

            Console.WriteLine("Enter the file path.");
            var userEntry = Console.ReadLine();
            Console.WriteLine("Generate a word soup document?");
            var wordSoup = Console.ReadKey();
            int soupSize = 0;
            if (wordSoup.Key == ConsoleKey.Y)
            {
                Console.WriteLine("\nHow large? specify in (numbers only), each number represents 1KB (AKA 512 characters).");
                var goodSoup = Console.ReadLine();

                try
                {
                    soupSize = Int32.Parse(goodSoup);

                    if (soupSize > int.MaxValue)
                        throw new();

                    if (soupSize <= 0)
                        throw new();

                    ValidateInput(userEntry, true, soupSize);
                }
                catch
                {
                    Console.WriteLine("It puts the proper size in the console, or it gets the hose again.");
                    DebugMain();
                    return;
                }
            }

            Console.WriteLine("Performing operation");
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
        private static void ValidateInput(string? userEntry, bool generateRandomized, int size)
        {
            if (userEntry == debugKey)
            {
                DebugMain();
                return;
            }

            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directory != null)
            {
                //unit tests
                if (userEntry.Contains("64"))
                {
                    var path = Path.Combine(directory, "SampleData", "64UnitTest.txt");
                    userEntry = path;
                }
                else if (userEntry.Contains("128"))
                {
                    var path = Path.Combine(directory, "SampleData", "128UnitTest.txt");
                    userEntry = path;
                }
                else if (userEntry.Contains("256"))
                {
                    var path = Path.Combine(directory, "SampleData", "256UnitTest.txt");
                    userEntry = path;
                }
                else if (userEntry.Contains("512"))
                {
                    var path = Path.Combine(directory, "SampleData", "512UnitTest.txt");
                    userEntry = path;
                }
                else if (userEntry.Contains("1024"))
                {
                    var path = Path.Combine(directory, "SampleData", "1024UnitTest.txt");
                    userEntry = path;
                }
            }

            if (string.IsNullOrEmpty(userEntry) || string.IsNullOrWhiteSpace(userEntry))
            {
                log.LogMessage(LogUtility.MessageType.Error, "Invalid input detected.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();

                Console.Clear();
                Main();
                return;
            }

            // Clean user input by removing quote characters that may interfere with path processing
            userEntry = Regex.Replace(userEntry, "\"", "");

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
                return;
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
                return;
            }

            // All validation passed, proceed to document processing
            if (generateRandomized)
            {
                ProcessData(filePath, fileName, size);
                return;
            }

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
        private static async void ProcessData(string filePath, string fileName, int documentSize = 0)
        {
            // Final safety check to ensure parameters are valid before processing
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            await docReader.ValidateData(filePath, fileName, documentSize);
            Process.Start("explorer.exe", filePath);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();

            Console.Clear();
            Main();
            return;
        }
    }
}