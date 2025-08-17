using ConsoleInfo;

namespace DocumentReader.Utils
{
    /// <summary>
    /// Processes text files to analyze word frequency, character counts, and generate statistical reports.
    /// Uses asynchronous processing for improved performance on large datasets.
    /// </summary>
    public static class ProcessTextData
    {
        /// <summary>
        /// Dictionary storing word frequency groups where key is occurrence count and value is formatted word list.
        /// </summary>
        private static Dictionary<int, string> wordDictionary = new();

        /// <summary>
        /// Total count of unique words found in the processed text.
        /// </summary>
        private static int totalUnique;

        /// <summary>
        /// Main processing method that validates, analyzes, and generates a statistical report for text files.
        /// Creates an output file with word frequency analysis and document statistics in a dedicated folder.
        /// </summary>
        /// <param name="filePath">Directory path containing the file</param>
        /// <param name="fileName">Name of the file to process</param>
        public static async Task ProcessData(string filePath, string fileName, string[] validFile)
        {
            var log = LogUtility.Current;
            log.LogMessage(LogUtility.MessageType.Log, "Started processing data.");

            if (validFile == null)
            {
                log.LogMessage(LogUtility.MessageType.Error, "Data is null.");
                return;
            }

            // Get character count from original file (excluding line endings)
            var totalCharacters = StringFormatter.CharacterCount(validFile);

            // Clean and format text (remove special chars, normalize spacing, convert to lowercase)
            var cleanedFile = StringFormatter.ProcessStringArray(validFile);
            if (cleanedFile == null)
            {
                log.LogMessage(LogUtility.MessageType.Error, "Processed Text is null.");
                return;
            }

            log.LogMessage(LogUtility.MessageType.Log, "Creating Word Collection.");

            // Count total words and get word array for analysis
            var totalWords = StringFormatter.WordCount(cleanedFile, out var wordCollection);

            log.LogMessage(LogUtility.MessageType.Log, "Processing common words.");
            List<string> output = new();

            // Process word frequency analysis asynchronously and populate wordDictionary
            await ProcessCommonWords(wordCollection);
            var commonWords = wordDictionary;
            if (commonWords == null)
            {
                log.LogMessage(LogUtility.MessageType.Error, "Common words is null.");
                return;
            }

            log.LogMessage(LogUtility.MessageType.Log, "Common words successfully processed.");
            log.LogMessage(LogUtility.MessageType.Log, "building save data.");

            // Build summary statistics section
            output.Add($"Total Lines: ({validFile.Length.ToString("#,##0")})");
            output.Add($"Total Words: ({totalWords.ToString("#,##0")})");
            output.Add($"Total Unique Words: ({totalUnique.ToString("#,##0")})");
            output.Add($"Total Character Count: ({totalCharacters.ToString("#,##0")})");

            // Generate detailed word frequency report with percentages
            var groupNumber = 1;

            string combinedEntry = new("");
            foreach (var entry in commonWords)
            {
                // Calculate percentage of total words for this frequency group
                var percentageValue = (float)entry.Key / totalWords * 100;
                var formattedPercentageValue = percentageValue < 0.001f ? "<0.001%" : $"{percentageValue.ToString("#0.##0")}%";
                var formattedTitle = entry.Key == 1 ? $"Once" : $"{entry.Key.ToString("#,##0")}";

                // Calculate percentage of unique words in this frequency group
                var entryUniqueWords = StringFormatter.WordCount(entry.Value, out _);
                var uniquePercentageValue = (float)entryUniqueWords / totalUnique * 100;
                var formattedUniquePercentage = uniquePercentageValue < 0.001f ? "<0.001%" : $"{uniquePercentageValue.ToString("#0.##0")}%";
                // Format frequency group header with statistics
                combinedEntry += $"\nGroup: {groupNumber}\n Total entries: ({formattedTitle}) Percentage Per Word: ({formattedPercentageValue}) " +
                    $"Unique Word Count: ({entryUniqueWords.ToString("#,##0")}) Total Unique Word Percentage: ({formattedUniquePercentage})\n";

                // Add formatted list of words that appear this many times
                combinedEntry += entry.Value;
                output.Add(combinedEntry);

                groupNumber++;
                combinedEntry = "";
            }

            // Save results to "Edited" folder with "New-" prefix
            await CreateOutputFile.CreateFile(filePath, "Edited", "New-", fileName, output.ToArray());

            log.LogMessage(LogUtility.MessageType.Log, "successfully processed all data.");
            return;
        }

        /// <summary>
        /// Asynchronously processes word frequency analysis using parallel tasks.
        /// Groups words by their occurrence count and populates the wordDictionary field.
        /// Uses Task.WhenAll for concurrent processing of different frequency groups.
        /// </summary>
        /// <param name="input">Array of words to analyze for frequency patterns</param>
        private static async Task ProcessCommonWords(string[]? input)
        {
            totalUnique = 0;
            wordDictionary.Clear();

            if (input == null || input.Length < 0)
            {
                return;
            }

            // Group words by frequency and sort by count (descending order)
            var keyPair = input.ToList().GroupBy(x => x).Select(y => new { Word = y.Key, Count = y.Count() });
            keyPair = keyPair.OrderByDescending(x => x.Count).Take(input.Length);
            var wordArray = keyPair.ToArray();

            totalUnique = wordArray.Length;

            // Convert to dictionary for efficient word-to-count lookup
            Dictionary<string, int> keyConversion = new();
            foreach (var value in wordArray)
            {
                keyConversion.Add(value.Word, value.Count);
            }

            // Get unique frequency values to avoid duplicate processing
            List<int> groupedEntries = new();
            foreach (var value in keyConversion.Values)
            {
                if (groupedEntries.Contains(value))
                    continue;

                groupedEntries.Add(value);
            }

            // Create parallel tasks for processing each frequency group
            List<Task> tasks = new();
            foreach (var value in groupedEntries)
            {
                Task task = GetWords(keyConversion, value);
                tasks.Add(task);
                Task.Run(() => task);
            }

            // Wait for all frequency group processing to complete
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Helper method that finds all words with a specific occurrence count and formats them.
        /// Adds the formatted word list to the wordDictionary with the frequency as the key.
        /// </summary>
        /// <param name="input">Dictionary mapping words to their occurrence counts</param>
        /// <param name="value">Target frequency count to filter words by</param>
        /// <returns>Completed task for async processing</returns>
        private static Task GetWords(Dictionary<string, int> input, int value)
        {
            string entries = new("");

            // Find all words that appear exactly 'value' times
            foreach (var word in input)
            {
                if (word.Value < value)
                    break;

                if (word.Value != value)
                    continue;

                // Format each word in parentheses
                entries += $" ({word.Key})";
            }

            // Store formatted word list in dictionary with frequency as key
            if (wordDictionary.ContainsKey(value))
            {
                LogUtility.Current.LogMessage(LogUtility.MessageType.Error, $"Attempted to assign the same key, {value}");

                return Task.CompletedTask;
            }

            wordDictionary.Add(value, entries);
            return Task.CompletedTask;
        }
    }
}