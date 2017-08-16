using Discord;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DragonLore.Managers
{
    internal class LogManager
    {
        public async Task Logger(LogMessage message)
        {
            var cc = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            var logMessage = $"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}";

            Console.WriteLine(logMessage);
            Console.ForegroundColor = cc;

            await WriteToFile(logMessage);
        }

        private async Task WriteToFile(string logMessage)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            Directory.CreateDirectory(folder);

            var logFile = Path.Combine(folder, $"{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}.log");

            var stream = File.Open(logFile, FileMode.Append);

            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteLineAsync(logMessage);
            }
        }
    }
}