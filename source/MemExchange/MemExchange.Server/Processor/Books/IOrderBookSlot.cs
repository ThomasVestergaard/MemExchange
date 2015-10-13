using System.Collections.Generic;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Executions;

namespace MemExchange.Server.Processor.Books
{
    public interface IOrderBookSlot
    {
        double Price { get; }
        List<LimitOrder> Orders { get; }
        int TotalQuantity { get; }
        void AddOrder(LimitOrder order);
        void RemoveOrder(LimitOrder order);
        List<IExecution> MatchOrder(LimitOrder order);
    }
}
