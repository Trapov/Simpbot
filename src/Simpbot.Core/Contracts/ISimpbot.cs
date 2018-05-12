using System;
using System.Threading.Tasks;

using Simpbot.Core.Dto;

namespace Simpbot.Core.Contracts
{
    public interface ISimpbot : IDisposable
    {
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="message">a text message</param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        Task SendMessage(Message message, ulong channelId);

        /// <summary>
        /// Starts to listening for messages
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        Action StopCallback { get; set; }
    }
}