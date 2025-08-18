using ConsoleInfo;
using System.Text.RegularExpressions;
namespace DocumentReader.Utils
{
    /// <summary>
    /// Static utility class providing text processing, cleaning, and analysis functionality.
    /// Handles string array processing, word counting, and character counting with comprehensive validation.
    /// </summary>
    public static class StringFormatter
    {
        public static readonly int TASKLIMIT = 500;

        /// <summary>
        /// Processes and cleans an array of strings by combining, sanitizing, and normalizing the text.
        /// Removes special characters, normalizes whitespace, and converts to lowercase for consistent analysis.
        /// </summary>
        /// <param name="input">
        /// • Array of strings to process and clean
        /// • Each element represents a line or section of text
        /// • Null or empty arrays return null with error logging
        /// </param>
        /// <returns>
        /// • Single cleaned string with normalized formatting
        /// • Returns null if input is invalid or processing fails
        /// • Output is lowercase, trimmed, and contains only alphanumeric characters and spaces
        /// </returns>
        public static async Task<string?> ProcessStringArray(string[]? input)
        {
            var log = LogUtility.Current;

            // Validate input array exists and contains data
            if (input == null || input.Length <= 0)
            {
                log.LogMessage(LogUtility.MessageType.Error, "input value is invalid.");
                return null;
            }

            // Combine all lines into single string with space separation
            string combinedValue = new("");

            //Using a list over a hashset in the event, there are duplicate lines within a document.
            var lineCollection = new List<string>();
            var taskCollection = new List<Task<string>>();

            //Process lines in batches of 10 thousand per thread.
            var taskLimit = input.Length < TASKLIMIT ? input.Length : TASKLIMIT;
            var startTaskLimit = taskLimit;
            for (var i = 0; i < input.Length; i++)
            {
                lineCollection.Add(input[i]);
                if (lineCollection.Count >= startTaskLimit)
                {
                    var task = JoinLinesSafely(lineCollection);
                    taskCollection.Add(task);
                    Task.Run(() => task);

                    var nextRound = i + taskLimit;
                    if (nextRound > input.Length)
                    {
                        nextRound -= input.Length;
                    }

                    startTaskLimit = nextRound;
                    lineCollection.Clear();
                }
            }

            await Task.WhenAll(taskCollection);

            foreach (var line in taskCollection)
            {
                combinedValue += line.Result;
            }

            // Verify combined string has content
            if (combinedValue.Length <= 0)
            {
                log.LogMessage(LogUtility.MessageType.Error, "Internal Error: No valid data found.");
                return null;
            }

            // Return cleaned, lowercase, trimmed text ready for analysis
            return combinedValue.ToLower().Trim();
        }

        private async static Task<string> JoinLinesSafely(List<string> input)
        {
            string combinedInput = new("");
            var stringArray = input.ToArray();
            for (var i = 0; i < stringArray.Length; i++)
            {
                // Add space between lines except for the last line
                if (i != stringArray.Length)
                {
                    stringArray[i] += " ";
                }

                combinedInput += stringArray[i];
            }


            // Remove all non-alphanumeric characters except spaces (preserves word boundaries)
            combinedInput = Regex.Replace(combinedInput, @"[^0-9a-zA-Z ]+", "");
            // Normalize multiple spaces to single spaces for consistent word separation
            combinedInput = Regex.Replace(combinedInput, @"\s+", " ");
            return combinedInput;
        }

        public static string[] GenerateRandomizeDocument(string input, int totalSize)
        {
            var sizeInKB = totalSize * 1024;
            var random = new Random();
            var randomizer = new Random();
            var wordList = new List<string>();
            string tempWordList = new("");

            var _ = WordCount(input, out var wordLibrary);

            for (var i = 0; i < sizeInKB;)
            {
                var wordToAdd = wordLibrary[random.Next(wordLibrary.Length)];
                tempWordList += $"{wordToAdd} ";

                //Build random sentences
                if (randomizer.Next(100) <= 5)
                {
                    tempWordList.Trim();
                    wordList.Add(tempWordList);
                    tempWordList = "";
                }

                //Build until we reach around the desired size.
                i += wordToAdd.Length;
            }

            return wordList.ToArray();
        }

        /// <summary>
        /// Counts the total number of words in a text string and returns the word array.
        /// Splits text on common whitespace characters for accurate word separation.
        /// </summary>
        /// <param name="input">
        /// • Text string to analyze for word count
        /// • Should be cleaned/processed text for best results
        /// • Null or empty strings return 0 count
        /// </param>
        /// <param name="count">
        /// • Output parameter containing array of individual words
        /// • Each element is a separate word from the input text
        /// • Returns null if input is invalid
        /// </param>
        /// <returns>
        /// • Total count of words found in the input text
        /// • Returns 0 if input is null, empty, or invalid
        /// </returns>
        public static int WordCount(string? input, out string[]? count)
        {
            count = null;
            if (string.IsNullOrEmpty(input))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            // Split on various whitespace characters to handle different text formats
            count = input.Trim().Split(' ', '\t', '\n', '\r');
            return count.Length;
        }

        /// <summary>
        /// Counts the total number of characters in a single string.
        /// Provides simple character-level analysis for text statistics.
        /// </summary>
        /// <param name="input">
        /// • Text string to count characters in
        /// • Includes all characters: letters, numbers, spaces, punctuation
        /// • Null or empty strings return 0
        /// </param>
        /// <returns>
        /// • Total character count including spaces and special characters
        /// • Returns 0 if input is null or empty
        /// </returns>
        public static int CharacterCount(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            return input.Length;
        }

        /// <summary>
        /// Counts the total number of characters across multiple strings (typically file lines).
        /// Intentionally excludes line ending characters for more accurate content-based counting.
        /// </summary>
        /// <param name="input">
        /// • Array of strings representing lines or sections of text
        /// • Each element's length is counted individually
        /// • Null or empty arrays return 0
        /// </param>
        /// <returns>
        /// • Total character count across all strings, excluding line endings
        /// • Returns 0 if input array is null or empty
        /// • Useful for analyzing original file content before processing
        /// </returns>
        public static int CharacterCount(string[]? input)
        {
            if (input == null || input.Length <= 0)
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, "data is not valid.");
                return 0;
            }

            var value = 0;

            // Sum character counts from each line, intentionally ignoring line endings
            // This provides a more accurate representation of actual text content
            foreach (var item in input)
            {
                value += item.Length;
            }

            return value;
        }
    }
}