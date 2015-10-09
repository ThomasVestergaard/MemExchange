using System.Collections.Generic;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Outgoing
{
    public interface IOutgoingQueue
    {
        void Start();
        void Stop();

        void EnqueueAddedLimitOrder(LimitOrder limitOrder);
        void EnqueueUpdatedLimitOrder(LimitOrder limitOrder);
        void EnqueueDeletedLimitOrder(LimitOrder limitOrder);
        void EnqueueMessage(int clientId, string message);
        void EnqueueOrderSnapshot(int clientId, List<LimitOrder> orders);
    }
}
