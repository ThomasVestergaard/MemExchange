using System.Collections.Generic;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Outgoing
{
    public interface IOutgoingQueue
    {
        void Start();
        void Stop();

        void EnqueueAddedLimitOrder(ILimitOrder limitOrder);
        void EnqueueUpdatedLimitOrder(ILimitOrder limitOrder, int oldQuantity, double oldPrice);
        void EnqueueDeletedLimitOrder(ILimitOrder limitOrder);
        void EnqueueMessage(int clientId, string message);
        void EnqueueLimitOrderSnapshot(int clientId, List<ILimitOrder> orders);
        void EnqueueStopLimitOrderSnapshot(int clientId, List<IStopLimitOrder> orders);
        void EnqueueClientExecution(INewExecution execution);
        void EnqueueLevel1Update(IOrderBookBestBidAsk orderBookBestBidAsk);
        void EnqueueAddedStopLimitOrder(IStopLimitOrder stopLimitOrder);
        void EnqueueUpdatedStopLimitOrder(IStopLimitOrder stopLimitOrder);
        void EnqueueDeletedStopLimitOrder(IStopLimitOrder stopLimitOrder);
    }
}
