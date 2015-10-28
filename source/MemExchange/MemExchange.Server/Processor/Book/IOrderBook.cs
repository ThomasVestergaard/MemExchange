using System.Collections.Generic;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderBook
    {
        Dictionary<double, IPriceSlot> PriceSlots { get; }
        string Symbol { get; }
        void HandleOrder(ILimitOrder limitOrder);
        void RemoveOrder(ILimitOrder limitOrder);
    }
}
