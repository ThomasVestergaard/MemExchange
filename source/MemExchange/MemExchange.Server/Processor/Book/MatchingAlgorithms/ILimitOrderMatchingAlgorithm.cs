using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book.MatchingAlgorithms
{
    public interface ILimitOrderMatchingAlgorithm : IMatchingAlgorithm
    {
        void TryMatch(ILimitOrder buyLimitOrder, ILimitOrder sellLimitOrder);
    }
}
