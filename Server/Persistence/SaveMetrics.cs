using System;
using System.Diagnostics;

namespace Server
{
    public sealed class SaveMetrics : IDisposable
    {
        private const string PerformanceCategoryName = "ServUO";
        private const string PerformanceCategoryDesc = "Performance counters for ServUO";

        private readonly PerformanceCounter numberOfWorldSaves;

        private readonly PerformanceCounter itemsPerSecond;
        private readonly PerformanceCounter mobilesPerSecond;
        private readonly PerformanceCounter dataPerSecond;

        private readonly PerformanceCounter serializedBytesPerSecond;
        private readonly PerformanceCounter writtenBytesPerSecond;

        public SaveMetrics()
        {
            if (!PerformanceCounterCategory.Exists(PerformanceCategoryName))
            {
                CounterCreationDataCollection counters = new CounterCreationDataCollection();

                counters.Add(new CounterCreationData(
                    "Save - Count",
                    "Number of world saves.",
                    PerformanceCounterType.NumberOfItems32));

                counters.Add(new CounterCreationData(
                    "Save - Items/sec",
                    "Number of items saved per second.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    "Save - Mobiles/sec",
                    "Number of mobiles saved per second.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    "Save - Customs/sec",
                    "Number of cores saved per second.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    "Save - Serialized bytes/sec",
                    "Amount of world-save bytes serialized per second.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                counters.Add(new CounterCreationData(
                    "Save - Written bytes/sec",
                    "Amount of world-save bytes written to disk per second.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

				if (!Core.Unix)
				{
					try
					{
						PerformanceCounterCategory.Create(PerformanceCategoryName, PerformanceCategoryDesc, PerformanceCounterCategoryType.SingleInstance, counters);
					}
					catch
					{
						if (Core.Debug)
							Console.WriteLine("Metrics: Metrics enabled. Performance counters creation requires ServUO to be run as Administrator once!");
					}               
				}
				else
				{
					Utility.PushColor(ConsoleColor.Yellow);
					Console.WriteLine("WARNING: You've enabled SaveMetrics. This is currently not supported on Unix based operating systems. Please disable this option to hide this message.");
					Utility.PopColor();
				}
            }

            this.numberOfWorldSaves = new PerformanceCounter(PerformanceCategoryName, "Save - Count", false);

            this.itemsPerSecond = new PerformanceCounter(PerformanceCategoryName, "Save - Items/sec", false);
            this.mobilesPerSecond = new PerformanceCounter(PerformanceCategoryName, "Save - Mobiles/sec", false);
            this.dataPerSecond = new PerformanceCounter(PerformanceCategoryName, "Save - Customs/sec", false);

            this.serializedBytesPerSecond = new PerformanceCounter(PerformanceCategoryName, "Save - Serialized bytes/sec", false);
            this.writtenBytesPerSecond = new PerformanceCounter(PerformanceCategoryName, "Save - Written bytes/sec", false);

            // increment number of world saves
            this.numberOfWorldSaves.Increment();
        }

        public void OnItemSaved(int numberOfBytes)
        {
            this.itemsPerSecond.Increment();

            this.serializedBytesPerSecond.IncrementBy(numberOfBytes);
        }

        public void OnMobileSaved(int numberOfBytes)
        {
            this.mobilesPerSecond.Increment();

            this.serializedBytesPerSecond.IncrementBy(numberOfBytes);
        }

        public void OnGuildSaved(int numberOfBytes)
        {
            this.serializedBytesPerSecond.IncrementBy(numberOfBytes);
        }

        public void OnDataSaved(int numberOfBytes)
        {
            this.dataPerSecond.Increment();

            this.serializedBytesPerSecond.IncrementBy(numberOfBytes);
        }

        public void OnFileWritten(int numberOfBytes)
        {
            this.writtenBytesPerSecond.IncrementBy(numberOfBytes);
        }

        private bool isDisposed;

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                this.numberOfWorldSaves.Dispose();

                this.itemsPerSecond.Dispose();
                this.mobilesPerSecond.Dispose();
                this.dataPerSecond.Dispose();

                this.serializedBytesPerSecond.Dispose();
                this.writtenBytesPerSecond.Dispose();
            }
        }
    }
}
