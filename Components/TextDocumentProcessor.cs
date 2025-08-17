using ConsoleInfo;
using DocumentReader.Utils;

namespace DocumentReader.Components
{
    /// <summary>
    /// Outlined success criteria:
    /// Word count analysis.
    /// Most frequent words.
    /// Character statistics.
    /// Line Count
    /// </summary>
    public class TextDocumentProcessor : DocumentProcessorBase
    {
        public async Task ProcessData(string filePath, string fileName)
        {
            var log = LogUtility.Current;
            log.LogMessage(LogUtility.MessageType.Log, "Validating data.");

            if (ValidateData(filePath, fileName, out var validatedFile) == false)
            {
                return;
            }

            log.LogMessage(LogUtility.MessageType.Log, "Validation successful.");

            if (validatedFile == null)
            {
                return;
            }

            log.LogMessage(LogUtility.MessageType.Log, "Started processing data.");

            var characterCount = StringFormatter.CharacterCount(validatedFile);
            var processedText = await StringFormatter.ProcessStringArray(validatedFile);
            if (processedText == null)
            {
                return;
            }

            var wordCount = StringFormatter.WordCount(processedText, out var words);

            log.LogMessage(LogUtility.MessageType.Log, "Processing common words.");
            List<string> output = new();
            var commonWords = ProcessCommonWords(words, out var uniqueWords);
            if (commonWords == null)
            {
                return;
            }

            log.LogMessage(LogUtility.MessageType.Log, "Common words successfully processed.");
            log.LogMessage(LogUtility.MessageType.Log, "building save file.");

            //Process base lines synchronously.
            output.Add($"Total Lines: ({validatedFile.Length}");
            output.Add($"Total Words: ({wordCount})");
            output.Add($"Total Unique Words: ({uniqueWords})");
            output.Add($"Total Character Count: ({characterCount})");

            string combinedEntry = new("");
            foreach (var entry in commonWords)
            {
                var percentageValue = (float)entry.Key / wordCount * 100;
                var formattedPercentageValue = percentageValue < 0.001f ? "<0.001%" : $"{percentageValue.ToString("#0.000")}%";
                var formattedTitle = entry.Key == 1 ? $"Once" : $"{entry.Key}";

                var entryUniqueWords = StringFormatter.WordCount(entry.Value, out _);
                var uniquePercentageValue = (float)entryUniqueWords / uniqueWords * 100;
                var formattedUniquePercentage = uniquePercentageValue < 0.001f ? "<0.001%" : $"{uniquePercentageValue.ToString("#0.000")}%";

                combinedEntry += $"\nTotal entries: ({formattedTitle}) Total Percentage: ({formattedPercentageValue}) " +
                    $"Unique Word Count: ({entryUniqueWords}) Unique Word Percentage ({formattedUniquePercentage})\n";

                combinedEntry += entry.Value;
                output.Add(combinedEntry);

                combinedEntry = "";
            }

            await CreateOutputFile.CreateFile(filePath, fileName, output.ToArray());

            log.LogMessage(LogUtility.MessageType.Log, "successfully processed data.");
            return;
        }

        public Dictionary<int, string>? ProcessCommonWords(string[]? input, out int outputUnique)
        {
            outputUnique = 0;

            if (input == null || input.Length < 0)
            {
                return null;
            }

            var keyPair = input.ToList().GroupBy(x => x).Select(y => new { Word = y.Key, Count = y.Count() });
            keyPair = keyPair.OrderByDescending(x => x.Count).Take(input.Length);
            var wordArray = keyPair.ToArray();

            outputUnique = wordArray.Length;

            var keyConversion = new Dictionary<string, int>();
            foreach (var value in wordArray)
            {
                keyConversion.Add(value.Word, value.Count);
            }

            string entries = new("");
            var wordDictionary = new Dictionary<int, string>();
            foreach (var value in keyConversion.Values)
            {
                entries = "";

                foreach (var key in wordArray)
                {
                    if (key.Count < value)
                        break;

                    if (key.Count != value)
                        continue;

                    entries += $" ({key.Word})";
                }

                if (wordDictionary.ContainsKey(value))
                    continue;

                wordDictionary.Add(value, entries.Trim());
            }

            return wordDictionary;
        }
    }
}
