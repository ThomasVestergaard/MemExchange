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
            publishSocket.SendMoreFrame(clientId.ToString()).SendFrame(serialized);
        }

        public void Publish(ServerToClientMessage serverToClientMessage)
        {
            var serialized = serializer.Serialize(serverToClientMessage);
            publishSocket.SendMoreFrame("a").SendFrame(serialized);
        }

        public void OnNext(ServerToClientMessage data, long sequence, bool endOfBatch)
        {
            if (data.ReceiverClientId > 0)
                Publish(data.ReceiverClientId, data);
            else if (data.ReceiverClientId == 0)
                Publish(data);
        }
    }
}