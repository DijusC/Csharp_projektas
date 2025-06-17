using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Master
{
    public class NamedPipeHandler
    {
        private readonly WordIndex wordIndex;

        public NamedPipeHandler(WordIndex wordIndex)
        {
            this.wordIndex = wordIndex;
        }

        public async Task ListenToPipeAsync(string pipeName)
        {
            try
            {
                var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);
                Console.WriteLine($"Laukiama prisijungimo prie kanalo {pipeName}...");
                await pipeServer.WaitForConnectionAsync();
                Console.WriteLine($"Prisijungta prie {pipeName}");

                using (var reader = new StreamReader(pipeServer))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"Gauta eilutė iš {pipeName}: {line}");
                        if (line == "END")
                        {
                            Console.WriteLine($"Gautas END signalas iš {pipeName}");
                            Program.AgentCompleted();
                            break;
                        }
                        var parts = line.Split(':');
                        if (parts.Length == 3)
                        {
                            string fileName = parts[0];
                            string word = parts[1];
                            if (int.TryParse(parts[2], out int count))
                            {
                                Console.WriteLine($"Apdorojama: {fileName}, {word}, {count}");
                                wordIndex.AddWord(fileName, word, count);
                            }
                            else
                            {
                                Console.WriteLine($"KLAIDA: Negalima konvertuoti skaičiaus: {parts[2]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"KLAIDA: Neteisingas eilutės formatas: {line}");
                        }
                    }
                }
                pipeServer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA klausantis kanalo {pipeName}: {ex.Message}");
            }
        }
    }
}