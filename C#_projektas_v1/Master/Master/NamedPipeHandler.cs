using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Master
{
    public class NamedPipeHandler
    {
        public static async Task ListenToPipeAsync(string pipeName, WordIndex wordIndex)
        {
            while (true)
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
                {
                    Console.WriteLine($"Laukiama prisijungimo prie kanalo {pipeName}...");
                    await Task.Run(() => pipeServer.WaitForConnection());
                    Console.WriteLine($"Prisijungta prie {pipeName}");

                    using (var reader = new StreamReader(pipeServer))
                    {
                        while (!reader.EndOfStream)
                        {
                            string message = await reader.ReadLineAsync();
                            if (message == "END") break;

                            var parts = message.Split(':');
                            if (parts.Length == 3)
                            {
                                string fileName = parts[0];
                                string word = parts[1];
                                int count = int.Parse(parts[2]);
                                wordIndex.AddWord(fileName, word, count);
                            }
                        }
                    }
                }
                Console.WriteLine($"Kanalas {pipeName} uždarytas.");
                break;
            }
        }
    }
}