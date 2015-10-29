using System.Collections.Generic;
using MemExchange.Core.Logging;
using MemExchange.Server.Common;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor
{
    public class OrderDispatcher : IOrderDispatcher
    {
        private readonly IOutgoingQueue outgoingQueue;
        private readonly ILogger logger;
        private readonly IDateService dateService;
        public Dictionary<string, IOrderBook> OrderBooks { get; private set; }

        public OrderDispatcher(IOutgoingQueue outgoingQueue, ILogger logger, IDateService dateService)
        {
            this.outgoingQueue = outgoingQueue;
            this.logger = logger;
            this.dateService = dateService;
            OrderBooks = new Dictionary<string, IOrderBook>();
        }

        public void HandleAddOrder(ILimitOrder limitOrder)
        {
            string symbol = limitOrder.Symbol;
            if (!OrderBooks.ContainsKey(symbol))
            {
                var bookMatchingAlgo = new LimitOrderMatchingAlgorithm(dateService);
                bookMatchingAlgo.AddExecutionsHandler(outgoingQueue.EnqueueClientExecution);

                var level1 = new OrderBookBestBidAsk(symbol);
                level1.RegisterUpdateHandler(outgoingQueue.EnqueueLevel1Update);
                
                OrderBooks.Add(symbol, new OrderBook(symbol, bookMatchingAlgo, level1, outgoingQueue));
            }

            outgoingQueue.EnqueueAddedLimitOrder(limitOrder);
            limitOrder.RegisterDeleteNotificationHandler(OrderBooks[symbol].RemoveLimitOrder);
            limitOrder.RegisterFilledNotification(OrderBooks[symbol].RemoveLimitOrder);
            limitOrder.RegisterModifyNotificationHandler(OrderBooks[symbol].HandleOrderModify);
            
            OrderBooks[symbol].HandleLimitOrder(limitOrder);

        }

      
    }
}