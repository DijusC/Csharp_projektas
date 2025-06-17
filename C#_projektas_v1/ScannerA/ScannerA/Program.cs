using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScannerA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Naudojimas: ScannerA.exe <katalogo_kelias> <agent_name>");
                return;
            }

            string folderPath = args[0];
            string agentName = args[1];
            int cpuCore = agentName == "A1" ? 1 : 2;

            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << cpuCore);

            Console.WriteLine($"Agentas {agentName} paleistas, skenuoja katalogą: {folderPath}");

            var scanner = new FileScanner(folderPath);
            var files = scanner.GetTextFiles();

            foreach (var file in files)
            {
                var wordCounts = await Task.Run(() => scanner.ScanFile(file));
                await NamedPipeClient.SendDataAsync(agentName == "A1" ? "agent1" : "agent2", file, wordCounts);
            }

            await NamedPipeClient.SendDataAsync(agentName == "A1" ? "agent1" : "agent2", null, null, true);
            Console.WriteLine($"Agentas {agentName} baigė darbą.");
        }
    }
}