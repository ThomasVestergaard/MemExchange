using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Executions
{
    public class Execution : IExecution
    {
        public LimitOrder BuySideOrder { get; private set; }
        public LimitOrder SellSideOrder { get; private set; }
        public int MatchedQuantity { get; private set; }
        public double MatchedPrice { get; private set; }

        public Execution(LimitOrder buySideOrder, LimitOrder sellSideOrder, int matchedQuantity, double matchedPrice)
        {
            BuySideOrder = buySideOrder;
            SellSideOrder = sellSideOrder;
            MatchedPrice = matchedPrice;
            MatchedQuantity = matchedQuantity;

        }
    }
}