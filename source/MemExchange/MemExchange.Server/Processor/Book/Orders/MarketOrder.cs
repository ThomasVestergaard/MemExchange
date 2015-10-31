using System;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Book.Orders
{
    public class MarketOrder : LimitOrder, IMarketOrder
    {
        public MarketOrder(string symbol, int quantity, WayEnum way, int clientId) : base(symbol, quantity, 0, way, clientId)
        {
            if (way == WayEnum.Buy)
                Price = double.MaxValue;
            else if (way == WayEnum.Sell)
                Price = double.MinValue;
        }

        public MarketOrderDto ToDto()
        {
            throw new InvalidOperationException("Should not be used");
        }
    }
}