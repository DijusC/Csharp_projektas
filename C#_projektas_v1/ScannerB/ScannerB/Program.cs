using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScannerB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Naudojimas: ScannerB.exe <katalogo_kelias> <agent_name>");
                return;
            }

            string folderPath = args[0];
            string agentName = args[1];
            int cpuCore = agentName == "A2" ? 2 : 1; // ScannerB naudoja 2 branduoli

            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << cpuCore);

            Console.WriteLine($"Agentas {agentName} paleistas, skenuoja katalogą: {folderPath}");

            var scanner = new FileScanner(folderPath); // failu skenavimas
            var files = scanner.GetTextFiles();

            foreach (var file in files)
            {
                var wordCounts = await Task.Run(() => scanner.ScanFile(file));
                await NamedPipeClient.SendDataAsync("agent2", file, wordCounts);
            }

            await NamedPipeClient.SendDataAsync("agent2", null, null, true);
            Console.WriteLine($"Agentas {agentName} baigė darbą.");
        }
    }
}