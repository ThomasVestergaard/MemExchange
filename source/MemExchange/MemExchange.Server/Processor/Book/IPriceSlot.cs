using System.Collections.Generic;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public interface IPriceSlot
    {
        double Price { get; }
        List<ILimitOrder> BuyOrders { get; }
        List<ILimitOrder> SellOrders { get; }
        bool HasOrders { get; }
        bool HasBids { get; }
        bool HasAsks { get; }
        void TryMatchLimitOrder(ILimitOrder order);
        void TryMatchMarketOrder(IMarketOrder order);
        bool ContainsOrder(ILimitOrder order);
        void AddOrder(ILimitOrder order);
        void RemoveOrder(ILimitOrder order);
    }
}
