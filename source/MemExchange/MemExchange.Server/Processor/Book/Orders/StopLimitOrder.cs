using System;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor.Book.Orders
{
    public class StopLimitOrder : IStopLimitOrder, IDisposable
    {
        private readonly WayEnum way;
        private readonly string symbol;
        private uint exchangeOrderId;
        private readonly int clientId;
        private double triggerPrice;
        private double limitPrice;
        private int quantity;

        public WayEnum Way
        {
            get { return way; }
        }
        public string Symbol
        {
            get { return symbol; }
        }
        public uint ExchangeOrderId
        {
            get { return exchangeOrderId; }
            private set { exchangeOrderId = value; }
        }
        public int ClientId
        {
            get { return clientId; }
        }
        public double TriggerPrice
        {
            get { return triggerPrice; }
            private set { triggerPrice = value; }
        }
        public IBestPriceTrigger Trigger { get; private set; }
        public double LimitPrice
        {
            get { return limitPrice; }
            private set { limitPrice = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            private set { quantity = value; }
        }

        private Action<IStopLimitOrder> RepositoryDeleteHandler { get; set; }
        private Action<IStopLimitOrder> OrderBookDeleteHandler { get; set; }
        private Action<IStopLimitOrder> OrderBookModifyHandler { get; set; }
        private Action<IStopLimitOrder> OutgoingQueueDeleteHandler { get; set; }

        public StopLimitOrder(string symbol, int quantity, double limitPrice, double triggerPrice, WayEnum way, int clientId, IBestPriceTrigger trigger)
        {
            this.way = way;
            this.symbol = symbol;
            this.exchangeOrderId = 0;
            this.clientId = clientId;
            this.triggerPrice = triggerPrice;
            this.limitPrice = limitPrice;
            this.quantity = quantity;
            Trigger = trigger;
        }

        public void SetExchangeOrderId(uint exchangeOrderId)
        {
            ExchangeOrderId = exchangeOrderId;
        }

        public StopLimitOrderDto ToDto()
        {
            return new StopLimitOrderDto
            {
                ClientId = clientId,
                Quantity = quantity,
                ExchangeOrderId = exchangeOrderId,
                LimitPrice = limitPrice,
                TriggerPrice = triggerPrice,
                Symbol = symbol,
                Way = way
            };
        }

        public void RegisterOrderRepositoryDeleteHandler(Action<IStopLimitOrder> deleteHandler)
        {
            RepositoryDeleteHandler = deleteHandler;
        }

        public void RegisterOrderBookDeleteHandler(Action<IStopLimitOrder> deleteHandler)
        {
            OrderBookDeleteHandler = deleteHandler;
        }

        public void RegisterOrderBookModifyHandler(Action<IStopLimitOrder> modifyHandler)
        {
            OrderBookModifyHandler = modifyHandler;
        }

        public void RegisterOutgoingQueueDeleteHandler(Action<IStopLimitOrder> deleteHandler)
        {
            OutgoingQueueDeleteHandler = deleteHandler;
        }

        public void UnRegisterOrderRepositoryDeleteHandler()
        {
            RepositoryDeleteHandler = null;
        }

        public void UnRegisterOrderBookDeleteHandler()
        {
            OrderBookDeleteHandler = null;
        }

        public void UnRegisterOrderBookModifyHandler()
        {
            OrderBookModifyHandler = null;
        }

        public void UnRegisterOutgoingQueueDeleteHandler()
        {
            OutgoingQueueDeleteHandler = null;
        }

        public void Delete()
        {
            if (RepositoryDeleteHandler != null)
                RepositoryDeleteHandler(this);

            if (OrderBookDeleteHandler != null)
                OrderBookDeleteHandler(this);

            if (OutgoingQueueDeleteHandler != null)
                OutgoingQueueDeleteHandler(this);

            
            UnRegisterOrderRepositoryDeleteHandler();
            UnRegisterOrderBookDeleteHandler();
            UnRegisterOutgoingQueueDeleteHandler();
            UnRegisterOrderBookModifyHandler();
            Dispose();
        }

        public void Modify(double newTriggerPrice, double newLimitPrice, int newQuantity)
        {
            Trigger.ModifyTriggerPrice(newTriggerPrice);
            TriggerPrice = newTriggerPrice;
            LimitPrice = newLimitPrice;
            Quantity = newQuantity;

            if (OrderBookModifyHandler != null)
                OrderBookModifyHandler(this);
        }

        public void Dispose()
        {
        }
    }
}