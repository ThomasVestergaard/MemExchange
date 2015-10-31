using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderRepository
    {
        ILimitOrder NewLimitOrder(LimitOrderDto dtoLimitOrder);
        ILimitOrder NewLimitOrder(string symbol, int clientId, double price, int quantity, WayEnum way);
        ILimitOrder TryGetOrder(uint exchangeOrderId);
        IMarketOrder NewMarketOrder(string symbol, int clientId, int quantity, WayEnum way);
        IMarketOrder NewMarketOrder(MarketOrderDto dtoMarketOrder);
        List<ILimitOrder> GetClientOrders(int clientId);
    }
}
