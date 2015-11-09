using System;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IStopLimitOrder : IOrder
    {
        IBestPriceTrigger Trigger { get; }

        double LimitPrice { get; }
        double TriggerPrice { get; }
        int Quantity { get; }
        StopLimitOrderDto ToDto();
        void RegisterOrderRepositoryDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void RegisterOrderBookDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void RegisterOrderBookModifyHandler(Action<IStopLimitOrder> modifyHandler);
        void RegisterOutgoingQueueDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void UnRegisterOrderRepositoryDeleteHandler();
        void UnRegisterOrderBookDeleteHandler();
        void UnRegisterOrderBookModifyHandler();
        void UnRegisterOutgoingQueueDeleteHandler();
        void Delete();
        void Modify(double newTriggerPrice, double newLimitPrice, int newQuantity);
    }
}
