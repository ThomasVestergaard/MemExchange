using System;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IStopLimitOrder : IOrder
    {
        IBestPriceTrigger Trigger { get; }

        double LimitPrice { get; set; }
        double TriggerPrice { get; set; }
        int Quantity { get; set; }
        StopLimitOrderDto ToDto();
        void RegisterOrderRepositoryDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void RegisterOrderBookDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void RegisterOutgoingQueueDeleteHandler(Action<IStopLimitOrder> deleteHandler);
        void UnRegisterOrderRepositoryDeleteHandler();
        void UnRegisterOrderBookDeleteHandler();
        void UnRegisterOutgoingQueueDeleteHandler();
        void Delete();

    }
}
