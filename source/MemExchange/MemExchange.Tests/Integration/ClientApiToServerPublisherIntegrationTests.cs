using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemExchange.ClientApi;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;
using MemExchange.Server.Outgoing;
using MemExchange.Tests.Tools;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Integration
{
    [TestFixture]
    public class ClientApiToServerPublisherIntegrationTests
    {
        private ILogger loggerMock;

        // Client
        private IMessageConnection clientMessageConnectionMock;
        private IServerMessageSubscriber clientMessageSubscriber;
        private IClient client;

        // Server
        private IMessagePublisher messagePublisher;


        [SetUp]
        public void Setup()
        {
            var publishPort = TcpHelper.AvailableTcpPort();
            loggerMock = MockRepository.GenerateMock<ILogger>();

            // Server
            messagePublisher = new MessagePublisher(loggerMock, new ProtobufSerializer());
            messagePublisher.Start(publishPort);

            // Client
            clientMessageConnectionMock = MockRepository.GenerateMock<IMessageConnection>();
            clientMessageSubscriber = new ServerMessageSubscriber(loggerMock, new ProtobufSerializer());
            client= new ClientApi.Client(clientMessageConnectionMock, clientMessageSubscriber);
            client.Start(88, "localhost", 9090, publishPort);

            Thread.Sleep(100);
        }

        [TearDown]
        public void Teardown()
        {
            client.Stop();
            messagePublisher.Stop();
        }

        [Test]
        public void ClientShouldRaiseLimitOrderAddedEvent()
        {
            var addedLimitOrders = new List<LimitOrderDto>();
            client.LimitOrderAccepted += (sender, order) => addedLimitOrders.Add(order);

            var newLimitOrder = new LimitOrderDto
            {
                ClientId = 88,
                Price = 10,
                ExchangeOrderId = 123,
                Quantity = 90,
                Symbol = "TSLA",
                Way = WayEnum.Buy
            };

            messagePublisher.Publish(88, new ServerToClientMessage
            {
                LimitOrder = newLimitOrder,
                MessageType = ServerToClientMessageTypeEnum.LimitOrderAccepted,
                ReceiverClientId = 88
            });

            Assert.That(() => addedLimitOrders.Count, Is.EqualTo(1).After(500, 100));
            Assert.AreEqual(newLimitOrder.ClientId, addedLimitOrders[0].ClientId);
            Assert.AreEqual(newLimitOrder.Price, addedLimitOrders[0].Price);
            Assert.AreEqual(newLimitOrder.ExchangeOrderId, addedLimitOrders[0].ExchangeOrderId);
            Assert.AreEqual(newLimitOrder.Quantity, addedLimitOrders[0].Quantity);
            Assert.AreEqual(newLimitOrder.Symbol, addedLimitOrders[0].Symbol);
            Assert.AreEqual(newLimitOrder.Way, addedLimitOrders[0].Way);
        }

        [Test]
        public void ClientShouldRaiseLimitOrderModifiedEvent()
        {
            var modifiedLimitOrders = new List<LimitOrderDto>();
            client.LimitOrderChanged += (sender, order) => modifiedLimitOrders.Add(order);

            var newLimitOrder = new LimitOrderDto
            {
                ClientId = 88,
                Price = 10,
                ExchangeOrderId = 123,
                Quantity = 90,
                Symbol = "TSLA",
                Way = WayEnum.Buy
            };

            messagePublisher.Publish(88, new ServerToClientMessage
            {
                LimitOrder = newLimitOrder,
                MessageType = ServerToClientMessageTypeEnum.LimitOrderChanged,
                ReceiverClientId = 88
            });

            Assert.That(() => modifiedLimitOrders.Count, Is.EqualTo(1).After(500, 100));
        }

        [Test]
        public void ClientShouldRaiseLimitOrderDeletedEvent()
        {
            var deletedLimitOrders = new List<LimitOrderDto>();
            client.LimitOrderDeleted += (sender, order) => deletedLimitOrders.Add(order);

            var newLimitOrder = new LimitOrderDto
            {
                ClientId = 88,
                Price = 10,
                ExchangeOrderId = 123,
                Quantity = 90,
                Symbol = "TSLA",
                Way = WayEnum.Buy
            };

            messagePublisher.Publish(88, new ServerToClientMessage
            {
                LimitOrder = newLimitOrder,
                MessageType = ServerToClientMessageTypeEnum.LimitOrderDeleted,
                ReceiverClientId = 88
            });

            Assert.That(() => deletedLimitOrders.Count, Is.EqualTo(1).After(500, 100));
        }

    }
}
