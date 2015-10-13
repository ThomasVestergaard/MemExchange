using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Executions
{
    public interface IExecution
    {
        LimitOrder BuySideOrder { get; }
        LimitOrder SellSideOrder { get; }
        int MatchedQuantity { get; }
        double MatchedPrice { get; }

        
    }
}
