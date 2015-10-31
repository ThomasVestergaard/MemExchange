using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IMarketOrder : ILimitOrder
    {
        new MarketOrderDto ToDto();
    }
}
