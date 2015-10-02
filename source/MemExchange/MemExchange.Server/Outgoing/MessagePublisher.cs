using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ServerToClient;
using NetMQ;
using NetMQ.Sockets;

namespace MemExchange.Server.Outgoing
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger logger;
        private readonly ISerializer serializer;

        private NetMQContext ctx;
        private PublisherSocket publishSocket;

        public MessagePublisher(ILogger logger, ISerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
        }

        public void Start(int publishPort)
        {
            ctx = NetMQContext.Create();
            publishSocket = ctx.CreatePublisherSocket();
            publishSocket.Bind("tcp://*:" + publishPort);
            
            logger.Info("Message publisher started on port " + publishPort);
        }

        public void Stop()
        {
            publishSocket.Close();
            logger.Info("Message publisher stopped");
        }

        public void Publish(int clientId, ServerToClientMessage serverToClientMessage)
        {
            var serialized = serializer.Serialize(serverToClientMessage);
            publishSocket.SendMore(clientId.ToString()).Send(serialized);
        }

        public void OnNext(ServerToClientMessage data, long sequence, bool endOfBatch)
        {
            Publish(data.ReceiverClientId, data);
        }
    }
}