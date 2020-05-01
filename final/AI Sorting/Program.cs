using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// Goal:
// Write an AI that can save and load protocols as a simple string


namespace AI_Sorting
{
    class Program
    {
        public static int MAX_SEQUENCES = 100;
        public static int TOTAL_RUNS = 1000;
        public static int MAX_DIFFICULTY = 10;
        static void Main(string[] args)
        {
            Random rand = new Random();
            List<Sequence> sequences = new List<Sequence>();
            Console.WriteLine("Loading Sequences");
            if (!File.Exists("champions.txt"))
                File.WriteAllText("champions.txt", "");
            else
            {
                string latestChampion = File.ReadLines("champions.txt").Last();
                Sequence champion = new Sequence(latestChampion.Trim());
                Console.WriteLine("Current champion: " + champion.ToString());
                Educator.evaluate(champion.build(), 100);
                Console.ReadLine();
            }



            if (File.Exists("brains.txt"))
            {
                Console.WriteLine("brains.txt found, importing...");
                string[] seqTexts = File.ReadAllLines("brains.txt");
                for (int a = 0; a < seqTexts.Length; a++)
                    if (seqTexts[a].Length > 2)
                        sequences.Add(new Sequence(seqTexts[a].Trim()));
                // Load sequences
            }
            
            if (sequences.Count() < MAX_SEQUENCES)
            {
                Console.WriteLine("Generating fresh sequences");
                for (int a = 0; a < MAX_SEQUENCES; a++)
                    sequences.Add(new Sequence());
            }
            List<int> lastScores = new List<int>();
            for(int iteration = 1; iteration <= TOTAL_RUNS; iteration++)
            {
                Console.WriteLine("Training " + iteration.ToString() + "/" + TOTAL_RUNS.ToString());
                List<int> scores = new List<int>();
                Console.Write("Evaluating sequences:");
                for(int sequence_id = lastScores.Count(); sequence_id < sequences.Count(); sequence_id++)
                {
                    Console.Write(" " + sequence_id.ToString());
                    int score = 0;
                    for (int difficulty = 1; difficulty <= MAX_DIFFICULTY; difficulty++)
                        score += Educator.educate(sequences[sequence_id].build(), difficulty);
                    scores.Add(score);
                }
                Console.WriteLine("");
                Console.WriteLine("Score management");
                scores = lastScores.Concat(scores).ToList();

                // Using insert sort variant for speedy development time
                for (int i = 0; i < scores.Count(); i++)
                    for (int j = i+1; j < scores.Count(); j++)
                        if(scores[j] > scores[i])
                        {
                            scores[i] += scores[j];
                            scores[j] = scores[i] - scores[j];
                            scores[i] -= scores[j];
                            Sequence temp = sequences[i];
                            sequences[i] = sequences[j];
                            sequences[j] = temp;
                        }

                Console.WriteLine("Sorted score list: " + String.Join(' ', scores));
                Console.WriteLine("Best: " + sequences[0].ToString());

                string latestChampion = File.ReadLines("champions.txt").Last();
                if(latestChampion.Trim() != sequences[0].save())
                    using (StreamWriter sw = File.AppendText("champions.txt"))
                    {
                        sw.WriteLine(sequences[0].save());
                    }
                Console.WriteLine("Deleting random sequences");

                List<int> deletion = new List<int>(); 
                while (sequences.Count() - deletion.Count() > MAX_SEQUENCES / 2)
                {
                    int n = (sequences.Count()-1) - (rand.Next(0, sequences.Count()-1) * rand.Next(0, sequences.Count()-1)) / (sequences.Count()-1);
                    if (!deletion.Contains(n))
                        deletion.Add(n);
                }
                deletion.Sort();
                deletion.Reverse();
                for (int i = 0; i < deletion.Count(); i++)
                {
                    sequences.RemoveAt(deletion[i]);
                    scores.RemoveAt(deletion[i]);
                }
                lastScores = new List<int>(scores);

                Console.WriteLine("Creating babies.");
                int parents = sequences.Count();
                for (int i = 0; i < parents; i++)
                    sequences.Add(sequences[i].baby());

            }
            Console.WriteLine("Building save strings");
            string[] save = new string[sequences.Count()];
            for (int i = 0; i < sequences.Count(); i++)
                save[i] = sequences[i].save();
            Console.WriteLine("Saving brains.txt");
            File.WriteAllLines("brains.txt", save);
            Console.WriteLine("brains.txt saved");
        }
    }
}
