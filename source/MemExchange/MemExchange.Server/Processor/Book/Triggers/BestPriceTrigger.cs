using System;
using MemExchange.Core.SharedDto;

namespace MemExchange.Server.Processor.Book.Triggers
{
    public class BestPriceTrigger : IBestPriceTrigger
    {
        public string Symbol { get; private set; }
        public WayEnum Way { get; private set; }
        public double TriggerPrice { get; private set; }
        private Action TriggerAction { get; set; }
        private bool TriggerHasFired { get; set; }

        public BestPriceTrigger(string symbol, double triggerPrice, WayEnum way)
        {
            Symbol = symbol;
            TriggerPrice = triggerPrice;
            Way = way;
        }

        private void Execute()
        {
            if (TriggerHasFired)
                return;
            
            TriggerAction();
            TriggerHasFired = true;
        }

        public bool TryExecute(IOrderBookBestBidAsk bestBidAsk)
        {
            if (TriggerAction == null)
                throw new InvalidOperationException("BestPriceTrigger cannot execute as action is null.");

            if (TriggerHasFired)
                return false;

            switch (Way)
            {
                case WayEnum.Buy:
                    
                    if ((bestBidAsk.BestBidPrice.HasValue && bestBidAsk.BestBidPrice.Value >= TriggerPrice) ||
                        (bestBidAsk.BestAskPrice.HasValue && bestBidAsk.BestAskPrice.Value >= TriggerPrice))
                    {
                        Execute();   
                        return true;
                    }

                    
                    break;

                case WayEnum.Sell:
                    
                    if ((bestBidAsk.BestAskPrice.HasValue && bestBidAsk.BestAskPrice.Value <= TriggerPrice) ||
                        (bestBidAsk.BestBidPrice.HasValue && bestBidAsk.BestBidPrice.Value <= TriggerPrice))
                    {
                        Execute();
                        return true;
                    }
                    break;
            }
            
            return false;
        }

        public void SetTriggerAction(Action action)
        {
            TriggerAction = action;
            TriggerHasFired = false;
        }

        public void ModifyTriggerPrice(double newTriggerPrice)
        {
            TriggerPrice = newTriggerPrice;
        }

    }
}