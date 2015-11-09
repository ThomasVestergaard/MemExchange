using System;
using MemExchange.Core.SharedDto;

namespace MemExchange.Server.Processor.Book.Triggers
{
    public interface IBestPriceTrigger
    {
        string Symbol { get; }
        WayEnum Way { get; }
        double TriggerPrice { get; }
        bool TryExecute(IOrderBookBestBidAsk bestBidAsk);
        void SetTriggerAction(Action action);
        void ModifyTriggerPrice(double newTriggerPrice);


    }
}
