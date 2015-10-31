using System;
using System.Collections.Generic;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Level1;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.ClientApi
{
    public class Client : IClient
    {
        public event EventHandler<LimitOrderDto> LimitOrderAccepted;
        public event EventHandler<LimitOrderDto> LimitOrderChanged;
        public event EventHandler<LimitOrderDto> LimitOrderDeleted;
        public event EventHandler<List<LimitOrderDto>> LimitOrderSnapshot;
        public event EventHandler<ExecutionDto> NewExecution;
        public event EventHandler<MarketBestBidAskDto> Level1Updated;

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
                    EventHandler<LimitOrderDto> acceptedHandler = LimitOrderAccepted;
                    if (acceptedHandler != null)
                        acceptedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderDeleted:
                    EventHandler<LimitOrderDto> deletedHandler = LimitOrderDeleted;
                    if (deletedHandler != null)
                        deletedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderChanged:
                    EventHandler<LimitOrderDto> changedHandler = LimitOrderChanged;
                    if (changedHandler != null)
                        changedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.OrderSnapshot:
                    EventHandler<List<LimitOrderDto>> snapshotHandler = LimitOrderSnapshot;
                    if (snapshotHandler != null)
                        snapshotHandler(this, message.OrderList);
                    break;

                case ServerToClientMessageTypeEnum.Execution:
                    EventHandler<ExecutionDto> executionHandler = NewExecution;
                    if (executionHandler != null)
                        executionHandler(this, message.Execution);
                    break;

                case ServerToClientMessageTypeEnum.Level1:
                    EventHandler<MarketBestBidAskDto> level1Handler = Level1Updated;
                    if (level1Handler != null)
                        level1Handler(this, message.Level1);
                    break;
            }
        }

        public void Stop()
        {
            isStarted = false;
            subscriber.Stop();
            messageConnection.Stop();
        }

        public void SubmitMarketOrder(string symbol, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                MarketOrder = new MarketOrderDto
                {
                    Quantity = quantity,
                    Symbol = symbol,
                    Way = way
                },
                MessageType = ClientToServerMessageTypeEnum.PlaceMarketOrder
            });
        }

        public void SubmitLimitOrder(string symbol, double price, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    Price = price,
                    Quantity = quantity,
                    Symbol = symbol,
                    Way = way
                },
                MessageType = ClientToServerMessageTypeEnum.PlaceLimitOrder
            });
        }

        public void ModifyLimitOrder(uint exchangeOrderId, double newPrice, int newQuantity)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                    Price = newPrice,
                    Quantity = newQuantity
                },
                MessageType = ClientToServerMessageTypeEnum.ModifyLimitOrder
            });
        }

        public void CancelLimitOrder(uint exchangeOrderId)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                },
                MessageType = ClientToServerMessageTypeEnum.CancelLimitOrder
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