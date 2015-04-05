using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Jump
{
    public static class HighScoreManager
    {
        private static string FilePath;

        public static void Initialise()
        {
            Initialise(string.Empty);
        }

        public static void Initialise(string filePath)
        {
            string file = string.Empty;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                string directory = Directory.GetCurrentDirectory() + @"\Highscores";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                file = directory + @"\highscores.txt";
                if (!File.Exists(file)) 
                {
                    File.Create(file).Dispose();
                }

                FilePath = file;
            }
            else
            {
                FilePath = filePath;
            }
        }

        public static void SaveScore(int score)
        {
            List<int> scores = GetScores();

            int timesToLoop = scores.Count <= 5 ? scores.Count : 5;
            scores.Add(score);

            scores.Sort();
            scores.Reverse();

            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                for (int i = 0; i <= timesToLoop; i++)
                {
                    writer.WriteLine(scores[i]);
                }
            }
        }

        public static List<int> GetScores()
        {
            string[] scores = File.ReadAllLines(FilePath);
            List<int> intScores = new List<int>();
            foreach (string score in scores)
            {
                int i = Convert.ToInt32(score);
                intScores.Add(i);
            }

            return intScores;
        }
    }
}
