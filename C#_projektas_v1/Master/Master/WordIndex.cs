public class WordIndex
{
    private readonly Dictionary<string, Dictionary<string, int>> fileWordCounts = new Dictionary<string, Dictionary<string, int>>();

    public void AddWord(string fileName, string word, int count)
    {
        if (!fileWordCounts.ContainsKey(fileName))
        {
            fileWordCounts[fileName] = new Dictionary<string, int>();
        }
        fileWordCounts[fileName][word] = fileWordCounts[fileName].ContainsKey(word) ? fileWordCounts[fileName][word] : count;
    }

    public void GenerateFinalReport(string outputPath)
    {
        using (var writer = new StreamWriter(outputPath))
        {
            foreach (var fileEntry in fileWordCounts.OrderBy(f => f.Key))
            {
                foreach (var wordEntry in fileEntry.Value.OrderBy(w => w.Key))
                {
                    writer.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
                }
            }
        }
    }
}