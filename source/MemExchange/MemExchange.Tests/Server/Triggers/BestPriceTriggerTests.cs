using MemExchange.Core.SharedDto;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Triggers;
using NUnit.Framework;

namespace MemExchange.Tests.Server.Triggers
{
    [TestFixture]
    public class BestPriceTriggerTests
    {
        [Test]
        public void ShouldTriggerBuySideOnSameBidPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Buy);
            trigger.SetTriggerAction(() => triggered = true);
            
            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(10.4d, null, 1,1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void ShouldTriggerBuySideOnHigherBidPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Buy);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(12.4d, null, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }
      
        [Test]
        public void ShouldNotTriggerBuySideOnLowerBidPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Buy);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(9.4d, null, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsFalse(triggerResult);
            Assert.IsFalse(triggered);
        }

        [Test]
        public void ShouldTriggerBuySideOnSameAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Sell);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(null, 10.4d, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void ShouldTriggerBuySideOnLowerAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Sell);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(null, 9.4d, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void ShouldNotTriggerBuySideOnHigherAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 10.4d, WayEnum.Sell);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(null, 19.4d, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsFalse(triggerResult);
            Assert.IsFalse(triggered);
        }

        [Test]
        public void ShouldNotTriggerBuySideOnLowerBidAndAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 52d, WayEnum.Buy);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(50, 51, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsFalse(triggerResult);
            Assert.IsFalse(triggered);
        }

        [Test]
        public void ShouldTriggerBuySideOnEqualBidHigherAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 50d, WayEnum.Buy);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(50, 51, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void ShouldNotTriggerSellSideOnHigherBidAndAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 40, WayEnum.Sell);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(50, 51, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsFalse(triggerResult);
            Assert.IsFalse(triggered);
        }

        [Test]
        public void ShouldTriggerSellSideOnHigherBidAndEqualAskPrice()
        {
            bool triggered = false;

            var trigger = new BestPriceTrigger("abc", 51, WayEnum.Sell);
            trigger.SetTriggerAction(() => triggered = true);

            var bb = new OrderBookBestBidAsk("abc");
            bb.Set(50, 51, 1, 1);

            var triggerResult = trigger.TryExecute(bb);

            Assert.IsTrue(triggerResult);
            Assert.IsTrue(triggered);
        }

    }
}
