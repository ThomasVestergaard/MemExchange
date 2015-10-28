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
        private readonly IMatchingAlgorithm matchingAlgorithm;
        private readonly IOrderBookBestBidAsk orderBookBestBidAsk;
        private readonly IOutgoingQueue outgoingQueue;
        public string Symbol { get; private set; }

        public Dictionary<double, IPriceSlot> PriceSlots { get; private set; }

        public OrderBook(string symbol, IMatchingAlgorithm matchingAlgorithm, IOrderBookBestBidAsk orderBookBestBidAsk, IOutgoingQueue outgoingQueue)
        {
            this.matchingAlgorithm = matchingAlgorithm;
            this.orderBookBestBidAsk = orderBookBestBidAsk;
            this.outgoingQueue = outgoingQueue;
            Symbol = symbol;
            PriceSlots = new Dictionary<double, IPriceSlot>();
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
            
            orderBookBestBidAsk.Set(bestBid, bestAsk, bidQuantity, askQuantity);

        }
        
        private void RemoveSlotIfEmpty(double price)
        {
            if (!PriceSlots[price].HasOrders)
                PriceSlots.Remove(price);
        }

        public void RemoveOrder(ILimitOrder order)
        {
            if (!PriceSlots.ContainsKey(order.Price))
                return;

            PriceSlots[order.Price].RemoveOrder(order);
            RemoveSlotIfEmpty(order.Price);

            order.UnRegisterDeleteNotificationHandler(RemoveOrder);
            order.UnRegisterFilledNotification(RemoveOrder);
            order.UnRegisterModifyNotificationHandler(HandleOrderModify);

            SetBestBidAndAsk();
        }

        private void MoveOrder(double oldPrice, ILimitOrder currentOrder)
        {
            if (PriceSlots.ContainsKey(oldPrice))
                PriceSlots[oldPrice].RemoveOrder(currentOrder);

            RemoveSlotIfEmpty(oldPrice);
            HandleOrder(currentOrder);
            
        }

        private void HandleOrderModify(ILimitOrder order, int oldQuantity, double oldPrice)
        {
            if (oldPrice != order.Price)
                MoveOrder(oldPrice, order);

            SetBestBidAndAsk();
        }

        private void TryMatch(ILimitOrder limitOrder)
        {
            if (limitOrder.Quantity == 0)
                return;

            switch (limitOrder.Way)
            {
                case WayEnum.Buy:
                    if (!orderBookBestBidAsk.BestAskPrice.HasValue)
                        return;

                    if (limitOrder.Price < orderBookBestBidAsk.BestAskPrice)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestAskPrice.Value].TryMatch(limitOrder);
                    
                    SetBestBidAndAsk();
                    TryMatch(limitOrder);
                    break;

                case WayEnum.Sell:
                    if (!orderBookBestBidAsk.BestBidPrice.HasValue)
                        return;

                    if (limitOrder.Price > orderBookBestBidAsk.BestBidPrice)
                        return;

                    PriceSlots[orderBookBestBidAsk.BestBidPrice.Value].TryMatch(limitOrder);

                    SetBestBidAndAsk();
                    TryMatch(limitOrder);
                    break;
            }
        }

        public void HandleOrder(ILimitOrder limitOrder)
        {
            TryMatch(limitOrder);

            if (limitOrder.Quantity == 0)
                return;

            if (!PriceSlots.ContainsKey(limitOrder.Price))
                PriceSlots.Add(limitOrder.Price, new PriceSlot(limitOrder.Price, matchingAlgorithm));

            if (PriceSlots[limitOrder.Price].ContainsOrder(limitOrder))
                return;

            limitOrder.RegisterDeleteNotificationHandler(RemoveOrder);
            limitOrder.RegisterFilledNotification(RemoveOrder);
            limitOrder.RegisterModifyNotificationHandler(HandleOrderModify);

            PriceSlots[limitOrder.Price].AddOrder(limitOrder);
            SetBestBidAndAsk();
            
        }
       
       
    }
}