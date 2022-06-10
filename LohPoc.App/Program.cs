namespace LohPoc.App
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using LohPoc.Lib;

    public static class Program
    {
        private static CancellationTokenSource Cts = new CancellationTokenSource();
        private static EventWaitHandle ShutdownSignal = new ManualResetEvent(false);
        private static Random Random = new Random();

        public static async Task Main()
        {
            var shutdownToken = SetupShutdown();

            const int minArraySize = 500000;
            const int maxArraySize = 1000000;

            while (!shutdownToken.IsCancellationRequested)
            {
                GC.Collect();

                var arraySize = Random.Next(minArraySize, maxArraySize + 1);

                Console.WriteLine($"Allocating array of {arraySize} elements.");

                using var temp = new RentedArray<SomeReferenceType>(arraySize);

                //var temp = new SomeReferenceType[arraySize];

                //var temp = new ShardedArray<SomeReferenceType>(arraySize);

                await DelayOrCancel(3000, shutdownToken);
            }

            ShutdownSignal.Set();
        }

        private static CancellationToken SetupShutdown()
        {
            Console.CancelKeyPress += (_, __) =>
            {
                Console.WriteLine("Received shutdown signal.");
                Cts.Cancel();
                ShutdownSignal.WaitOne(1000);
                Console.WriteLine("Shutdown complete.");
            };
            
            Console.WriteLine("Press Ctrl+C to terminate.");

            return Cts.Token;
        }

        private static async Task DelayOrCancel(int millisecondsDelay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Ignore task cancellation
            }
        }
    }
}
