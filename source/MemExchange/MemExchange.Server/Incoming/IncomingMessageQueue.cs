using System;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Server.Processor;

namespace MemExchange.Server.Incoming
{
    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        private int ringbufferSize = (int)Math.Pow(256, 2);
        private readonly ILogger logger;
        private readonly IIncomingMessageProcessor messageProcessor;
        private Disruptor<IClientToServerMessage> messageDisrupter;
        private RingBuffer<IClientToServerMessage> messageRingBuffer;
        
        public IncomingMessageQueue(ILogger logger, IIncomingMessageProcessor messageProcessor)
        {
            this.logger = logger;
            this.messageProcessor = messageProcessor;
        }

        public void Start()
        {
            messageDisrupter = new Disruptor<IClientToServerMessage>(() => new ClientToServerMessage(), new SingleThreadedClaimStrategy(ringbufferSize), new SleepingWaitStrategy(), TaskScheduler.Default);
            messageDisrupter.HandleEventsWith(messageProcessor);
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