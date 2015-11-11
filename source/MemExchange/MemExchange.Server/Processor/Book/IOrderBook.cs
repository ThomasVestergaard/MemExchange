using System.Collections.Generic;
using MemExchange.Server.Processor.Book.Orders;

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
        void AddLimitOrder(ILimitOrder limitOrder);
        void HandleMarketOrder(IMarketOrder marketOrder);
        void RemoveLimitOrder(ILimitOrder limitOrder);
        void HandleLimitOrderModify(ILimitOrder order, int oldQuantity, double oldPrice);

        void SetSuspendLimitOrderMatchingStatus(bool isSuspended);
        void TryMatchLimitOrder(ILimitOrder limitOrder);
    }
}
