using System.Collections.Concurrent;

using Discord;

namespace Simpbot.Core
{
    public sealed class PrunedMessagesInMemoryService : ConcurrentStack<IMessage>
    {
        public void PushMessage(IMessage message) => Push(message);

        public IMessage PopMessage()
        {
            return TryPop(out var message) ? message : null;
        }
    }
}