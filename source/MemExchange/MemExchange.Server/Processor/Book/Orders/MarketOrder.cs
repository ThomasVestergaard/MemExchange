using System;
using MemExchange.Core.SharedDto;

namespace MemExchange.Server.Processor.Book.Orders
{
    public class MarketOrder : IMarketOrder
    {
        public int Quantity { get; private set; }
        public WayEnum Way { get; private set; }
        public uint ExchangeOrderId { get; private set; }
        public int ClientId { get; private set; }
        public void SetExchangeOrderId(uint exchangeOrderId)
        {
            ExchangeOrderId = exchangeOrderId;
        }

        public void Modify(int newQuantity)
        {
            int oldQuantity = Quantity;
            Quantity = newQuantity;

        }

        public void RegisterModifyNotificationHandler(Action<IMarketOrder, int> handler)
        {
        }

        public void UnRegisterModifyNotificationHandler(Action<IMarketOrder, int> handler)
        {
        }

        private void RaiseOrderModified(IMarketOrder order, int oldQuantity)
        {
            
        }

        public void Delete()
        {
        }

        public void Dispose()
        {
        }

        public string Symbol { get; private set; }
    }
}