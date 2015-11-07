using System.Collections.Generic;
using MemExchange.Server.Processor.Book.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderBook
    {
        List<IStopLimitOrder> BuySideStopLimitOrders { get; }
        List<IStopLimitOrder> SellSideStopLimitOrders { get; }
        void AddStopLimitOrder(IStopLimitOrder stopLimitOrder);
        void RemoveStopLimitOrder(IStopLimitOrder stopLimitOrder);

        Dictionary<double, IPriceSlot> PriceSlots { get; }
        string Symbol { get; }
        void HandleLimitOrder(ILimitOrder limitOrder);
        void HandleMarketOrder(IMarketOrder marketOrder);
        void RemoveLimitOrder(ILimitOrder limitOrder);
        void HandleOrderModify(ILimitOrder order, int oldQuantity, double oldPrice);
        
    }
}
