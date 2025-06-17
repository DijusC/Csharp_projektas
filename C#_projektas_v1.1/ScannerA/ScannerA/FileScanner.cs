using System;
using System.Collections.Generic;
using System.IO;

namespace ScannerA
{
    public class FileScanner
    {
        private readonly string folderPath;
        private readonly List<string> targetWords = new List<string> { "example", "home", "test" };

        public FileScanner(string folderPath)
        {
            this.folderPath = folderPath;
            Console.WriteLine($"FileScanner inicializuotas su keliu: {folderPath}");
        }

        public string[] GetTextFiles()
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine($"KLAIDA: Katalogas nerastas: {folderPath}");
                    return Array.Empty<string>();
                }
                var files = Directory.GetFiles(folderPath, "*.txt");
                Console.WriteLine($"Rasti failai ({folderPath}): {string.Join(", ", files)}");
                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA gaunant failus: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public Dictionary<string, int> ScanFile(string filePath)
        {
            Console.WriteLine($"Skenuojamas failas: {filePath}");
            var wordCounts = new Dictionary<string, int>();
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] words = line.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        string cleanWord = word.Trim('.', ',', ';');
                        if (targetWords.Contains(cleanWord))
                        {
                            wordCounts[cleanWord] = wordCounts.ContainsKey(cleanWord) ? wordCounts[cleanWord] + 1 : 1;
                        }
                    }
                }
                Console.WriteLine($"Skenavimo rezultatas ({filePath}): {string.Join(", ", wordCounts.Select(kv => $"{kv.Key}:{kv.Value}"))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KLAIDA skenuojant {filePath}: {ex.Message}");
            }
            return wordCounts;
        }
    }
}