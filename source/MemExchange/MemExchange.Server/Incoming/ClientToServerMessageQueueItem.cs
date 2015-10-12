using System;
using MemExchange.Core.SharedDto.ClientToServer;

namespace MemExchange.Server.Incoming
{
    public class ClientToServerMessageQueueItem
    {
        public ClientToServerMessage Message { get; set; }
        public DateTimeOffset StartProcessTime { get; set; }

        public ClientToServerMessageQueueItem()
        {
            Message = new ClientToServerMessage();
        }

        public void Update(ClientToServerMessage otherMessage)
        {
            Message.Update(otherMessage);
        }
    }
}
