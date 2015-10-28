using System;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.Executions
{
    public interface INewExecution
    {
        ILimitOrder BuySideOrder { get; }
        ILimitOrder SellSideOrder { get; }
        int MatchedQuantity { get; }
        double MatchedPrice { get; }
        DateTimeOffset ExecutionTime { get; }
    }
}