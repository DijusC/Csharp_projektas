using System;
using System.Threading;
using System.Threading.Tasks;

namespace Master
{
    class Program
    {
        private static int agentsCompleted = 0;
        private static readonly object lockObject = new object();

        public static void AgentCompleted()
        {
            lock (lockObject)
            {
                agentsCompleted++;
                Console.WriteLine($"Agentas baigė darbą. Baigusių agentų skaičius: {agentsCompleted}");
            }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Master procesas paleistas, laukia agentų prisijungimų...");
            try
            {
                var wordIndex = new WordIndex();
                var handler = new NamedPipeHandler(wordIndex);

                var agent1Task = handler.ListenToPipeAsync("agent1");
                var agent2Task = handler.ListenToPipeAsync("agent2");

                while (agentsCompleted < 2)
                {
                    await Task.Delay(100);
                }

                string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");
                wordIndex.GenerateFinalReport(outputPath);
                Console.WriteLine($"Ataskaita suformuota: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA Master procese: {ex.Message}");
            }
        }
    }
}