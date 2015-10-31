using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Common;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public class MarketOrderMatchingAlgorithm : IMarketOrderMatchingAlgorithm
    {
        private readonly IDateService dateService;
        private List<Action<INewExecution>> executionHandlers { get; set; }

        public MarketOrderMatchingAlgorithm(IDateService dateService)
        {
            this.dateService = dateService;
            executionHandlers = new List<Action<INewExecution>>();
        }

        public void AddExecutionsHandler(Action<INewExecution> executionHandler)
        {
            executionHandlers.Add(executionHandler);
        }

        private int FindMatchQuantity(IMarketOrder marketOrder, ILimitOrder limitOrder)
        {
            return Math.Min(marketOrder.Quantity, limitOrder.Quantity);
        }

        public void TryMatch(IMarketOrder buyMarketOrder, ILimitOrder sellLimitOrder)
        {
            if (buyMarketOrder.Way != WayEnum.Buy || sellLimitOrder.Way != WayEnum.Sell)
                return;

            if (buyMarketOrder.Symbol != sellLimitOrder.Symbol)
                return;

            if (buyMarketOrder.Quantity == 0 || sellLimitOrder.Quantity == 0)
                return;

            int matchQuantity = FindMatchQuantity(buyMarketOrder, sellLimitOrder);
            if (matchQuantity == 0)
                return;

            double matchPrice = sellLimitOrder.Price;

            buyMarketOrder.Modify(buyMarketOrder.Quantity - matchQuantity);
            sellLimitOrder.Modify(sellLimitOrder.Quantity - matchQuantity);

            var execution = new NewExecution(buyMarketOrder, sellLimitOrder, matchQuantity, matchPrice, dateService.UtcNow());
            for (int i = 0; i < executionHandlers.Count; i++)
                executionHandlers[i].Invoke(execution);
        }

        public void TryMatch(ILimitOrder buyLimitOrder, IMarketOrder sellMarketOrder)
        {
            if (sellMarketOrder.Way != WayEnum.Sell || buyLimitOrder.Way != WayEnum.Buy)
                return;

            if (sellMarketOrder.Symbol != buyLimitOrder.Symbol)
                return;

            if (sellMarketOrder.Quantity == 0 || buyLimitOrder.Quantity == 0)
                return;

            int matchQuantity = FindMatchQuantity(sellMarketOrder, buyLimitOrder);
            if (matchQuantity == 0)
                return;

            double matchPrice = buyLimitOrder.Price;

            sellMarketOrder.Modify(sellMarketOrder.Quantity - matchQuantity);
            buyLimitOrder.Modify(buyLimitOrder.Quantity - matchQuantity);

            var execution = new NewExecution(buyLimitOrder, sellMarketOrder, matchQuantity, matchPrice, dateService.UtcNow());
            for (int i = 0; i < executionHandlers.Count; i++)
                executionHandlers[i].Invoke(execution);
        }
    }
}