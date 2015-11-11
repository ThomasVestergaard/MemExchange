namespace MemExchange.Server.Processor.Book.Orders
{
    public class DuoLimitOrder : IDuoLimitOrder
    {
        public ILimitOrder LimitOrder1 { get; private set; }
        public ILimitOrder LimitOrder2 { get; private set; }

        public DuoLimitOrder(ILimitOrder limitOrder1, ILimitOrder limitOrder2)
        {
            LimitOrder1 = limitOrder1;
            LimitOrder2 = limitOrder2;

        }

    }
}