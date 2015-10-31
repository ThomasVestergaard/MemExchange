using System;
using System.Collections.Generic;
using MemExchange.Server.Common;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public class LimitOrderMatchingAlgorithm : ILimitOrderMatchingAlgorithm
    {
        private readonly IDateService dateService;
        private List<Action<INewExecution>> executionHandlers { get; set; }
        private double priceDifference;

        public LimitOrderMatchingAlgorithm(IDateService dateService)
        {
            this.dateService = dateService;
            executionHandlers = new List<Action<INewExecution>>();
            priceDifference = 0;
        }

        public void AddExecutionsHandler(Action<INewExecution> executionsHandler)
        {
            executionHandlers.Add(executionsHandler);
        }

        public void TryMatch(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder)
        {
            /*if (buyLimitOrder.ClientId == sellLimitOrder.ClientId)
                return;*/

            if (buyLimitOrder.Symbol != sellLimitOrder.Symbol)
                return;

            if (buyLimitOrder.Price < sellLimitOrder.Price)
                return;
            
            if (buyLimitOrder.Quantity == 0 || sellLimitOrder.Quantity == 0)
                return;

            double matchPrice = FindMatchPrice(buyLimitOrder, sellLimitOrder);

            int matchQuantity = FindMatchQuantity(buyLimitOrder, sellLimitOrder);
            if (matchQuantity == 0)
                return;

            buyLimitOrder.Modify(buyLimitOrder.Quantity - matchQuantity);
            sellLimitOrder.Modify(sellLimitOrder.Quantity - matchQuantity);

            var execution = new NewExecution(buyLimitOrder, sellLimitOrder, matchQuantity, matchPrice, dateService.UtcNow());
            for (int i = 0; i < executionHandlers.Count; i++)
                executionHandlers[i].Invoke(execution);
        }

        private int FindMatchQuantity(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder)
        {
            return Math.Min(buyLimitOrder.Quantity, sellLimitOrder.Quantity);
        }

        private double FindMatchPrice(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder)
        {
            if (buyLimitOrder.Price.Equals(sellLimitOrder.Price))
                return buyLimitOrder.Price;

            priceDifference = buyLimitOrder.Price - sellLimitOrder.Price;
            return Math.Min(buyLimitOrder.Price, sellLimitOrder.Price) + (priceDifference / 2);
        }

        
    }
}