using ConsoleInfo;

namespace DocumentReader.Components
{
    public static class CreateOutputFile
    {
        public static Task CreateFile(string filepath, string fileName, string[] input)
        {
            var log = LogUtility.Current;
            log.LogMessage(LogUtility.MessageType.Log, "Saving File.");

            var file = File.CreateText($"{filepath}/Updated-{fileName}");
            foreach (var item in input)
            {
                Console.WriteLine(item);
                file.WriteLine(item);
            }

            file.Close();
            log.LogMessage(LogUtility.MessageType.Log, $"File Saved, to {filepath}");
            return Task.CompletedTask;
        }
    }
}
