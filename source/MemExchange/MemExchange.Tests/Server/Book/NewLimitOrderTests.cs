using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Processor.Book.Orders;
using NUnit.Framework;

namespace MemExchange.Tests.Server.Book
{
    [TestFixture]
    public class NewLimitOrderTests
    {

        [Test]
        public void ShouldInstantiateWithValues()
        {
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);

            Assert.AreEqual("ABC", newOrder.Symbol);
            Assert.AreEqual(10, newOrder.Quantity);
            Assert.AreEqual(99.22d, newOrder.Price);
            Assert.AreEqual(WayEnum.Buy, newOrder.Way);
            Assert.AreEqual(3, newOrder.ClientId);
        }

        [Test]
        public void ShouldRegisterModifyEventHandlerAndReceiveModifyEvent()
        {
            
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);
            newOrder.RegisterModifyNotificationHandler((order, oldQuantity, oldPrice) =>
            {
                Assert.AreEqual(newOrder, order);
                Assert.AreEqual(11, order.Quantity);
                Assert.AreEqual(88.44d, order.Price);

                Assert.AreEqual(10, oldQuantity);
                Assert.AreEqual(99.22d, oldPrice);
            });

            newOrder.Modify(11, 88.44d);
        }

        [Test]
        public void ShouldRegisterModifyEventHandlerAndUnregister()
        {
            var receivedModifedOrders = new List<ILimitOrder>();
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);

            Action<ILimitOrder, int, double> eventHandler;
            eventHandler = (order, oldQuantity, oldPrice) =>
            {
                receivedModifedOrders.Add(order);
            };
            
            newOrder.RegisterModifyNotificationHandler(eventHandler);

            newOrder.Modify(11, 88.44d);
            newOrder.UnRegisterModifyNotificationHandler(eventHandler);
            newOrder.Modify(13, 12.44d);
            Assert.AreEqual(1, receivedModifedOrders.Count);
        }

        [Test]
        public void ShouldRegisterDeleteHandlerAndRaiseDeleteEvent()
        {
            var receivedDeletedOrders = new List<ILimitOrder>();
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);
            newOrder.RegisterDeleteNotificationHandler(receivedDeletedOrders.Add);
            newOrder.Delete();
            Assert.AreEqual(1, receivedDeletedOrders.Count);
        }

        [Test]
        public void ShouldUnregisterAllHandlersOnDelete()
        {
            var receivedDeletedOrders = new List<ILimitOrder>();
            var receivedModifyOrders = new List<ILimitOrder>();
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);

            Action<ILimitOrder, int, double> modifyEventHandler;
            modifyEventHandler = (order, oldQuantity, oldPrice) =>
            {
                receivedModifyOrders.Add(order);
            };


            newOrder.RegisterDeleteNotificationHandler(receivedDeletedOrders.Add);
            newOrder.RegisterModifyNotificationHandler(modifyEventHandler);
            newOrder.Delete();
            newOrder.Modify(90,20);
            newOrder.Delete();
            Assert.AreEqual(1, receivedDeletedOrders.Count);
            Assert.AreEqual(0, receivedModifyOrders.Count);
        }

        [Test]
        public void ShouldRegisterFilledHandlerAndReceiveFilledEvent()
        {
            var filledOrders = new List<ILimitOrder>();
            var newOrder = new LimitOrder("ABC", 10, 99.22d, WayEnum.Buy, 3);
            newOrder.RegisterFilledNotification(filledOrders.Add);

            newOrder.Modify(0, 88.44d);

            Assert.AreEqual(1, filledOrders.Count);
            Assert.AreEqual(newOrder, filledOrders[0]);
        }
    }
}
