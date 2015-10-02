using System.Collections.Generic;
using System.Threading;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ServerToClient;
using MemExchange.Server.Outgoing;
using MemExchange.Tests.Tools;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Integration
{
    [TestFixture]
    public class ServerToClientIntegrationTest
    {
        private ILogger loggerMock { get; set; }
        private IMessagePublisher serverPublihser { get; set; }
        private IServerMessageSubscriber clientSubscriber { get; set; }

        private List<IServerToClientMessage> receivedMessages { get; set; }

        [SetUp]
        public void Setup()
        {
            receivedMessages = new List<IServerToClientMessage>();

            loggerMock = MockRepository.GenerateMock<ILogger>();
            serverPublihser = new MessagePublisher(loggerMock, new ProtobufSerializer());
            clientSubscriber = new ServerMessageSubscriber(loggerMock, new ProtobufSerializer());

            var port = TcpHelper.AvailableTcpPort();
            serverPublihser.Start(port);

            Thread.Sleep(100);
            clientSubscriber.Start("localhost", port, 1, receivedMessages.Add);
            Thread.Sleep(100);
        }

        [TearDown]
        public void TearDown()
        {
            clientSubscriber.Stop();
            serverPublihser.Stop();
        }


        [Test]
        public void ClientShouldReceiveMessagesOnSubscribedClientId()
        {
            serverPublihser.Publish(1, new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted});
            serverPublihser.Publish(2, new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted });
            serverPublihser.Publish(3, new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted });
            serverPublihser.Publish(1, new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted });
            serverPublihser.Publish(1, new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted });

            Assert.That(() => receivedMessages.Count, Is.EqualTo(3).After(500,50));
        }
        
    }
}
