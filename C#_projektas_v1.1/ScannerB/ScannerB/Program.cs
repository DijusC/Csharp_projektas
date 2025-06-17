using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO.Pipes;

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
            string pipeName = "agent2";
            int cpuCore = 2;

            Console.WriteLine($"Agentas {agentName} paleistas, kelias: {folderPath}, kanalas: {pipeName}");

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << cpuCore);
                    Console.WriteLine($"Nustatytas CPU afinitetas: branduolys {cpuCore}");
                }
                else
                {
                    Console.WriteLine("CPU afinitetas nepalaikomas šioje platformoje.");
                }

                var scanner = new FileScanner(folderPath);
                var files = scanner.GetTextFiles();
                Console.WriteLine($"Pradeda apdoroti {files.Length} failus");

                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    Console.WriteLine($"Bandoma prisijungti prie kanalo {pipeName}...");
                    await pipeClient.ConnectAsync(10000);
                    Console.WriteLine($"Prisijungta prie kanalo {pipeName}");
                    using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                    {
                        foreach (var file in files)
                        {
                            Console.WriteLine($"Apdorojamas failas: {file}");
                            var wordCounts = await Task.Run(() => scanner.ScanFile(file));
                            foreach (var entry in wordCounts)
                            {
                                var line = $"{Path.GetFileName(file)}:{entry.Key}:{entry.Value}";
                                Console.WriteLine($"Siunčiama eilutė per kanalą {pipeName}: {line}");
                                await writer.WriteLineAsync(line);
                            }
                        }
                        Console.WriteLine($"Siunčiamas END signalas per kanalą {pipeName}");
                        await writer.WriteLineAsync("END");
                    }
                }

                Console.WriteLine($"Agentas {agentName} baigė darbą.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA ScannerB procese: {ex.Message}");
            }
        }
    }
}