// See https://aka.ms/new-console-template for more information
using ConsoleInfo;
using DocumentReader.Components;

TextDocumentProcessor processor = new TextDocumentProcessor();
var log = LogUtility.Current;

entry:
Console.WriteLine("Enter your books file path");
var userEntry = Console.ReadLine();

if (Path.Exists(userEntry) == false)
{
    log.LogMessage(LogUtility.MessageType.Error, "Invalid file path detected.");
    Console.WriteLine("Press any key to continue.");
    Console.ReadKey();

    Console.Clear();
    goto entry;
}

Console.WriteLine($"valid path detected: {userEntry}");

var filePath = Path.GetDirectoryName(userEntry);
if (string.IsNullOrEmpty(filePath))
{
    log.LogMessage(LogUtility.MessageType.Error, "Invalid file path detected.");
    Console.WriteLine("Press any key to continue.");
    Console.ReadKey();

    Console.Clear();
    goto entry;
}

var fileName = Path.GetFileName(userEntry);
if (string.IsNullOrEmpty(fileName))
{
    log.LogMessage(LogUtility.MessageType.Error, "Invalid file name detected.");
    Console.WriteLine("Press any key to continue.");
    Console.ReadKey();

    Console.Clear();
    goto entry;
}

await processor.ProcessData(filePath, fileName);