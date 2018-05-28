using System;
using System.Threading.Tasks;
using Discord;

namespace Simpbot.Core
{
    public class ColorfullConsoleLogger : ICustomLogger
    {
        public Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public Task Log(Exception logMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            return Task.CompletedTask;
        }
    }
}