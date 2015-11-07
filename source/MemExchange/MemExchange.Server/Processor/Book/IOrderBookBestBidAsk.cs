using System;
using MemExchange.Core.SharedDto.Level1;

namespace MemExchange.Server.Processor.Book
{
    public interface IOrderBookBestBidAsk
    {
        string Symbol { get; }
        double? BestBidPrice { get; }
        double? BestAskPrice { get; }
        int BestBidQuantity { get; }
        int BestAskQuantity { get; }

        void RegisterUpdateHandler(Action<IOrderBookBestBidAsk> handler);
        void UnRegisterUpdateHandler(Action<IOrderBookBestBidAsk> handler);
        bool Set(double? bestBid, double? bestAsk, int bestBidQuantity, int bestAskQuantity);
        MarketBestBidAskDto ToDto();
    }
}
