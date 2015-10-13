using System.Collections.Generic;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Common;
using MemExchange.Server.Incoming;
using MemExchange.Server.Outgoing;

namespace MemExchange.Server.Processor
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private readonly IOrderKeep orderKeep;
        private readonly IOutgoingQueue outgoingQueue;
        private readonly IDateService dateService;

        public IncomingMessageProcessor(IOrderKeep orderKeep, IOutgoingQueue outgoingQueue, IDateService dateService)
        {
            this.orderKeep = orderKeep;
            this.outgoingQueue = outgoingQueue;
            this.dateService = dateService;
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

                    LimitOrder addedOrder;
                    orderKeep.AddLimitOrder(data.Message.LimitOrder, out addedOrder);
                    outgoingQueue.EnqueueAddedLimitOrder(addedOrder);
                break;

                case ClientToServerMessageTypeEnum.CancelOrder:
                if (!data.Message.LimitOrder.ValidateForDelete())
                    {
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Error: Cancellation of limit order was rejected.");
                        break;
                    }

                var orderWasDeleted = orderKeep.DeleteLimitOrder(data.Message.LimitOrder);
                    if (orderWasDeleted)
                    {
                        outgoingQueue.EnqueueDeletedLimitOrder(data.Message.LimitOrder);
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Limit order cancelled.");
                    }
                    break;

                case ClientToServerMessageTypeEnum.ModifyOrder:
                    if (!data.Message.LimitOrder.ValidatesForModify())
                    {
                        outgoingQueue.EnqueueMessage(data.Message.ClientId, "Error: Modification of limit order was rejected.");
                        break;
                    }

                    LimitOrder modifiedOrder;
                    var modifyResult = orderKeep.TryUpdateLimitOrder(data.Message.LimitOrder, out modifiedOrder);
                    if (modifyResult)
                        outgoingQueue.EnqueueUpdatedLimitOrder(modifiedOrder);
                    
                    break;

                case ClientToServerMessageTypeEnum.RequestOpenOrders:
                    if (data.Message.ClientId <= 0)
                        break;

                    var orderList = new List<LimitOrder>();
                    orderKeep.GetClientOrders(data.Message.ClientId, out orderList);
                    outgoingQueue.EnqueueOrderSnapshot(data.Message.ClientId, orderList);
                    break;
            }

            data.Message.Reset();
        }
    }
}