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
                        break;

                    var addResult = orderKeep.AddLimitOrder(data.LimitOrder);
                    outgoingQueue.EnqueueAddedLimitOrder(addResult);
                break;
            }


            data.Reset();
        }
    }
}