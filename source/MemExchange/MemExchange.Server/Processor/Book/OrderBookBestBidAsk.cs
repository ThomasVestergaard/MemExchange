using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto.Level1;

namespace MemExchange.Server.Processor.Book
{
    public class OrderBookBestBidAsk : IOrderBookBestBidAsk
    {
        public string Symbol { get; private set; }
        public double? BestBidPrice { get; private set; }
        public double? BestAskPrice { get; private set; }
        public int BestBidQuantity { get; private set; }
        public int BestAskQuantity { get; private set; }

        private List<Action<IOrderBookBestBidAsk>> updateHandlers { get; set; }

        public OrderBookBestBidAsk(string symbol)
        {
            Symbol = symbol;
            updateHandlers = new List<Action<IOrderBookBestBidAsk>>();
            BestBidPrice = null;
            BestAskPrice = null;
            BestBidQuantity = 0;
            BestAskQuantity = 0;
        }

        public void RegisterUpdateHandler(Action<IOrderBookBestBidAsk> handler)
        {
            if (!updateHandlers.Contains(handler))
                updateHandlers.Add(handler);
        }

        public void UnRegisterUpdateHandler(Action<IOrderBookBestBidAsk> handler)
        {
            if (updateHandlers.Contains(handler))
                updateHandlers.Remove(handler);
        }

        private void NotifyHandlers()
        {
            for (int i=0; i<updateHandlers.Count; i++)
                updateHandlers[i].Invoke(this);
        }

        public bool Set(double? bestBid, double? bestAsk, int bestBidQuantity, int bestAskQuantity)
        {
            bool isUpdated = false;

            if (bestBid != BestBidPrice)
            {
                BestBidPrice = bestBid;
                isUpdated = true;
            }

            if (bestAsk != BestAskPrice)
            {
                BestAskPrice = bestAsk;
                isUpdated = true;
            }

            if (bestBidQuantity != BestBidQuantity)
            {
                BestBidQuantity = bestBidQuantity;
                isUpdated = true;
            }

            if (bestAskQuantity != BestAskQuantity)
            {
                BestAskQuantity = bestAskQuantity;
                isUpdated = true;
            }

            if (isUpdated)
                NotifyHandlers();

            return isUpdated;
        }

        public MarketBestBidAskDto ToDto()
        {
            return new MarketBestBidAskDto
            {
                Symbol = Symbol,
                BestAskPrice = BestAskPrice,
                BestBidPrice = BestBidPrice,
                BestAskQuantity = BestAskQuantity,
                BestBidQuantity = BestBidQuantity
            };
        }
    }
}