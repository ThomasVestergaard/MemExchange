using System.Collections.Generic;
using MemExchange.Core.Logging;
using MemExchange.Server.Common;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor
{
    public class OrderDispatcher : IOrderDispatcher
    {
        private readonly IOutgoingQueue outgoingQueue;
        private readonly ILogger logger;
        private readonly IDateService dateService;
        private readonly IOrderRepository orderRepository;
        public Dictionary<string, IOrderBook> OrderBooks { get; private set; }

        public OrderDispatcher(IOutgoingQueue outgoingQueue, ILogger logger, IDateService dateService, IOrderRepository orderRepository)
        {
            this.outgoingQueue = outgoingQueue;
            this.logger = logger;
            this.dateService = dateService;
            this.orderRepository = orderRepository;
            OrderBooks = new Dictionary<string, IOrderBook>();
        }

        public void HandleMarketOrder(IMarketOrder marketOrder)
        {
            string symbol = marketOrder.Symbol;
            if (!OrderBooks.ContainsKey(symbol))
                return;

            OrderBooks[symbol].HandleMarketOrder(marketOrder);
        }

        public void HandleAddStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            string symbol = stopLimitOrder.Symbol;
            if (!OrderBooks.ContainsKey(symbol))
                return;
            
            stopLimitOrder.RegisterOutgoingQueueDeleteHandler(outgoingQueue.EnqueueDeletedStopLimitOrder);
            outgoingQueue.EnqueueAddedStopLimitOrder(stopLimitOrder);
            stopLimitOrder.Trigger.SetTriggerAction(() =>
            {
                stopLimitOrder.Delete();
                var newLimitOrder = orderRepository.NewLimitOrder(stopLimitOrder);
                newLimitOrder.RegisterDeleteNotificationHandler(outgoingQueue.EnqueueDeletedLimitOrder);
                newLimitOrder.RegisterModifyNotificationHandler(outgoingQueue.EnqueueUpdatedLimitOrder);
                newLimitOrder.RegisterFilledNotification(outgoingQueue.EnqueueDeletedLimitOrder);
                newLimitOrder.RegisterFilledNotification((order) => order.Delete());
                HandleAddLimitOrder(newLimitOrder);
            });

            OrderBooks[symbol].AddStopLimitOrder(stopLimitOrder);
        }

        public void HandleAddLimitOrder(ILimitOrder limitOrder)
        {
            string symbol = limitOrder.Symbol;
            if (!OrderBooks.ContainsKey(symbol))
            {
                var bookMatchingLimitAlgo = new LimitOrderMatchingAlgorithm(dateService);
                bookMatchingLimitAlgo.AddExecutionsHandler(outgoingQueue.EnqueueClientExecution);

                var bookMatchingMarketAlgo = new MarketOrderMatchingAlgorithm(dateService);
                bookMatchingMarketAlgo.AddExecutionsHandler(outgoingQueue.EnqueueClientExecution);

                var level1 = new OrderBookBestBidAsk(symbol);
                level1.RegisterUpdateHandler(outgoingQueue.EnqueueLevel1Update);

                var book = new OrderBook(symbol, bookMatchingLimitAlgo, bookMatchingMarketAlgo, level1);
                OrderBooks.Add(symbol, book);
            }

            outgoingQueue.EnqueueAddedLimitOrder(limitOrder);
            limitOrder.RegisterDeleteNotificationHandler(OrderBooks[symbol].RemoveLimitOrder);
            limitOrder.RegisterFilledNotification(OrderBooks[symbol].RemoveLimitOrder);
            limitOrder.RegisterModifyNotificationHandler(OrderBooks[symbol].HandleLimitOrderModify);
            
            OrderBooks[symbol].AddLimitOrder(limitOrder);
        }

        public void HandDuoLimitOrderUpdate(ILimitOrder limitOrder1, double limitOrder1NewPrice, int limitOrder1NewQuantity, ILimitOrder limitOrder2, double limitOrder2NewPrice, int limitOrder2NewQuantity)
        {
            if (!OrderBooks.ContainsKey(limitOrder1.Symbol))
                return;

            OrderBooks[limitOrder1.Symbol].SetSuspendLimitOrderMatchingStatus(true);

            limitOrder1.Modify(limitOrder1NewQuantity, limitOrder1NewPrice);
            limitOrder2.Modify(limitOrder2NewQuantity, limitOrder2NewPrice);

            OrderBooks[limitOrder1.Symbol].SetSuspendLimitOrderMatchingStatus(false);
            OrderBooks[limitOrder1.Symbol].TryMatchLimitOrder(limitOrder1);
            OrderBooks[limitOrder1.Symbol].TryMatchLimitOrder(limitOrder2);
        }
    }
}