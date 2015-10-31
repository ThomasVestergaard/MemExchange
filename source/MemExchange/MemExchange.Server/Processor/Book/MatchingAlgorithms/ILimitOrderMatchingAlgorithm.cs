using System;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public interface ILimitOrderMatchingAlgorithm
    {
        void AddExecutionsHandler(Action<INewExecution> executionHandler);
        void TryMatch(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder);
    }
}
