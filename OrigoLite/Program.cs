using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoLite
{
    public class Program
    {
        private static OrigoDbLite<Dictionary<int, int>> engine;

        public static void Main(string[] args)
        {
            int numThreads = 10;
            Console.WriteLine("Num threads: " + numThreads);
            if (File.Exists("journal.dat")) File.Delete("journal.dat");
            engine = new OrigoDbLite<Dictionary<int, int>>("journal.dat");
            Console.WriteLine("Start: " + DateTime.Now);
            float readWriteRatio = 2000;
            var duration = TimeSpan.FromMinutes(0.5);
            var tasks = Enumerable.Repeat(42, numThreads)
                .Select(
                    _ => Task.Factory.StartNew(r => RandomTransactions(readWriteRatio, duration), 0)).ToArray();
            Task.WaitAll(tasks);

             int[] readsWrites = tasks.Select(t => t.Result).Aggregate(new[] {0, 0}, (sums, result) =>
                {
                    sums[0] += result[0];
                    sums[1] += result[1];
                    return sums;
                });

            double readsPerSecond = readsWrites[0]/duration.TotalSeconds;
            double writesPerSecond = readsWrites[1]/duration.TotalSeconds;

            Console.WriteLine("Reads:       {0}", readsWrites[0]);
            Console.WriteLine("Writes:      {0}", readsWrites[1]);
            Console.WriteLine("Reads/s:     {0}", readsPerSecond);
            Console.WriteLine("Writes/s:    {0}", writesPerSecond);
            Console.WriteLine("Total:       {0}", readsWrites.Sum());
            Console.WriteLine("TPS:         {0}", readsWrites.Sum() * 1.0 / duration.TotalSeconds);
            Console.WriteLine("Stop time:   {0}", DateTime.Now);
        }


        /// <summary>
        /// Execute a mix of reads and writes during a fixed period of time
        /// </summary>
        /// <param name="readWriteRatio"></param>
        /// <returns></returns>
        private static int[] RandomTransactions(float readWriteRatio, TimeSpan duration)
        {
            //normalize: 
            float readWeight = readWriteRatio / (readWriteRatio + 1);
            const int READ = 0;
            const int WRITE = 1;
            var stopWatch = new Stopwatch();
            int[] result = {0, 0};
            var rnd = new Random();
            stopWatch.Start();
            while (stopWatch.Elapsed < duration)
            {

                    if (rnd.NextDouble() < readWeight)
                    {
                        result[READ]++;
                        engine.ExecuteRead(new Get(rnd.Next()));
                    }
                    else
                    {
                        result[WRITE]++;
                        engine.ExecuteWrite(RandomWriteTransaction(rnd));
                    }
            }
            return result;
        }

        private static WriteTransaction<Dictionary<int, int>> RandomWriteTransaction(Random r)
        {
            if (r.NextDouble() < 0.5) return new Set(r.Next(), r.Next());
            return new Remove(r.Next());
        }
    }
}
