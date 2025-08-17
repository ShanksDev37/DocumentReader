using ConsoleInfo;

namespace DocumentReader.Components
{
    public abstract class DocumentProcessorBase
    {
        public enum SupportedFileTypes
        {
            txt,
            csv,
            json
        }

        public bool ValidateData(string filePath, string fileName, out string[]? validatedFile)
        {
            validatedFile = null;
            var joinedFile = $"{filePath}\\{fileName}";

            if (string.IsNullOrEmpty(joinedFile))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"file path cannot be null");
                return false;
            }

            if (File.Exists(joinedFile) == false)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Selected file is invalid.");
                return false;
            }

            var extention = Path.GetExtension(joinedFile);
            if (extention == null)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Failed to find extention.");
                return false;
            }
            extention = extention.ToLower().Replace(".", "");

            var SupportedFiles = Enum.GetNames(typeof(SupportedFileTypes));
            if (SupportedFiles.Contains(extention.ToLower()) == false)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"Invalid file type selected.");
                return false;
            }

            validatedFile = File.ReadAllLines(joinedFile);
            if (validatedFile.Length == 0)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Warning, $"No data found in file.");
                return false;
            }

            return true;
        }
    }
}
