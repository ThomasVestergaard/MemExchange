using System;
using System.Collections.Generic;
using System.Linq;
using Disruptor;
using MemExchange.Server.Common;

namespace MemExchange.Server.Incoming.Logging
{
    public class PerformanceRecorderDirectConsoleOutput : IPerformanceRecorder
    {
        private readonly IDateService dateService;
        private int outputMetricsPerCount { get; set; }
        private RingBuffer<ClientToServerMessageQueueItem> ringBuffer { get; set; }

        private List<double> durationMeasurements { get; set; }
        private int countSinceLastOutput;

        public PerformanceRecorderDirectConsoleOutput(IDateService dateService)
        {
            this.dateService = dateService;
            durationMeasurements = new List<double>();
            countSinceLastOutput = 0;
        }

        private int GetAvailableRingbufferPercentage()
        {
            var tenPercent = ringBuffer.BufferSize / 10;

            for (int i = 1; i < 10; i++)
            {
                if (!ringBuffer.HasAvailableCapacity(tenPercent * i))
                    return 10 * i;
            }

            return 100;
        }

        public void Setup(RingBuffer<ClientToServerMessageQueueItem> ringBuffer, int outputMetricsPerCount)
        {
            this.ringBuffer = ringBuffer;
            this.outputMetricsPerCount = outputMetricsPerCount;
        }

        private void OutputMetrics()
        {
            //Console.Clear();
            double mean = durationMeasurements.Average();
            Console.WriteLine("---------");
            Console.WriteLine("Messages processed: {0}", countSinceLastOutput);
            Console.WriteLine("Average process time: {0} ms", mean.ToString("N5"));
            Console.WriteLine("Message per sec: {0}", (1000d / mean).ToString("N5"));
            Console.WriteLine("Available input buffer: {0}", GetAvailableRingbufferPercentage());
        }

        public void OnNext(ClientToServerMessageQueueItem data, long sequence, bool endOfBatch)
        {
            durationMeasurements.Add((dateService.UtcNow() - data.StartProcessTime).TotalMilliseconds);
            countSinceLastOutput ++;

            if (countSinceLastOutput >= outputMetricsPerCount)
            {
                OutputMetrics();
                countSinceLastOutput = 0;
                durationMeasurements.Clear();
            }
        }
    }
}