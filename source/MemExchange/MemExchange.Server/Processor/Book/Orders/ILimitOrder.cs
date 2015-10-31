using System;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface ILimitOrder : IOrder
    {
        
        int Quantity { get; }
        double Price { get; }
        void SetExchangeOrderId(uint exchangeOrderId);
        void Modify(int newQuantity, double newPrice);
        void Modify(int newQuantity);

        void RegisterFilledNotification(Action<ILimitOrder> handler);
        void UnRegisterFilledNotification(Action<ILimitOrder> handler);
        void RegisterDeleteNotificationHandler(Action<ILimitOrder> handler);
        void UnRegisterDeleteNotificationHandler(Action<ILimitOrder> handler);
        void RegisterModifyNotificationHandler(Action<ILimitOrder, int, double> handler);
        void UnRegisterModifyNotificationHandler(Action<ILimitOrder, int, double> handler);
        void Delete();
        void Dispose();

        LimitOrderDto ToDto();

    }
}