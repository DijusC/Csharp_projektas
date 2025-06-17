using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Master
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0);// Nustatome CPU branduolio afinitetą (0 branduolys)

            Console.WriteLine("Master procesas paleistas, laukia agentų prisijungimų...");

            var wordIndex = new WordIndex();

            var pipe1Task = NamedPipeHandler.ListenToPipeAsync("agent1", wordIndex);
            var pipe2Task = NamedPipeHandler.ListenToPipeAsync("agent2", wordIndex);

            await Task.WhenAll(pipe1Task, pipe2Task);

            wordIndex.GenerateFinalReport("output.txt");
            Console.WriteLine("Ataskaita suformuota output.txt faile. Programa baigia darbą.");
        }
    }
}