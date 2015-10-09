using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Components.DictionaryAdapter;
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
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
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

            LimitOrder addedOrder;
            orderKeep.AddLimitOrder(order, out addedOrder);

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
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
            var staticClient2 = new MemExchange.Server.Clients.Client { ClientId = 2 };
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

            LimitOrder added1;
            LimitOrder added2;
            LimitOrder added3;
            orderKeep.AddLimitOrder(client1_order1, out added1);
            orderKeep.AddLimitOrder(client1_order2, out added2);
            orderKeep.AddLimitOrder(client2_order1, out added3);

            Assert.IsTrue(orderKeep.ClientLimitOrders.ContainsKey(staticClient1));
            Assert.IsTrue(orderKeep.ClientLimitOrders.ContainsKey(staticClient2));
            Assert.AreEqual(2, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient2].Count);
        }

        [Test]
        public void ShouldAddOrderAndReturnFullOrderObject()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
            var staticClient2 = new MemExchange.Server.Clients.Client { ClientId = 2 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(2))).Return(staticClient2);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order1 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            LimitOrder addedOrder;
            orderKeep.AddLimitOrder(order1, out addedOrder);
            Assert.IsNotNull(addedOrder);
            Assert.AreEqual(1, addedOrder.ClientId);
            Assert.AreEqual(90, addedOrder.Price);
            Assert.AreEqual(10, addedOrder.Quantity);
            Assert.AreEqual("ABC", addedOrder.Symbol);
            Assert.AreEqual(WayEnum.Buy, addedOrder.Way);
            Assert.IsTrue(addedOrder.ExchangeOrderId > 0);
        }

        [Test]
        public void ShouldModifyOrderAndReturnFullOrderObject()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
            var staticClient2 = new MemExchange.Server.Clients.Client { ClientId = 2 };
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(1))).Return(staticClient1);
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(Arg<int>.Is.Equal(2))).Return(staticClient2);

            var orderKeep = new OrderKeep(clientRepositoryMock);
            var order1 = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            LimitOrder addedOrder;
            orderKeep.AddLimitOrder(order1, out addedOrder);

            var modify = new LimitOrder();
            modify.ExchangeOrderId = addedOrder.ExchangeOrderId;
            modify.Price = 80;
            modify.Quantity = 20;
            modify.ClientId = 1;

            LimitOrder modified;
            var modifyResult = orderKeep.TryUpdateLimitOrder(modify, out modified);
            Assert.IsNotNull(modified);
            Assert.AreEqual(addedOrder.ExchangeOrderId, modified.ExchangeOrderId);
            Assert.AreEqual(1, modified.ClientId);
            Assert.AreEqual(80, modified.Price);
            Assert.AreEqual(20, modified.Quantity);
            Assert.AreEqual("ABC", modified.Symbol);
            Assert.AreEqual(WayEnum.Buy, modified.Way);
            
        }
        
        [Test]
        public void AddLimitOrderShouldIncrementSequenceNumber()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
            var staticClient2 = new MemExchange.Server.Clients.Client { ClientId = 2 };
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

            LimitOrder add1;
            LimitOrder add2;
            LimitOrder add3;

            orderKeep.AddLimitOrder(client1_order1, out add1);
            orderKeep.AddLimitOrder(client1_order2, out add2);
            orderKeep.AddLimitOrder(client2_order1, out add3);

            Assert.AreEqual(1, add1.ExchangeOrderId);
            Assert.AreEqual(2, add2.ExchangeOrderId);
            Assert.AreEqual(3, add3.ExchangeOrderId);

        }

        [Test]
        public void UpdateLimitOrderShouldReturnNullWhenOrderIsNotFound()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
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

            LimitOrder modifiedOrder;
            var updateResult = orderKeep.TryUpdateLimitOrder(order, out modifiedOrder);
            Assert.IsNull(modifiedOrder);
            Assert.IsFalse(updateResult);
        }

        [Test]
        public void UpdateLimitOrderShouldReturnOrderWhenOrderUpdated()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
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

            LimitOrder addedOrder;
            orderKeep.AddLimitOrder(order, out addedOrder);
            
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(90, orderKeep.ClientLimitOrders[staticClient1].Values.First().Price);
            Assert.AreEqual(10, orderKeep.ClientLimitOrders[staticClient1].Values.First().Quantity);
            
            addedOrder.Quantity = 99;
            addedOrder.Price = 50.22d;

            LimitOrder modifiedOrder;
            var updateResult = orderKeep.TryUpdateLimitOrder(addedOrder, out modifiedOrder);
            Assert.IsNotNull(modifiedOrder);
            Assert.IsTrue(updateResult);
            Assert.AreEqual(1, orderKeep.ClientLimitOrders[staticClient1].Count);
            Assert.AreEqual(50.22d, orderKeep.ClientLimitOrders[staticClient1].Values.First().Price);
            Assert.AreEqual(99, orderKeep.ClientLimitOrders[staticClient1].Values.First().Quantity);
        }

        [Test]
        public void DeleteOrderShouldReturnFalseIfOrderIsNotFound()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
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
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
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

            LimitOrder addResult;
            orderKeep.AddLimitOrder(order, out addResult);

            var deleteResult = orderKeep.DeleteLimitOrder(addResult);
            Assert.IsTrue(deleteResult);
            Assert.AreEqual(0, orderKeep.ClientLimitOrders[staticClient1].Count);
        }

        [Test]
        public void OrderAddPerformanceMeasureTest()
        {
            var clientStub = new MemExchange.Server.Clients.Client {ClientId = 1};
            clientRepositoryMock.Stub(a => a.GetOrAddClientFromId(1)).Return(clientStub);
            var orderKeep = new OrderKeep(clientRepositoryMock);
            
            var order = new LimitOrder
            {
                ClientId = 1,
                Price = 90,
                Quantity = 10,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            var sw = new Stopwatch();
            sw.Start();

            LimitOrder addedOrder;
            LimitOrder modifiedOrder;
            for (int i = 0; i < 50000; i++)
            {
                
                orderKeep.AddLimitOrder(order, out addedOrder);

                addedOrder.Quantity = 20;
                addedOrder.Price = 100;
                
                orderKeep.TryUpdateLimitOrder(addedOrder, out modifiedOrder);
                orderKeep.DeleteLimitOrder(addedOrder);

            }

            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            double perItem = (double)elapsed / (double)50000;
            Console.WriteLine("First 50k:");
            Console.WriteLine("Total ms: " + elapsed);
            Console.WriteLine("Per item: " + perItem);
            Console.WriteLine("");

            sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 50000; i++)
            {
                orderKeep.AddLimitOrder(order, out addedOrder);
            }

            sw.Stop();
            elapsed = sw.ElapsedMilliseconds;

            perItem = (double)elapsed / (double)50000;
            Console.WriteLine("Second 50k:");
            Console.WriteLine("Total ms: " + elapsed);
            Console.WriteLine("Per item: " + perItem);
            Console.WriteLine("");
        }

        [Test]
        public void ShouldSetCurrectClientOrderCount()
        {
            var staticClient1 = new MemExchange.Server.Clients.Client { ClientId = 1 };
            var staticClient2 = new MemExchange.Server.Clients.Client { ClientId = 2 };
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

            LimitOrder added1;
            LimitOrder added2;
            LimitOrder added3;
            orderKeep.AddLimitOrder(client1_order1, out added1);
            orderKeep.AddLimitOrder(client1_order2, out added2);
            orderKeep.AddLimitOrder(client2_order1, out added3);

            var client1Orders = new List<LimitOrder>();
            var client2Orders = new List<LimitOrder>();

            orderKeep.GetClientOrders(1, out client1Orders);
            orderKeep.GetClientOrders(2, out client2Orders);

            Assert.AreEqual(2, client1Orders.Count);
            Assert.AreEqual(1, client2Orders.Count);
        }

    }
}
