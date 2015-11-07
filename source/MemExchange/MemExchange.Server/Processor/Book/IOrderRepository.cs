using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderRepository
    {
        IStopLimitOrder NewStopLimitOrder(StopLimitOrderDto dtoStopLimitOrder);
        IStopLimitOrder NewStopLimitOrder(string symbol, int clientId, double triggerPrice, double limitPrice, int quantity, WayEnum way);
        IStopLimitOrder TryGetStopLimitOrder(uint exchangeOrderId);
        List<IStopLimitOrder> GetClientStopLimitOrders(int clientId);

        ILimitOrder NewLimitOrder(LimitOrderDto dtoLimitOrder);
        ILimitOrder NewLimitOrder(string symbol, int clientId, double price, int quantity, WayEnum way);
        ILimitOrder NewLimitOrder(IStopLimitOrder stopLimitOrder);
        ILimitOrder TryGetLimitOrder(uint exchangeOrderId);
        List<ILimitOrder> GetClientLimitOrders(int clientId);

        IMarketOrder NewMarketOrder(string symbol, int clientId, int quantity, WayEnum way);
        IMarketOrder NewMarketOrder(MarketOrderDto dtoMarketOrder);
        
    }
}
