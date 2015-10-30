using System;
using MemExchange.Core.SharedDto;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IMarketOrder
    {
        string Symbol { get; }
        int Quantity { get; }
        WayEnum Way { get; }
        uint ExchangeOrderId { get; }
        int ClientId { get; }
        void SetExchangeOrderId(uint exchangeOrderId);
        void Modify(int newQuantity);

        void RegisterModifyNotificationHandler(Action<IMarketOrder, int> handler);
        void UnRegisterModifyNotificationHandler(Action<IMarketOrder, int> handler);
        void Delete();
        void Dispose();
    }
}
