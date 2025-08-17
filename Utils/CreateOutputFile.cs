using ConsoleInfo;

namespace DocumentReader.Utils
{
    /// <summary>
    /// Static utility class for creating and saving processed document output files.
    /// Handles directory creation, file naming conventions, and content writing with logging support.
    /// </summary>
    public static class CreateOutputFile
    {
        /// <summary>
        /// Creates an output file with processed content in a specified subdirectory.
        /// Automatically creates the target directory if it doesn't exist and handles null content gracefully.
        /// Outputs each line to both console and file for real-time monitoring.
        /// </summary>
        /// <param name="filepath">Base directory path where the output folder will be created</param>
        /// <param name="subDirectory">Name of the subdirectory to create for organized file storage</param>
        /// <param name="savePrefix">Prefix to add to the original filename (e.g., "New-", "Processed-")</param>
        /// <param name="fileName">Original filename to use as the base for the output file</param>
        /// <param name="input">Array of strings containing the processed content to write to file</param>
        /// <returns>Completed task for async compatibility</returns>
        public static Task CreateFile(string filepath, string subDirectory, string savePrefix, string fileName, string[] input)
        {
            var log = LogUtility.Current;
            log.LogMessage(LogUtility.MessageType.Log, "Saving File.");

            // Construct path for organized output directory (e.g., "C:\Documents\Edited")
            var newDirectory = $"{filepath}\\{subDirectory}";

            // Create output directory if it doesn't exist
            if (Directory.Exists(newDirectory) == false)
            {
                Directory.CreateDirectory(newDirectory);
            }

            // Create output file with naming convention: {prefix}{originalFileName}
            // Example: "New-document.txt" in the "Edited" folder
            var file = File.CreateText($"{newDirectory}\\{savePrefix}{fileName}");

            // Write each line to both console and file, skipping null entries
            foreach (var item in input)
            {
                // Skip null entries to prevent file corruption or errors
                if (item == null)
                    continue;

                // Display content in real-time for user feedback
                Console.WriteLine(item);
                // Write to output file
                file.WriteLine(item);
            }

            // Ensure file is properly closed and saved
            file.Close();
            log.LogMessage(LogUtility.MessageType.Log, $"File Saved, to {newDirectory}");
            return Task.CompletedTask;
        }
    }
}