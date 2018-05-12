using System;
using System.Threading.Tasks;

using Discord;
using Microsoft.Extensions.Logging;

namespace Simpbot.Core
{
    public class CustomLogger : ICustomLogger
    {
        public Task LogAsync(LogMessage msg)
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

        public Task LogAsync(Exception logMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            return Task.CompletedTask;
        }
    }

    public class LoggerAdapter
    {
        private readonly ILogger<Simpbot> _logger;

        public LoggerAdapter(ILogger<Simpbot> logger)
        {
            _logger = logger;
        }

        public void Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(msg.Exception, msg.Message, msg.Source);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(msg.Exception, msg.Message, msg.Source);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(msg.Exception, msg.Message, msg.Source);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(msg.Exception, msg.Message, msg.Source);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(msg.Exception, msg.Message, msg.Source);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(msg.Exception, msg.Message, msg.Source);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public interface ICustomLogger
    {
        Task LogAsync(LogMessage logMessage);
        Task LogAsync(Exception logMessage);
    }
}