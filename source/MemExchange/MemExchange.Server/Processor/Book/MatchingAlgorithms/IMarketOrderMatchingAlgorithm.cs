using System;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public interface IMarketOrderMatchingAlgorithm
    {
        void AddExecutionsHandler(Action<INewExecution> executionHandler);
        void TryMatch(IMarketOrder buyMarketOrder, ILimitOrder sellLimitOrder);
        void TryMatch(ILimitOrder buyLimitOrder, IMarketOrder sellMarketOrder);
    }
}
