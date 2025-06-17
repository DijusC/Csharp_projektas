using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Master
{
    public class WordIndex
    {
        private readonly Dictionary<string, Dictionary<string, int>> fileWordCounts = new Dictionary<string, Dictionary<string, int>>();

        public void AddWord(string fileName, string word, int count)
        {
            try
            {
                if (!fileWordCounts.ContainsKey(fileName))
                {
                    fileWordCounts[fileName] = new Dictionary<string, int>();
                }
                fileWordCounts[fileName][word] = fileWordCounts[fileName].ContainsKey(word) ? fileWordCounts[fileName][word] : count; // Uztikrinu, kad zodis nebus dubliuojamas
                Console.WriteLine($"Pridėtas žodis: {fileName}, {word}, {count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA pridedant žodį {word} failui {fileName}: {ex.Message}");
            }
        }

        public void GenerateFinalReport(string outputPath)
        {
            try
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
                Console.WriteLine($"Ataskaita įrašyta į: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA generuojant ataskaitą: {ex.Message}");
            }
        }
    }
}