namespace MemExchange.Server.Processor.Book.Orders
{
    public interface IDuoLimitOrder
    {
        ILimitOrder LimitOrder1 { get; }
        ILimitOrder LimitOrder2 { get; }
    }
}
