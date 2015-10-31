using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public class PriceSlot : IPriceSlot
    {
        private readonly ILimitOrderMatchingAlgorithm limitOrderMatchingAlgorithm;
        private readonly IMarketOrderMatchingAlgorithm marketOrderMatchingAlgorithm;
        public double Price { get; private set; }
        public List<ILimitOrder> BuyOrders { get; private set; }
        public List<ILimitOrder> SellOrders { get; private set; }
        
        public PriceSlot(double price, ILimitOrderMatchingAlgorithm limitOrderMatchingAlgorithm, IMarketOrderMatchingAlgorithm marketOrderMatchingAlgorithm)
        {
            this.limitOrderMatchingAlgorithm = limitOrderMatchingAlgorithm;
            this.marketOrderMatchingAlgorithm = marketOrderMatchingAlgorithm;
            Price = price;
            BuyOrders = new List<ILimitOrder>();
            SellOrders = new List<ILimitOrder>();
        }

        public bool ContainsOrder(ILimitOrder order)
        {
            switch (order.Way)
            {
                case WayEnum.Buy:
                    if (BuyOrders.Contains(order))
                        return true;
                    break;

                    case WayEnum.Sell:
                    if (SellOrders.Contains(order))
                        return true;
                    break;
            }
            return false;
        }

        public void TryMatchMarketOrder(IMarketOrder order)
        {
            switch (order.Way)
            {
                case WayEnum.Buy:
                    if (SellOrders.Count == 0)
                        return;

                    while (order.Quantity > 0 && SellOrders.Count > 0)
                        marketOrderMatchingAlgorithm.TryMatch(order, SellOrders[0]);

                    break;

                case WayEnum.Sell:
                    if (BuyOrders.Count == 0)
                        return;

                    while (order.Quantity > 0 && BuyOrders.Count > 0)
                        marketOrderMatchingAlgorithm.TryMatch(BuyOrders[0], order);
                    break;
            }

        }

        public void TryMatchLimitOrder(ILimitOrder order)
        {
            switch (order.Way)
            {
                case WayEnum.Buy:
                    if (SellOrders.Count == 0)
                        return;

                    while (order.Quantity > 0 && SellOrders.Count > 0)
                        limitOrderMatchingAlgorithm.TryMatch(order, SellOrders[0]);
                    break;

                case WayEnum.Sell:
                    if (BuyOrders.Count == 0)
                        return;

                    while (order.Quantity > 0 && BuyOrders.Count > 0)
                        limitOrderMatchingAlgorithm.TryMatch(BuyOrders[0], order);
                    break;
            }
        }

        public bool HasOrders
        {
            get { return BuyOrders.Count > 0 || SellOrders.Count > 0; }
        }

        public bool HasBids
        {
            get { return BuyOrders.Count > 0; }
        }

        public bool HasAsks
        {
            get { return SellOrders.Count > 0; }
        }
        private void HandleDeleteOrFilled(ILimitOrder order)
        {
            RemoveOrder(order);
        }

        public void AddOrder(ILimitOrder order)
        {
            if (order.Price != Price)
                return;

            switch (order.Way)
            {
                case WayEnum.Buy:
                    BuyOrders.Add(order);
                    break;

                case WayEnum.Sell:
                    SellOrders.Add(order);
                    break;
            }

            order.RegisterDeleteNotificationHandler(HandleDeleteOrFilled);
            order.RegisterFilledNotification(HandleDeleteOrFilled);
        }

        public void RemoveOrder(ILimitOrder order)
        {
            switch (order.Way)
            {
                case WayEnum.Buy:
                    BuyOrders.Remove(order);
                    break;

                case WayEnum.Sell:
                    SellOrders.Remove(order);
                    break;
            }

            order.UnRegisterDeleteNotificationHandler(HandleDeleteOrFilled);
            order.UnRegisterFilledNotification(HandleDeleteOrFilled);
        }
    }
}