using System;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Server.Incoming.Logging;
using MemExchange.Server.Processor;

namespace MemExchange.Server.Incoming
{
    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        private int ringbufferSize = (int)Math.Pow(256, 2);
        private readonly ILogger logger;
        private readonly IIncomingMessageProcessor messageProcessor;
        private readonly IPerformanceRecorder performanceRecorder;
        private Disruptor<ClientToServerMessageQueueItem> messageDisrupter;
        private RingBuffer<ClientToServerMessageQueueItem> messageRingBuffer;
        
        public IncomingMessageQueue(ILogger logger, IIncomingMessageProcessor messageProcessor, IPerformanceRecorder performanceRecorder)
        {
            this.logger = logger;
            this.messageProcessor = messageProcessor;
            this.performanceRecorder = performanceRecorder;
        }

        public void Start()
        {
            messageDisrupter = new Disruptor<ClientToServerMessageQueueItem>(() => new ClientToServerMessageQueueItem(), new SingleThreadedClaimStrategy(ringbufferSize), new SleepingWaitStrategy(), TaskScheduler.Default);
            messageDisrupter.HandleEventsWith(messageProcessor).Then(performanceRecorder);
            messageRingBuffer = messageDisrupter.Start();
            logger.Info("Incoming message queue started.");
        }

        public void Stop()
        {
            messageDisrupter.Halt();
            logger.Info("Incoming message queue stopped.");
        }

        public void Enqueue(ClientToServerMessage clientToServerMessage)
        {
            var next = messageRingBuffer.Next();
            var entry = messageRingBuffer[next];
            entry.Update(clientToServerMessage);
            messageRingBuffer.Publish(next);
        }
    }
}