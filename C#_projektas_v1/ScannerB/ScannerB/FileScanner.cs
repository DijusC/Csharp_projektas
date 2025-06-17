using System;
using System.Collections.Generic;
using System.IO;

namespace ScannerB
{
    public class FileScanner
    {
        private readonly string folderPath;
        private readonly List<string> targetWords = new List<string> { "example", "home", "test" };

        public FileScanner(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public string[] GetTextFiles()
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Katalogas nerastas.");
                return Array.Empty<string>();
            }
            return Directory.GetFiles(folderPath, "*.txt");
        }

        public Dictionary<string, int> ScanFile(string filePath)
        {
            var wordCounts = new Dictionary<string, int>();
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
            return wordCounts;
        }
    }
}