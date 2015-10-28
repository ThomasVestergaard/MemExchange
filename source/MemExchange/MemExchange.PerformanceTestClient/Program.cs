using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemExchange.Client.UI.Setup;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;

namespace MemExchange.PerformanceTestClient
{
    class Program
    {
        private static DependencyInjection dependencyInjection;
        private static Configuration config;

        static void Main(string[] args)
        {
            config = new Configuration
            {
                ClientId = 99,
                ServerAddress = "localhost",
                ServerCommandPort = 9192,
                ServerPublishPort = 9193
            };

            var dependencyInjection = new DependencyInjection();
            dependencyInjection.Initialize(config);

            var prices = new List<double>();
            for (double x = 0; x < 90; x += 1)
            {
                double val = Math.Round(Math.Sin(x / Math.PI), 4) + 2;
                prices.Add(val);
            }

            var client = DependencyInjection.Container.Resolve<IClient>();
            client.Start(config.ClientId, config.ServerAddress, config.ServerCommandPort, config.ServerPublishPort);
            client.LimitOrderAccepted += (sender, dto) =>
            {
                while (true)
                {
                    for (int c = 0; c < prices.Count; c++)
                    {
                        client.ModifyLimitOrder(dto.ExchangeOrderId, prices[c], 100);
                        Thread.Sleep(1);
                    }
                }
            };

            client.SubmitLimitOrder("qqq", 90, 90, WayEnum.Buy);

            Console.WriteLine("Sending orders ...");
            Console.ReadKey();
            client.Stop();
        }
    }
}
