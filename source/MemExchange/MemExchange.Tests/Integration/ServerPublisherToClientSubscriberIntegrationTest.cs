using System.Threading;
using MemExchange.ClientApi.Commands;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Server.Incoming;
using MemExchange.Tests.Tools;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Integration
{
    [TestFixture]
    public class ServerPublisherToClientSubscriberIntegrationTest
    {
        private ILogger loggerMock;
        
        // Client
        private IMessageConnection clientMessageConnection;


        // Server
        private IIncomingMessageQueue incomingMessageQueueMock;
        private IClientMessagePuller clientMessagePuller;


        [SetUp]
        public void Setup()
        {
            int connectionPort = TcpHelper.AvailableTcpPort();

            loggerMock = MockRepository.GenerateMock<ILogger>();
            
            // Server
            incomingMessageQueueMock = MockRepository.GenerateMock<IIncomingMessageQueue>();
            

            clientMessagePuller = new ClientMessagePuller(loggerMock, new ProtobufSerializer(), incomingMessageQueueMock);
            clientMessagePuller.Start(connectionPort);

            Thread.Sleep(500);

            // Client
            clientMessageConnection = new MessageConnection(loggerMock, new ProtobufSerializer());
            clientMessageConnection.Start("localhost", connectionPort);
        }

        [TearDown]
        public void TearDown()
        {
            // Client
            clientMessageConnection.Stop();
            
            // Server
            clientMessagePuller.Stop();
        }

        [Test]
        public void ServerInputQueueShouldReceiveClientMessage()
        {
            var newMessage = new ClientToServerMessage
            {
                ClientId = 90,
                MessageType = ClientToServerMessageTypeEnum.NotSet
            };

            clientMessageConnection.SendMessage(newMessage);

            Thread.Sleep(150);
            incomingMessageQueueMock.AssertWasCalled(a => a.Enqueue((Arg<ClientToServerMessage>.Matches(b => 
                b.ClientId == 90 &&
                b.MessageType == ClientToServerMessageTypeEnum.NotSet))),
                options => options.Repeat.Once());

        }

    }
}
