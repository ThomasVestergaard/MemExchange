using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.ClientApi
{
    public class Client : IClient
    {
        public event EventHandler<LimitOrder> LimitOrderAccepted;
        public event EventHandler<LimitOrder> LimitOrderChanged;
        public event EventHandler<LimitOrder> LimitOrderDeleted;
        public event EventHandler<List<LimitOrder>> LimitOrderSnapshot;

        private IMessageConnection messageConnection;
        private readonly IServerMessageSubscriber subscriber;
        private int clientId ;
        private bool isStarted;

        public Client(IMessageConnection messageConnection, IServerMessageSubscriber subscriber)
        {
            this.messageConnection = messageConnection;
            this.subscriber = subscriber;
            isStarted = false;
        }

        public void Start(int clientId, string serverAddress, int serverCommandPort, int serverPublishPort)
        {
            this.clientId = clientId;
            subscriber.Start(serverAddress, serverPublishPort, clientId, HandleServerMessage);
            messageConnection.Start(serverAddress, serverCommandPort);
            isStarted = true;
        }

        private void HandleServerMessage(ServerToClientMessage message)
        {
            switch (message.MessageType)
            {
                case ServerToClientMessageTypeEnum.OrderAccepted:
                    EventHandler<LimitOrder> acceptedHandler = LimitOrderAccepted;
                    if (acceptedHandler != null)
                        acceptedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderDeleted:
                    EventHandler<LimitOrder> deletedHandler = LimitOrderDeleted;
                    if (deletedHandler != null)
                        deletedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderChanged:
                    EventHandler<LimitOrder> changedHandler = LimitOrderChanged;
                    if (changedHandler != null)
                        changedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderSnapshop:
                    EventHandler<List<LimitOrder>> snapshotHandler = LimitOrderSnapshot;
                    if (snapshotHandler != null)
                        snapshotHandler(this, message.OrderList);
                    break;
            }
        }

        public void Stop()
        {
            isStarted = false;
            subscriber.Stop();
            messageConnection.Stop();
        }

        public void SubmitLimitOrder(string symbol, double price, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrder
                {
                    ClientId = clientId,
                    Price = price,
                    Quantity = quantity,
                    Symbol = symbol,
                    Way = WayEnum.Buy
                },
                MessageType = ClientToServerMessageTypeEnum.PlaceOrder
            });
        }

        public void ModifyLimitOrder(uint exchangeOrderId, double newPrice, int newQuantity)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrder
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                    Price = newPrice,
                    Quantity = newQuantity
                },
                MessageType = ClientToServerMessageTypeEnum.ModifyOrder
            });
        }

        public void CancelLimitOrder(uint exchangeOrderId)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrder
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                },
                MessageType = ClientToServerMessageTypeEnum.CancelOrder
            });
        }

        public void RequestOpenLimitOrders()
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                MessageType = ClientToServerMessageTypeEnum.RequestOpenOrders
            });
        }
    }
}