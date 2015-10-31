using MemExchange.Core.SharedDto;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IOrder
    {
        WayEnum Way { get; }
        string Symbol { get; }
        uint ExchangeOrderId { get; }
        int ClientId { get; }
    }
}
