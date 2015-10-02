using System.Linq;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Clients;
using MemExchange.Server.Processor;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class OrderKeepTests
    {
        private IClientRepository clientRepositoryMock;

        [SetUp]
        public void Setup()
        {
            clientRepositoryMock = MockRepository.GenerateMock<IClientRepository>();
        }

        [Test]
        public void ShouldAddOrderToCollection()
        {
            var staticClient1 = new Client { ClientId = 1 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            orderKeep.AddLimitOrder(order);

            Assert.IsTrue(orderKeep.ClientLimitOrders.ContainsKey(staticClient1));
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(90, orderKeep.ClientLimitOrders[staticClient1].Values.First().Price);
            Assert.AreEqual(10, orderKeep.ClientLimitOrders[staticClient1].Values.First().Quantity);
            Assert.AreEqual("ABC", orderKeep.ClientLimitOrders[staticClient1].Values.First().Symbol);
            Assert.AreEqual(WayEnum.Buy, orderKeep.ClientLimitOrders[staticClient1].Values.First().Way);

        }

        [Test]
        public void ShouldAddOrderToIndividualClientCollection()
        {
            var staticClient1 = new Client { ClientId = 1 };
            var staticClient2 = new Client { ClientId = 2 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(2))).Return(staticClient2);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var client1_order1 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var client1_order2 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "DEF",
                Way = WayEnum.Buy
            };

            var client2_order1 = new LimitOrder
            {
                ClientId = 2,
                Price = 90,
                Quantity = 10,
                Symbol = "QQQ",
                Way = WayEnum.Buy
            };

            orderKeep.AddLimitOrder(client1_order1);
            orderKeep.AddLimitOrder(client1_order2);
            orderKeep.AddLimitOrder(client2_order1);

            Assert.IsTrue(orderKeep.ClientLimitOrders.ContainsKey(staticClient1));
            Assert.IsTrue(orderKeep.ClientLimitOrders.ContainsKey(staticClient2));
            Assert.AreEqual(2, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient2].Count);
            

        }

        [Test]
        public void AddLimitOrderShouldIncrementSequenceNumber()
        {
            var staticClient1 = new Client { ClientId = 1 };
            var staticClient2 = new Client { ClientId = 2 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(2))).Return(staticClient2);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var client1_order1 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var client1_order2 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "DEF",
                Way = WayEnum.Buy
            };

            var client2_order1 = new LimitOrder
            {
                ClientId = 2,
                Price = 90,
                Quantity = 10,
                Symbol = "QQQ",
                Way = WayEnum.Buy
            };

            var add1 = orderKeep.AddLimitOrder(client1_order1);
            var add2 = orderKeep.AddLimitOrder(client1_order2);
            var add3 = orderKeep.AddLimitOrder(client2_order1);

            Assert.AreEqual(1, add1.ExchangeOrderId);
            Assert.AreEqual(2, add2.ExchangeOrderId);
            Assert.AreEqual(3, add3.ExchangeOrderId);

        }

        [Test]
        public void UpdateLimitOrderShouldReturnNullWhenOrderIsNotFound()
        {
            var staticClient1 = new Client { ClientId = 1 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order = new LimitOrder
            {
                ClientId = 1,
                ExchangeOrderId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var updateResult = orderKeep.TryUpdateLimitOrder(order);
            Assert.IsNull(updateResult);
        }

        [Test]
        public void UpdateLimitOrderShouldReturnOrderWhenOrderUpdated()
        {
            var staticClient1 = new Client { ClientId = 1 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var addedOrder = orderKeep.AddLimitOrder(order);
            
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(90, orderKeep.ClientLimitOrders[staticClient1].Values.First().Price);
            Assert.AreEqual(10, orderKeep.ClientLimitOrders[staticClient1].Values.First().Quantity);
            
            addedOrder.Quantity = 99;
            addedOrder.Price = 50.22d;

            var updateResult = orderKeep.TryUpdateLimitOrder(order);
            Assert.IsNotNull(updateResult);
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(50.22d, orderKeep.ClientLimitOrders[staticClient1].Values.First().Price);
            Assert.AreEqual(99, orderKeep.ClientLimitOrders[staticClient1].Values.First().Quantity);
        }

        [Test]
        public void DeleteOrderShouldReturnFalseIfOrderIsNotFound()
        {
            var staticClient1 = new Client { ClientId = 1 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order = new LimitOrder
            {
                ClientId = 1,
                ExchangeOrderId = 90,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var deleteResult = orderKeep.DeleteLimitOrder(order);
            Assert.IsFalse(deleteResult);
        }


        [Test]
        public void DeleteOrderShouldReturnTrueWhenOrderIsDeleted()
        {
            var staticClient1 = new Client { ClientId = 1 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var addResult = orderKeep.AddLimitOrder(order);

            var deleteResult = orderKeep.DeleteLimitOrder(addResult);
            Assert.IsTrue(deleteResult);
            Assert.AreEqual(0, orderKeep.ClientLimitOrders[staticClient1].Count);
        }

    }
}
