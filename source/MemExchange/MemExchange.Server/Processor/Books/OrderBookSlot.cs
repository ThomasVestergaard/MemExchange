using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Executions;

namespace MemExchange.Server.Processor.Books
{
    public class OrderBookSlot : IOrderBookSlot
    {
        public double Price { get; private set; }
        public List<LimitOrder> Orders { get; private set; }
        public int TotalQuantity { get; private set; }
        private WayEnum SlotWay { get; set; }

        public OrderBookSlot(double price, WayEnum slotWay)
        {
            if (slotWay == WayEnum.NotSet)
                throw new InvalidOperationException("Way cannot be set to 'NotSet'.");

            Price = price;
            SlotWay = slotWay;
            TotalQuantity = 0;
            Orders = new EditableList<LimitOrder>();
        }

        public void AddOrder(LimitOrder order)
        {
            if (order.Way != SlotWay )
                return;

            if (order.Price != Price)
                return;

            Orders.Add(order);
            TotalQuantity += order.Quantity;
        }

        public void RemoveOrder(LimitOrder order)
        {
            if (order.Way != SlotWay)
                return;

            if (order.Price != Price)
                return;

            Orders.Remove(order);
            TotalQuantity -= order.Quantity;
        }

        private IExecution MatchOrder(LimitOrder order, int wantedQuantity)
        {
            if (wantedQuantity >= Orders[0].Quantity)
            {
                var matchedOrder = Orders[0];
                Orders.RemoveAt(0);
                TotalQuantity -= matchedOrder.Quantity;
                if (SlotWay == WayEnum.Sell)
                    return new Execution(order, matchedOrder, matchedOrder.Quantity, matchedOrder.Price);
                else if (SlotWay == WayEnum.Buy)
                    return new Execution(matchedOrder, order, matchedOrder.Quantity, matchedOrder.Price);
                    
            }

            Orders[0].Quantity -= wantedQuantity;
            TotalQuantity -= wantedQuantity;

            if (SlotWay == WayEnum.Sell)
                return new Execution(order, Orders[0], wantedQuantity, Orders[0].Price);
            else
                return new Execution(Orders[0], order, wantedQuantity, Orders[0].Price);
                
        }

        public List<IExecution> MatchOrder(LimitOrder order)
        {
            var toReturn = new List<IExecution>();

            if (order.Way == SlotWay)
                return toReturn;

            if (TotalQuantity == 0)
                return toReturn;

            if (order.Price != Price)
            return toReturn;

            int unmatchedQuantityFromIncomingOrder = order.Quantity;
            while (TotalQuantity > 0 && unmatchedQuantityFromIncomingOrder > 0)
            {
                var execution = MatchOrder(order, unmatchedQuantityFromIncomingOrder);
                toReturn.Add(execution);
                unmatchedQuantityFromIncomingOrder -= execution.MatchedQuantity;
            }

            return toReturn;
        }
    }
}