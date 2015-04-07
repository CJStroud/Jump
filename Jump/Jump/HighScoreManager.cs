using System;
using System.Collections.Generic;
using System.IO;

namespace Jump
{
    public static class HighScoreManager
    {
        private static string _filePath;

        /// <summary>
        /// Setups the HighScoreManager by creating the neccessary files and folder
        /// </summary>
        /// <param name="filePath">The file path of the highscores text file. Defaults to: application directory / highscores / highscores.txt</param>
        public static void Initialise(string filePath = null)
        {
            // if file doesn't exist create it
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                // check directory exists
                string directory = Directory.GetCurrentDirectory() + @"\Highscores";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // check that file exists
                var file = directory + @"\highscores.txt";
                if (!File.Exists(file)) 
                {
                    File.Create(file).Dispose();
                }

                _filePath = file;
            }
            else
            {
                _filePath = filePath;
            }
        }

        /// <summary>
        /// Save the new score to the text file
        /// </summary>
        /// <param name="score">The score to be saved</param>
        public static void SaveScore(int score)
        {
            // get the current high score
            List<int> scores = GetScores();

            // work out how many scores we want to write back, the max is 5
            int timesToLoop = scores.Count <= 5 ? scores.Count : 5;
            
            // Add score to current scores and sort the list.
            scores.Add(score);
            scores.Sort();
            scores.Reverse();

            // write back the scores, if there is more than 5 then ignore them
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                for (int i = 0; i <= timesToLoop; i++)
                {
                    writer.WriteLine(scores[i]);
                }
            }
        }

        /// <summary>
        /// Returns the scores that are stored in the highscore text file
        /// </summary>
        /// <returns></returns>
        public static List<int> GetScores()
        {
            // read everything in the file and convert to integers
            string[] scores = File.ReadAllLines(_filePath);
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
