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
        List<ILimitOrder> GetClientOrders(int clientId);
    }
}
