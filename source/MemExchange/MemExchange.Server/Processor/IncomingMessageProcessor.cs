using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Server.Outgoing;

namespace MemExchange.Server.Processor
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private readonly IOrderKeep orderKeep;
        private readonly IOutgoingQueue outgoingQueue;

        public IncomingMessageProcessor(IOrderKeep orderKeep, IOutgoingQueue outgoingQueue)
        {
            this.orderKeep = orderKeep;
            this.outgoingQueue = outgoingQueue;
        }

        public void OnNext(IClientToServerMessage data, long sequence, bool endOfBatch)
        {
            switch (data.MessageType)
            {
                case ClientToServerMessageTypeEnum.PlaceOrder:
                    if (!data.LimitOrder.ValidatesForAdd())
                    {
                        outgoingQueue.EnqueueMessage(data.ClientId, "Error: Limit order was rejected.");
                        break;
                    }

                    var addResult = orderKeep.AddLimitOrder(data.LimitOrder);
                    outgoingQueue.EnqueueAddedLimitOrder(addResult);
                break;

                case ClientToServerMessageTypeEnum.CancelOrder:
                    if (!data.LimitOrder.ValidateForDelete())
                    {
                        outgoingQueue.EnqueueMessage(data.ClientId, "Error: Cancellation of limit order was rejected.");
                        break;
                    }

                    var orderWasDeleted = orderKeep.DeleteLimitOrder(data.LimitOrder);
                    if (orderWasDeleted)
                    {
                        outgoingQueue.EnqueueDeletedLimitOrder(data.LimitOrder);
                        outgoingQueue.EnqueueMessage(data.ClientId, "Limit order cancelled.");
                    }
                    break;

                case ClientToServerMessageTypeEnum.ModifyOrder:
                    if (!data.LimitOrder.ValidatesForModify())
                    {
                        outgoingQueue.EnqueueMessage(data.ClientId, "Error: Modification of limit order was rejected.");
                        break;
                    }

                    var modifiedLimitOrder = orderKeep.TryUpdateLimitOrder(data.LimitOrder);
                    if (modifiedLimitOrder != null)
                        outgoingQueue.EnqueueUpdatedLimitOrder(modifiedLimitOrder);

                    break;
            }

            data.Reset();
        }
    }
}