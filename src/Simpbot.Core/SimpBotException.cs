using System;

namespace Simpbot.Core
{
    [Serializable]
    public class SimpbotException : Exception
    {
        public SimpbotException(string message) : base(message)
        {   
        }

        public SimpbotException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}