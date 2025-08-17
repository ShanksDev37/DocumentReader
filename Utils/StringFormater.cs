using ConsoleInfo;
using System.Text.RegularExpressions;

namespace DocumentReader.Utils
{
    public static class StringFormatter
    {
        public static async Task<string?> ProcessStringArray(string[]? input)
        {
            var log = LogUtility.Current;

            if (input == null || input.Length <= 0)
            {
                log.LogMessage(LogUtility.MessageType.Error, "input value is invalid.");
                return null;
            }

            string combinedValue = new("");
            for (var i = 0; i < input.Length; i++)
            {
                if (i != input.Length - 1)
                {
                    input[i] += " ";
                }

                combinedValue += input[i];
            }

            if (combinedValue.Length <= 0)
            {
                log.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return null;
            }

            combinedValue = Regex.Replace(combinedValue, @"[^0-9a-zA-Z ]+", "");
            combinedValue = Regex.Replace(combinedValue, @"\s+", " ");
            return combinedValue.ToLower().Trim();
        }

        public static int WordCount(string? input, out string[]? count)
        {
            count = null;
            if (string.IsNullOrEmpty(input))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            count = input.Split(' ', '\t', '\n', '\r');
            return count.Length;
        }

        public static int CharacterCount(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            return input.Length;
        }

        public static int CharacterCount(string[]? input)
        {
            if (input == null || input.Length <= 0)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            var value = 0;

            //Intentionally ignoring line endings, for a more accurate character count.
            foreach (var item in input)
            {
                value += item.Length;
            }

            return value;
        }
    }
}
