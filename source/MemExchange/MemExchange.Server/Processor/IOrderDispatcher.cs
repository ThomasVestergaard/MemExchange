using System.Collections.Generic;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor
{
    public interface IOrderDispatcher
    {
        Dictionary<string, IOrderBook> OrderBooks { get; }

        void HandleAddStopLimitOrder(IStopLimitOrder stopLimitOrder);
        void HandleAddLimitOrder(ILimitOrder limitOrder);
        void HandleMarketOrder(IMarketOrder marketOrder);
        void HandDuoLimitOrderUpdate(ILimitOrder limitOrder1, double limitOrder1NewPrice, int limitOrder1NewQuantity, ILimitOrder limitOrder2, double limitOrder2NewPrice, int limitOrder2NewQuantity);
    }
}
