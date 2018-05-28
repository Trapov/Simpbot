using System;
using Discord;
using Microsoft.Extensions.Logging;

namespace Simpbot.Core
{
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
}