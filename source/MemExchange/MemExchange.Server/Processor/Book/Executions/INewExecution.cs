using System;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.Executions
{
    public interface INewExecution
    {
        IOrder BuySideOrder { get; }
        IOrder SellSideOrder { get; }
        int MatchedQuantity { get; }
        double MatchedPrice { get; }
        DateTimeOffset ExecutionTime { get; }
    }
}