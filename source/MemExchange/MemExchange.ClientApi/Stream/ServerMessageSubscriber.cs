using System;
using System.Threading;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ServerToClient;
using NetMQ;
using NetMQ.Sockets;

namespace MemExchange.ClientApi.Stream
{
    public class ServerMessageSubscriber : IServerMessageSubscriber
    {
        private readonly ILogger logger;
        private readonly ISerializer serializer;

        private Thread receiveThread;
        private bool isRunning;

        private NetMQContext ctx;
        private SubscriberSocket subscribeSocket;

        private Action<ServerToClientMessage> messageHandler { get; set; }

        public ServerMessageSubscriber(ILogger logger, ISerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
            isRunning = false;
        }

        public void Start(string serverAddress, int serverPublishPort, int clientId, Action<ServerToClientMessage> messageHandler)
        {
            this.messageHandler = messageHandler;
            ctx = NetMQContext.Create();
            subscribeSocket = ctx.CreateSubscriberSocket();
            subscribeSocket.Connect(string.Format("tcp://{0}:{1}", serverAddress, serverPublishPort));
            subscribeSocket.Subscribe(clientId.ToString());
            subscribeSocket.Subscribe("a");
            isRunning = true;
            receiveThread = new Thread(Run);
            receiveThread.Name = "ClientMessageListenThread";
            receiveThread.Start();
            logger.Info("Server message subscriber started");
        }

        public void Stop()
        {
            isRunning = false;
            subscribeSocket.Close();
            ctx.Dispose();

            receiveThread.Join(100);
            logger.Info("Server message subscriber stopped");
        }

        private void Run()
        {
            while (isRunning)
            {
                try
                {
                    bool hasMore;
                    var clientIdString = subscribeSocket.ReceiveFrameString(out hasMore);

                    if (hasMore)
                    {
                        var receiedBuffer = subscribeSocket.ReceiveFrameBytes(out hasMore);

                        if (!hasMore && receiedBuffer != null && receiedBuffer.Length > 0)
                        {
                            var deserialized = serializer.Deserialize<ServerToClientMessage>(receiedBuffer);
                            if (deserialized != null && messageHandler != null)
                                messageHandler(deserialized);
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }

            }
        }
    }
}