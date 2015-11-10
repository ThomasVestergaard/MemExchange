using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemExchange.Client.UI.Setup;
using MemExchange.ClientApi;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto;

namespace MemExchange.PerformanceTestClient
{
    class Program
    {
        private static DependencyInjection dependencyInjection;
        private static Configuration config;
        static Random random = new Random();

        public static char GetLetter()
        {
            int num = random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }
       

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

            string symbol = string.Format("{0}{1}{2}", GetLetter(), GetLetter(), GetLetter());
            var client = DependencyInjection.Container.Resolve<IClient>();
            client.Start(config.ClientId, config.ServerAddress, config.ServerCommandPort, config.ServerPublishPort);
            client.LimitOrderAccepted += (sender, dto) =>
            {
                while (true)
                {
                    for (int c = 0; c < prices.Count; c++)
                    {
                        client.ModifyLimitOrder(dto.ExchangeOrderId, prices[c], 100);
                        Thread.Sleep(0);
                    }
                }
            };

            client.SubmitLimitOrder(symbol, 90, 90, WayEnum.Buy);

            Console.WriteLine("Sending orders on symbol '{0}' ...", symbol);
            Console.ReadKey();
            client.Stop();
        }


      
    }
}
