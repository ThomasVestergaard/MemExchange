using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Common;
using MemExchange.Server.Incoming;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book;

namespace MemExchange.Server.Processor
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private readonly IOrderRepository ordeRepository;
        private readonly IOutgoingQueue outgoingQueue;
        private readonly IDateService dateService;
        private readonly IOrderDispatcher dispatcher;

        public IncomingMessageProcessor(IOrderRepository ordeRepository, IOutgoingQueue outgoingQueue, IDateService dateService, IOrderDispatcher dispatcher)
        {
            this.ordeRepository = ordeRepository;
            this.outgoingQueue = outgoingQueue;
            this.dateService = dateService;
            this.dispatcher = dispatcher;
        }

        public void OnNext(ClientToServerMessageQueueItem data, long sequence, bool endOfBatch)
        {
            data.StartProcessTime = dateService.UtcNow();

            switch (data.Message.MessageType)
            {
                case ClientToServerMessageTypeEnum.PlaceOrder:
                    if (!data.Message.LimitOrder.ValidatesForAdd())
                    {
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Error: Limit order was rejected.");
                        break;
                    }

                    var newLimitOrder = ordeRepository.NewLimitOrder(data.Message.LimitOrder);
                    newLimitOrder.RegisterDeleteNotificationHandler(outgoingQueue.EnqueueDeletedLimitOrder);
                    newLimitOrder.RegisterModifyNotificationHandler(outgoingQueue.EnqueueUpdatedLimitOrder);
                    newLimitOrder.RegisterFilledNotification(outgoingQueue.EnqueueDeletedLimitOrder);
                    newLimitOrder.RegisterFilledNotification((order) => order.Delete());

                    dispatcher.HandleAddOrder(newLimitOrder);
                break;

                case ClientToServerMessageTypeEnum.CancelOrder:
                if (!data.Message.LimitOrder.ValidateForDelete())
                    {
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Error: Cancellation of limit order was rejected.");
                        break;
                    }

                    var orderToDelete = ordeRepository.TryGetOrder(data.Message.LimitOrder.ExchangeOrderId);
                    if (orderToDelete != null)
                    {
                        orderToDelete.Delete();
                        outgoingQueue.EnqueueDeletedLimitOrder(orderToDelete);
                    }
                    break;

                case ClientToServerMessageTypeEnum.ModifyOrder:
                    if (!data.Message.LimitOrder.ValidatesForModify())
                    {
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Error: Modification of limit order was rejected.");
                        break;
                    }

                    var orderToModify = ordeRepository.TryGetOrder(data.Message.LimitOrder.ExchangeOrderId);
                    if (orderToModify != null)
                        orderToModify.Modify(data.Message.LimitOrder.Quantity, data.Message.LimitOrder.Price);
                    break;

                case ClientToServerMessageTypeEnum.RequestOpenOrders:
                    if (data.Message.ClientId <= 0)
                        break;

                    var orderList = ordeRepository.GetClientOrders(data.Message.ClientId);
                    outgoingQueue.EnqueueOrderSnapshot(data.Message.ClientId, orderList);
                    break;
            }

            data.Message.Reset();
        }
    }
}