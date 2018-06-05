using System;
using System.Threading.Tasks;

using Simpbot.Core.Dto;

namespace Simpbot.Core.Contracts
{
    public interface ISimpbot : IDisposable
    {
        /// <summary>
        /// Starts to listening for messages
        /// </summary>
        /// <returns></returns>
        Task StartAsync();
        Action StopCallback { get; set; }
    }
}