using System.Collections.Generic;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderBook
    {
        Dictionary<double, IPriceSlot> PriceSlots { get; }
        string Symbol { get; }
        void HandleLimitOrder(ILimitOrder limitOrder);
        void HandleMarketOrder(IMarketOrder marketOrder);
        void RemoveLimitOrder(ILimitOrder limitOrder);
        void HandleOrderModify(ILimitOrder order, int oldQuantity, double oldPrice);
        
    }
}
