using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Server.Processor.Book.Orders
{
    public class LimitOrder : IDisposable, ILimitOrder
    {
        private int quantity;
        private double price;
        private string symbol;
        private readonly WayEnum way;
        private readonly int clientId;
        private uint exchangeOrderId;
        private List<Action<ILimitOrder, int, double>> ModifiedHandlers { get; set; }
        private List<Action<ILimitOrder>> DeletedHandlers { get; set; }
        private List<Action<ILimitOrder>> FilledHandlers { get; set; }

        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol == value)
                    return;

                symbol = value;
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value == quantity)
                    return;

                quantity = value;
            }
        }
        public double Price
        {
            get { return price; }
            set
            {
                if (value == price)
                    return;

                price = value;
            }
        }

        public WayEnum Way
        {
            get { return way; }
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

        public LimitOrder(string symbol, int quantity, double price, WayEnum way, int clientId)
        {
            this.symbol = symbol;
            this.quantity = quantity;
            this.price = price;
            this.way = way;
            this.clientId = clientId;
            this.exchangeOrderId = 0;
            ModifiedHandlers = new List<Action<ILimitOrder, int, double>>();
            DeletedHandlers = new List<Action<ILimitOrder>>();
            FilledHandlers = new List<Action<ILimitOrder>>();
        }

        public void SetExchangeOrderId(uint exchangeOrderId)
        {
            ExchangeOrderId = exchangeOrderId;
        }

        public void Modify(int newQuantity, double newPrice)
        {
            int oldQuantity = Quantity;
            double oldPrice = Price;

            Quantity = newQuantity;
            Price = newPrice;
            RaiseModifiedEvent(oldQuantity, oldPrice);

            if (Quantity == 0)
                RaiseFilledEvent();
        }

        public void Modify(int newQuantity)
        {
            int oldQuantity = Quantity;
            double oldPrice = Price;

            Quantity = newQuantity;
            RaiseModifiedEvent(oldQuantity, oldPrice);

            if (Quantity == 0)
                RaiseFilledEvent();
        }

        public void RegisterDeleteNotificationHandler(Action<ILimitOrder> handler)
        {
                DeletedHandlers.Add(handler);
        }

        public void UnRegisterDeleteNotificationHandler(Action<ILimitOrder> handler)
        {
            //if (DeletedHandlers.Contains(handler))
                DeletedHandlers.Remove(handler);
        }

        public void RegisterModifyNotificationHandler(Action<ILimitOrder, int, double> handler)
        {
                ModifiedHandlers.Add(handler);
        }

        public void UnRegisterModifyNotificationHandler(Action<ILimitOrder, int, double> handler)
        {
            //if (ModifiedHandlers.Contains(handler))
                ModifiedHandlers.Remove(handler);
        }

        public void RegisterFilledNotification(Action<ILimitOrder> handler)
        {
                FilledHandlers.Add(handler);
        }

        public void UnRegisterFilledNotification(Action<ILimitOrder> handler)
        {
            //if (FilledHandlers.Contains(handler))
                FilledHandlers.Remove(handler);
        }

        private void RaiseFilledEvent()
        {
            for (int i = 0; i < FilledHandlers.Count; i++)
                FilledHandlers[i].Invoke(this);
        }
        private void RaiseModifiedEvent(int oldQuantity, double oldPrice)
        {
            for (int i=0; i<ModifiedHandlers.Count; i++)
                ModifiedHandlers[i].Invoke(this, oldQuantity, oldPrice);
        }
        private void RaiseDeletedEvent()
        {
            for (int i = 0; i < DeletedHandlers.Count; i++)
                DeletedHandlers[i].Invoke(this);
        }

        public void Delete()
        {
            RaiseDeletedEvent();
            ModifiedHandlers.Clear();
            DeletedHandlers.Clear();
        }

        public void Dispose()
        {
            
        }

        public LimitOrderDto ToDto()
        {
            return new LimitOrderDto
            {
                ClientId = clientId,
                ExchangeOrderId = exchangeOrderId,
                Price = price,
                Quantity = quantity,
                Symbol = symbol,
                Way = way
            };
        }


    }
}
