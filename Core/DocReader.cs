using ConsoleInfo;
using DocumentReader.Utils;

namespace DocumentReader.Core
{
    /// <summary>
    /// Abstract base class providing common file validation functionality for document processors.
    /// Defines supported file types and implements comprehensive file validation logic.
    /// Serves as foundation for specialized document processing classes.
    /// </summary>
    public class DocReader
    {
        /// <summary>
        /// Enumeration defining the file types supported by the document processing system.
        /// Used for file extension validation to ensure only compatible formats are processed.
        /// </summary>
        public enum SupportedFileTypes
        {
            /// <summary>Plain text files (.txt)</summary>
            txt,
            /// <summary>Comma-separated values files (.csv)</summary>
            csv,
            /// <summary>JavaScript Object Notation files (.json)</summary>
            json
        }

        /// <summary>
        /// Validates file existence, format compatibility, and content availability.
        /// Performs comprehensive checks to ensure the file can be safely processed.
        /// </summary>
        /// <param name="filePath">
        /// • Directory path containing the target file
        /// • Should be a valid directory path without the filename
        /// • Example: "C:\Documents\Data"
        /// </param>
        /// <param name="fileName">
        /// • Name of the file to validate including extension
        /// • Must have a supported file extension (.txt, .csv, .json)
        /// • Example: "document.txt", "data.csv"
        /// </param>
        /// <param name="validatedFile">
        /// • Output parameter containing file contents as string array
        /// • Each array element represents one line from the file
        /// • Returns null if validation fails
        /// </param>
        /// <returns>
        /// • True if file passes all validation checks and content is loaded
        /// • False if any validation step fails (logged with specific error details)
        /// </returns>
        public async Task<bool> ValidateData(string filePath, string fileName, int documentSize = 0)
        {
            // Combine path and filename to create full file path
            var joinedFile = $"{filePath}\\{fileName}";

            // Check if the combined file path is valid
            if (string.IsNullOrEmpty(joinedFile))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"file path cannot be null");
                return false;
            }

            // Verify file exists at the specified location
            if (File.Exists(joinedFile) == false)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Selected file is invalid.");
                return false;
            }

            // Extract and validate file extension
            var extention = Path.GetExtension(joinedFile);
            if (extention == null)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Failed to find extention.");
                return false;
            }

            // Normalize extension format (remove dot, convert to lowercase)
            extention = extention.ToLower().Replace(".", "");

            // Check if file extension is in the supported formats list
            var SupportedFiles = Enum.GetNames(typeof(SupportedFileTypes));
            if (SupportedFiles.Contains(extention.ToLower()) == false)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Invalid file type selected.");
                return false;
            }

            if (extention.Contains(SupportedFileTypes.txt.ToString()))
            {
                // Load file content and verify it contains data
                var validatedFile = File.ReadAllLines(joinedFile);
                if (validatedFile.Length == 0)
                {
                    LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"No data found in file.");
                    return false;
                }

                await ProcessTextData.ProcessData(filePath, fileName, validatedFile, documentSize);
            }
            else if (extention.Contains(SupportedFileTypes.csv.ToString()))
            {
                
            }

            // All validation checks passed successfully
            return true;
        }
    }
}