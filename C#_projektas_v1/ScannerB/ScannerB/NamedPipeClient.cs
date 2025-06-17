using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ScannerB
{
    public class NamedPipeClient
    {
        public static async Task SendDataAsync(string pipeName, string fileName, Dictionary<string, int> wordCounts, bool isEnd = false)
        {
            using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                await pipeClient.ConnectAsync();
                using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                {
                    if (isEnd)
                    {
                        await writer.WriteLineAsync("END");
                    }
                    else
                    {
                        foreach (var entry in wordCounts)
                        {
                            await writer.WriteLineAsync($"{Path.GetFileName(fileName)}:{entry.Key}:{entry.Value}");
                        }
                    }
                }
            }
        }
    }
}