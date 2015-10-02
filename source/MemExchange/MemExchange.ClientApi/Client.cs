using System;
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

        public void ModifyLimitOrder(long exchangeOrderId, double newPrice, int newQuantity)
        {
            if (!isStarted)
                return;
        }

        public void CancelLimitOrder(long exchangeOrderId)
        {
            if (!isStarted)
                return;
        }
    }
}