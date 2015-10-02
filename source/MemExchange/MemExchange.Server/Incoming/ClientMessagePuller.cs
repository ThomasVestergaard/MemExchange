using System;
using System.Threading;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ClientToServer;
using NetMQ;
using NetMQ.Sockets;

namespace MemExchange.Server.Incoming
{
    public class ClientMessagePuller : IClientMessagePuller
    {
        private readonly ILogger logger;
        private readonly ISerializer serializer;
        private readonly IIncomingMessageQueue incomingMessageQueue;
        private NetMQContext ctx;
        private PullSocket responseSocket;

        private Thread listenThread;
        private bool isRunning;

        public ClientMessagePuller(ILogger logger, ISerializer serializer, IIncomingMessageQueue incomingMessageQueue)
        {
            this.logger = logger;
            this.serializer = serializer;
            this.incomingMessageQueue = incomingMessageQueue;
        }

        public void Start(int listenPort)
        {
            ctx = NetMQContext.Create();
            responseSocket = ctx.CreatePullSocket();
            responseSocket.Bind("tcp://*:" + listenPort);

            isRunning = true;
            listenThread = new Thread(ListenForMessages);
            listenThread.Name = "ClientMessageListenThread";
            listenThread.Start();

            logger.Info("Client message puller has started listening on port " + listenPort);
        }

        private void ListenForMessages()
        {
            while (isRunning)
            {
                try
                {
                    bool hasMore;
                    var receiedBuffer = responseSocket.Receive(out hasMore);

                    if (!hasMore && receiedBuffer != null && receiedBuffer.Length > 0)
                    {
                        var deserialized = serializer.Deserialize<ClientToServerMessage>(receiedBuffer);
                        if (deserialized != null)
                            incomingMessageQueue.Enqueue(deserialized);
                    }
                }
                catch(Exception ex)
                {
                    int derp = 0;
                }
            }
        }

        public void Stop()
        {
            isRunning = false;
            responseSocket.Close();
            ctx.Dispose();

            listenThread.Join(100);
            logger.Info("Message puller has stopped");
        }
    }
}