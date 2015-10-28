using System;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public interface IMatchingAlgorithm
    {
        void AddExecutionsHandler(Action<INewExecution> executionHandler);
        void TryMatch(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder);
    }
}
