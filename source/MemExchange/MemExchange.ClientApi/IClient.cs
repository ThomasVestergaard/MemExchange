using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.ClientApi
{
    public interface IClient
    {
        event EventHandler<LimitOrder> LimitOrderAccepted;
        event EventHandler<LimitOrder> LimitOrderChanged;
        event EventHandler<LimitOrder> LimitOrderDeleted;
        event EventHandler<List<LimitOrder>> LimitOrderSnapshot;

        void Start(int clientId, string serverAddress, int serverCommandPort, int serverPublishPort);
        void Stop();

        void SubmitLimitOrder(string symbol, double price, int quantity, WayEnum way);
        void ModifyLimitOrder(uint exchangeOrderId, double newPrice, int newQuantity);
        void CancelLimitOrder(uint exchangeOrderId);
        void RequestOpenLimitOrders();
    }
}
