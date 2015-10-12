using MemExchange.Core.Logging;
using MemExchange.Server.Common;
using MemExchange.Server.Incoming;
using MemExchange.Server.Outgoing;

namespace MemExchange.Server
{
    public class Application
    {
        private DependencyInjection dependencyInjection;
        private ILogger logger;

        public void Start()
        {
            dependencyInjection = new DependencyInjection();
            dependencyInjection.Initialize();
            logger = dependencyInjection.Container.Resolve<ILogger>();

            dependencyInjection.Container.Resolve<IMessagePublisher>().Start(9193);
            dependencyInjection.Container.Resolve<IOutgoingQueue>().Start();

            dependencyInjection.Container.Resolve<IIncomingMessageQueue>().Start();
            dependencyInjection.Container.Resolve<IClientMessagePuller>().Start(9192);

            logger.Info("Service started");
        }

        public void Stop()
        {
            logger.Info("Service stopping");

            dependencyInjection.Container.Resolve<IClientMessagePuller>().Stop();
            dependencyInjection.Container.Resolve<IIncomingMessageQueue>().Stop();
            dependencyInjection.Container.Resolve<IOutgoingQueue>().Stop();
            dependencyInjection.Container.Resolve<IMessagePublisher>().Stop();

            dependencyInjection.Container.Dispose();
        }
    }
}
