using System.Collections.Generic;
using System.Linq;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public class OrderBook : IOrderBook
    {
        private readonly ILimitOrderMatchingAlgorithm limitOrderMatchingAlgorithm;
        private readonly IMarketOrderMatchingAlgorithm marketOrderMatchingAlgorithm;
        private readonly IOrderBookBestBidAsk orderBookBestBidAsk;
        public string Symbol { get; private set; }
        public List<IStopLimitOrder> BuySideStopLimitOrders { get; private set; }
        public List<IStopLimitOrder> SellSideStopLimitOrders { get; private set; }
        private bool LimitOrderMatchingIsSuspended { get; set; }

        public Dictionary<double, IPriceSlot> PriceSlots { get; private set; }

        public OrderBook(string symbol, ILimitOrderMatchingAlgorithm limitOrderMatchingAlgorithm, IMarketOrderMatchingAlgorithm marketOrderMatchingAlgorithm, IOrderBookBestBidAsk orderBookBestBidAsk)
        {
            this.limitOrderMatchingAlgorithm = limitOrderMatchingAlgorithm;
            this.marketOrderMatchingAlgorithm = marketOrderMatchingAlgorithm;
            this.orderBookBestBidAsk = orderBookBestBidAsk;
            Symbol = symbol;
            PriceSlots = new Dictionary<double, IPriceSlot>();
            BuySideStopLimitOrders = new List<IStopLimitOrder>();
            SellSideStopLimitOrders = new List<IStopLimitOrder>();
        }

        private void SetBestBidAndAsk()
        {
            // TODO: This is not very efficient. Should be optimized at some point.

            double? bestBid = null;
            double? bestAsk = null;
            int bidQuantity = 0;
            int askQuantity = 0;
            
            var allBuySlots = PriceSlots.Values.ToList().FindAll(a => a.HasBids).OrderByDescending(b => b.Price);
            var allSellSlots = PriceSlots.Values.ToList().FindAll(a => a.HasAsks).OrderBy(b => b.Price);

            if (allBuySlots.Any())
            {
                var bid = allBuySlots.First();
                bestBid = bid.Price;
                bidQuantity = bid.BuyOrders.Sum(a => a.Quantity);
            }

            if (allSellSlots.Any())
            {
                var ask = allSellSlots.First();
                bestAsk = ask.Price;
                askQuantity = ask.SellOrders.Sum(a => a.Quantity);
            }
            
            if (orderBookBestBidAsk.Set(bestBid, bestAsk, bidQuantity, askQuantity))
                TryExecuteTriggers();
        }

        private void TryExecuteTriggers()
        {
            for (int i = BuySideStopLimitOrders.Count - 1; i >= 0; i--)
                BuySideStopLimitOrders[i].Trigger.TryExecute(orderBookBestBidAsk);

            for (int i = SellSideStopLimitOrders.Count - 1; i >= 0; i--)
                SellSideStopLimitOrders[i].Trigger.TryExecute(orderBookBestBidAsk);
        }
        
        private void RemoveSlotIfEmpty(double price)
        {
            if (!PriceSlots[price].HasOrders)
                PriceSlots.Remove(price);
        }

        public void RemoveLimitOrder(ILimitOrder order)
        {
            if (!PriceSlots.ContainsKey(order.Price))
                return;

            PriceSlots[order.Price].RemoveOrder(order);
            RemoveSlotIfEmpty(order.Price);

            order.UnRegisterDeleteNotificationHandler(RemoveLimitOrder);
            order.UnRegisterFilledNotification(RemoveLimitOrder);
            order.UnRegisterModifyNotificationHandler(HandleLimitOrderModify);
            
            SetBestBidAndAsk();
        }

        private void MoveOrder(double oldPrice, ILimitOrder currentOrder)
        {
            if (PriceSlots.ContainsKey(oldPrice))
                PriceSlots[oldPrice].RemoveOrder(currentOrder);

            RemoveSlotIfEmpty(oldPrice);
            AddLimitOrder(currentOrder);
        }

        public void HandleLimitOrderModify(ILimitOrder order, int oldQuantity, double oldPrice)
        {
            if (oldPrice != order.Price)
                MoveOrder(oldPrice, order);

            SetBestBidAndAsk();
        }

        public void SetSuspendLimitOrderMatchingStatus(bool isSuspended)
        {
            LimitOrderMatchingIsSuspended = isSuspended;
        }

        public void TryMatchLimitOrder(ILimitOrder limitOrder)
        {
            if (LimitOrderMatchingIsSuspended)
                return;

            if (limitOrder.Quantity == 0)
                return;

            switch (limitOrder.Way)
            {
                case WayEnum.Buy:
                    if (!orderBookBestBidAsk.BestAskPrice.HasValue)
                        return;

                    if (limitOrder.Price < orderBookBestBidAsk.BestAskPrice)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestAskPrice.Value].TryMatchLimitOrder(limitOrder);
                    
                    SetBestBidAndAsk();
                    TryMatchLimitOrder(limitOrder);
                    break;

                case WayEnum.Sell:
                    if (!orderBookBestBidAsk.BestBidPrice.HasValue)
                        return;

                    if (limitOrder.Price > orderBookBestBidAsk.BestBidPrice)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestBidPrice.Value].TryMatchLimitOrder(limitOrder);

                    SetBestBidAndAsk();
                    TryMatchLimitOrder(limitOrder);
                    break;
            }
        }

        private void TryMatchMarketOrder(IMarketOrder marketOrder)
        {
            if (marketOrder.Quantity == 0)
                return;

            switch (marketOrder.Way)
            {
                case WayEnum.Buy:
                    if (!orderBookBestBidAsk.BestAskPrice.HasValue)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestAskPrice.Value].TryMatchMarketOrder(marketOrder);

                    SetBestBidAndAsk();
                    TryMatchMarketOrder(marketOrder);

                    break;

                case WayEnum.Sell:
                    if (!orderBookBestBidAsk.BestBidPrice.HasValue)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestBidPrice.Value].TryMatchMarketOrder(marketOrder);

                    SetBestBidAndAsk();
                    TryMatchMarketOrder(marketOrder);
                    break;
            }
        }

        public void AddLimitOrder(ILimitOrder limitOrder)
        {
            TryMatchLimitOrder(limitOrder);

            if (limitOrder.Quantity == 0)
                return;

            if (!PriceSlots.ContainsKey(limitOrder.Price))
                PriceSlots.Add(limitOrder.Price, new PriceSlot(limitOrder.Price, limitOrderMatchingAlgorithm, marketOrderMatchingAlgorithm));

            if (PriceSlots[limitOrder.Price].ContainsOrder(limitOrder))
                return;
            
            PriceSlots[limitOrder.Price].AddOrder(limitOrder);
            SetBestBidAndAsk();
        }

        public void HandleMarketOrder(IMarketOrder marketOrder)
        {
            TryMatchMarketOrder(marketOrder);
            SetBestBidAndAsk();
        }

        public void AddStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            switch (stopLimitOrder.Way)
            {
                case WayEnum.Buy:
                    BuySideStopLimitOrders.Insert(0, stopLimitOrder);
                    break;

                case WayEnum.Sell:
                    SellSideStopLimitOrders.Insert(0, stopLimitOrder);
                    break;
            }

            stopLimitOrder.RegisterOrderBookDeleteHandler(RemoveStopLimitOrder);
            stopLimitOrder.RegisterOrderBookModifyHandler(StopLimitOrderModified);
            TryExecuteTriggers();
        }

        public void RemoveStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            switch (stopLimitOrder.Way)
            {
                case WayEnum.Buy:
                    BuySideStopLimitOrders.Remove(stopLimitOrder);
                    break;

                case WayEnum.Sell:
                    SellSideStopLimitOrders.Remove(stopLimitOrder);
                    break;
            }
        }

        private void StopLimitOrderModified(IStopLimitOrder modifiedStopLimitOrder)
        {
            TryExecuteTriggers();
        }

    }
}