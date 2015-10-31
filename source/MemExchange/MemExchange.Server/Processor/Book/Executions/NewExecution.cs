using System;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.Executions
{
    public class NewExecution : INewExecution
    {
        public IOrder BuySideOrder { get; private set; }
        public IOrder SellSideOrder { get; private set; }
        public int MatchedQuantity { get; private set; }
        public double MatchedPrice { get; private set; }
        public DateTimeOffset ExecutionTime { get; private set; }

        public NewExecution(IOrder buySideOrder, IOrder sellSideOrder, int matchedQuantity, double matchedPrice, DateTimeOffset executionTime)
        {
            BuySideOrder = buySideOrder;
            SellSideOrder = sellSideOrder;
            MatchedPrice = matchedPrice;
            MatchedQuantity = matchedQuantity;
            ExecutionTime = executionTime;
        }
    }
}
