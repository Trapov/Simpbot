using System;
using System.Threading.Tasks;
using Discord;

namespace Simpbot.Core
{
    public interface ICustomLogger
    {
        Task Log(LogMessage logMessage);
        Task Log(Exception logMessage);
    }
}