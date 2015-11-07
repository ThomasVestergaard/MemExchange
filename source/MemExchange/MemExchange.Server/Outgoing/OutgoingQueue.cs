using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

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

        public void EnqueueAddedLimitOrder(ILimitOrder limitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder.ToDto());
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.LimitOrderAccepted;
            Enqueue();

        }

        public void EnqueueUpdatedLimitOrder(ILimitOrder limitOrder, int oldQuantity, double oldPrice)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder.ToDto());
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.LimitOrderChanged;
            Enqueue();
        }

        public void EnqueueDeletedLimitOrder(ILimitOrder limitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = limitOrder.ClientId;
            serverToClientMessage.LimitOrder.Update(limitOrder.ToDto());
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.LimitOrderDeleted;
            Enqueue();
        }

        public void EnqueueLimitOrderSnapshot(int clientId, List<ILimitOrder> orders)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = clientId;
            serverToClientMessage.LimitOrderList.AddRange(orders.Select(a => a.ToDto()));
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.LimitOrderSnapshot;
            Enqueue();
        }

        public void EnqueueLevel1Update(IOrderBookBestBidAsk orderBookBestBidAsk)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.Level1;
            serverToClientMessage.ReceiverClientId = 0;
            serverToClientMessage.Level1.Update(orderBookBestBidAsk.ToDto());
            Enqueue();
        }

        public void EnqueueAddedStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = stopLimitOrder.ClientId;
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.StopLimitOrderAccepted;
            serverToClientMessage.StopLimitOrder.Update(stopLimitOrder.ToDto());
            Enqueue();
        }

        public void EnqueueUpdatedStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = stopLimitOrder.ClientId;
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.StopLimitOrderChanged;
            serverToClientMessage.StopLimitOrder.Update(stopLimitOrder.ToDto());
            Enqueue();
        }

        public void EnqueueDeletedStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = stopLimitOrder.ClientId;
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.StopLimitOrderDeleted;
            serverToClientMessage.StopLimitOrder.Update(stopLimitOrder.ToDto());
            Enqueue();
        }

        public void EnqueueClientExecution(INewExecution execution)
        {
            var buySideExecution = new ExecutionDto
            {
                ExchangeOrderId = execution.BuySideOrder.ExchangeOrderId,
                Quantity = execution.MatchedQuantity,
                Price = execution.MatchedPrice,
                Symbol = execution.BuySideOrder.Symbol,
                ExecutionTime = execution.ExecutionTime,
                Way = execution.BuySideOrder.Way
            };
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = execution.BuySideOrder.ClientId;
            serverToClientMessage.Execution.Update(buySideExecution);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.Execution;
            Enqueue();

            var sellSideExecution = new ExecutionDto
            {
                ExchangeOrderId = execution.SellSideOrder.ExchangeOrderId,
                Quantity = execution.MatchedQuantity,
                Price = execution.MatchedPrice,
                Symbol = execution.SellSideOrder.Symbol,
                ExecutionTime = execution.ExecutionTime,
                Way = execution.SellSideOrder.Way
            };
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = execution.SellSideOrder.ClientId;
            serverToClientMessage.Execution.Update(sellSideExecution);
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.Execution;
            Enqueue();
        }

        public void EnqueueStopLimitOrderSnapshot(int clientId, List<IStopLimitOrder> orders)
        {
            serverToClientMessage.Reset();
            serverToClientMessage.ReceiverClientId = clientId;
            serverToClientMessage.StopLimitOrderList.AddRange(orders.Select(a => a.ToDto()));
            serverToClientMessage.MessageType = ServerToClientMessageTypeEnum.StopLimitOrderSnapshot;
            Enqueue();
        }
    }
}