using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ClientToServer;
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
        private readonly ISerializer serializer;
        private ClientToServerMessage deserializedMessage;
        private byte[] queueBuffer;

        public IncomingMessageProcessor(IOrderRepository ordeRepository, IOutgoingQueue outgoingQueue, IDateService dateService, IOrderDispatcher dispatcher, ISerializer serializer)
        {
            this.ordeRepository = ordeRepository;
            this.outgoingQueue = outgoingQueue;
            this.dateService = dateService;
            this.dispatcher = dispatcher;
            this.serializer = serializer;
            queueBuffer = new byte[512];
        }
        
        public void OnNext(RingbufferByteArray data, long sequence, bool endOfBatch)
        {
            data.StartProcessTime = dateService.UtcNow();

            data.GetContent(ref queueBuffer);
            deserializedMessage = serializer.Deserialize<ClientToServerMessage>(queueBuffer, data.ContentLength);

            if (deserializedMessage == null)
                return;

            switch (deserializedMessage.MessageType)
            {
               case ClientToServerMessageTypeEnum.ModifyStopLimitOrder:
                    if (deserializedMessage.ClientId <= 0)
                        break;

                    var stopLimitOrderToModify = ordeRepository.TryGetStopLimitOrder(deserializedMessage.StopLimitOrder.ExchangeOrderId);
                    if (stopLimitOrderToModify == null)
                        return;

                    stopLimitOrderToModify.Modify(deserializedMessage.StopLimitOrder.TriggerPrice, deserializedMessage.StopLimitOrder.LimitPrice, deserializedMessage.StopLimitOrder.Quantity);
                    outgoingQueue.EnqueueUpdatedStopLimitOrder(stopLimitOrderToModify);

                    break;

               case ClientToServerMessageTypeEnum.RequestOpenStopLimitOrders:
                    if (deserializedMessage.ClientId <= 0)
                        break;

                    var orders = ordeRepository.GetClientStopLimitOrders(deserializedMessage.ClientId);
                    if (orders.Count == 0)
                        return;

                    outgoingQueue.EnqueueStopLimitOrderSnapshot(deserializedMessage.ClientId, orders);
                    break;

                case ClientToServerMessageTypeEnum.CancelStopLimitOrder:
                    var stopOrderToCancel = ordeRepository.TryGetStopLimitOrder(deserializedMessage.StopLimitOrder.ExchangeOrderId);

                    if (stopOrderToCancel != null)
                    {
                        stopOrderToCancel.Delete();
                        outgoingQueue.EnqueueDeletedStopLimitOrder(stopOrderToCancel);
                    }
                    break;

                case ClientToServerMessageTypeEnum.PlaceStopLimitOrder:
                    if (!deserializedMessage.StopLimitOrder.ValidateForAdd())
                        return;

                    var newStopLimitOrder = ordeRepository.NewStopLimitOrder(deserializedMessage.StopLimitOrder);
                    dispatcher.HandleAddStopLimitOrder(newStopLimitOrder);
                    break;


                case ClientToServerMessageTypeEnum.PlaceMarketOrder:
                    if (!deserializedMessage.MarketOrder.ValidateForExecute())
                        return;

                    var newMarketOrder = ordeRepository.NewMarketOrder(deserializedMessage.MarketOrder);
                    dispatcher.HandleMarketOrder(newMarketOrder);
                    break;

                case ClientToServerMessageTypeEnum.PlaceLimitOrder:
                    if (!deserializedMessage.LimitOrder.ValidatesForAdd())
                    {
                        outgoingQueue.EnqueueMessage(deserializedMessage.ClientId, "Error: Limit order was rejected.");
                        break;
                    }

                    var newLimitOrder = ordeRepository.NewLimitOrder(deserializedMessage.LimitOrder);
                    newLimitOrder.RegisterDeleteNotificationHandler(outgoingQueue.EnqueueDeletedLimitOrder);
                    newLimitOrder.RegisterModifyNotificationHandler(outgoingQueue.EnqueueUpdatedLimitOrder);
                    newLimitOrder.RegisterFilledNotification(outgoingQueue.EnqueueDeletedLimitOrder);
                    newLimitOrder.RegisterFilledNotification((order) => order.Delete());

                    dispatcher.HandleAddLimitOrder(newLimitOrder);
                break;

                case ClientToServerMessageTypeEnum.CancelLimitOrder:
                if (!deserializedMessage.LimitOrder.ValidateForDelete())
                    {
                        outgoingQueue.EnqueueMessage(deserializedMessage.ClientId, "Error: Cancellation of limit order was rejected.");
                        break;
                    }

                var orderToDelete = ordeRepository.TryGetLimitOrder(deserializedMessage.LimitOrder.ExchangeOrderId);
                    if (orderToDelete != null)
                    {
                        orderToDelete.Delete();
                        outgoingQueue.EnqueueDeletedLimitOrder(orderToDelete);
                    }
                    break;

                case ClientToServerMessageTypeEnum.ModifyLimitOrder:
                    if (!deserializedMessage.LimitOrder.ValidatesForModify())
                    {
                        outgoingQueue.EnqueueMessage(deserializedMessage.ClientId, "Error: Modification of limit order was rejected.");
                        break;
                    }

                    var orderToModify = ordeRepository.TryGetLimitOrder(deserializedMessage.LimitOrder.ExchangeOrderId);
                    if (orderToModify != null)
                        orderToModify.Modify(deserializedMessage.LimitOrder.Quantity, deserializedMessage.LimitOrder.Price);
                    break;

                case ClientToServerMessageTypeEnum.RequestOpenLimitOrders:
                    if (deserializedMessage.ClientId <= 0)
                        break;

                    var orderList = ordeRepository.GetClientStopLimitOrders(deserializedMessage.ClientId);
                    outgoingQueue.EnqueueStopLimitOrderSnapshot(deserializedMessage.ClientId, orderList);
                    break;


                case ClientToServerMessageTypeEnum.DuoLimitOrderUpdate:
                    var order1ToModify = ordeRepository.TryGetLimitOrder(deserializedMessage.DuoLimitOrder.LimitOrder1.ExchangeOrderId);
                    var order2ToModify = ordeRepository.TryGetLimitOrder(deserializedMessage.DuoLimitOrder.LimitOrder2.ExchangeOrderId);

                    if (order1ToModify == null || order2ToModify == null)
                        return;

                    if (order1ToModify.Symbol != order2ToModify.Symbol)
                        return;

                    dispatcher.HandDuoLimitOrderUpdate(
                        order1ToModify, 
                        deserializedMessage.DuoLimitOrder.LimitOrder1.Price,
                        deserializedMessage.DuoLimitOrder.LimitOrder1.Quantity,
                        order2ToModify,
                        deserializedMessage.DuoLimitOrder.LimitOrder2.Price,
                        deserializedMessage.DuoLimitOrder.LimitOrder2.Quantity);

                    break;
            }

            deserializedMessage.Reset();
            
        }
    }
}