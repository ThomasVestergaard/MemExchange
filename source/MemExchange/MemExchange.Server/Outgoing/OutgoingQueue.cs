using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.Server.Outgoing
{
    public class OutgoingQueue : IOutgoingQueue
    {
        private int ringbufferSize = (int)Math.Pow(256, 2);
        private readonly ILogger logger;
        private readonly IMessagePublisher publisher;

        private Disruptor<ServerToClientMessage> messageDisrupter;
        private RingBuffer<ServerToClientMessage> messageRingBuffer;

        private ServerToClientMessage serverToClientMessage;

        public OutgoingQueue(ILogger logger, IMessagePublisher publisher)
        {
            this.logger = logger;
            this.publisher = publisher;
            serverToClientMessage = new ServerToClientMessage();
        }

        public void Start()
        {
            messageDisrupter = new Disruptor<ServerToClientMessage>(() => new ServerToClientMessage(), new SingleThreadedClaimStrategy(ringbufferSize), new SleepingWaitStrategy(), TaskScheduler.Default);
            messageDisrupter.HandleEventsWith(publisher);
            messageRingBuffer = messageDisrupter.Start();

            logger.Info("Outgoing queue message queue started.");
        }

        public void Stop()
        {
            messageDisrupter.Halt();
            logger.Info("Outgoing queue message queue stopped.");
        }

        private void Enqueue()
        {
            var next = messageRingBuffer.Next();
            var entry = messageRingBuffer[next];
            entry.Update(serverToClientMessage);
            messageRingBuffer.Publish(next);
        }

        public void EnqueueMessage(int clientId, string message)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = clientId;
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.Message;
            serverToClientMessage.Message = message;
            Enqueue();
        }

        public void EnqueueAddedLimitOrder(LimitOrder limitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.OrderAccepted;
            Enqueue();

        }

        public void EnqueueUpdatedLimitOrder(LimitOrder limitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.OrderChanged;
            Enqueue();
        }

        public void EnqueueDeletedLimitOrder(LimitOrder limitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.OrderDeleted;
            Enqueue();
        }

        public void EnqueueOrderSnapshot(int clientId, List<LimitOrder> orders)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = clientId;
            serverToClientMessage.OrderList.AddRange(orders);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.OrderSnapshop;
            Enqueue();
        }
    }
}