using System;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.Executions
{
    public class NewExecution : INewExecution
    {
        public ILimitOrder BuySideOrder { get; private set; }
        public ILimitOrder SellSideOrder { get; private set; }
        public int MatchedQuantity { get; private set; }
        public double MatchedPrice { get; private set; }
        public DateTimeOffset ExecutionTime { get; private set; }

        public NewExecution(ILimitOrder buySideOrder, ILimitOrder sellSideOrder, int matchedQuantity, double matchedPrice, DateTimeOffset executionTime)
        {
            BuySideOrder = buySideOrder;
            SellSideOrder = sellSideOrder;
            MatchedPrice = matchedPrice;
            MatchedQuantity = matchedQuantity;
            ExecutionTime = executionTime;
        }
    }
}
