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
        public static int MAX_SEQUENCES = 1000;
        public static int TOTAL_RUNS = 1000;
        public static int MAX_DIFFICULTY = 4;
        public static int COMPLETE_RANDOMS_PER_ROUND = 1;

        static Sequence train()
        {
            Sequence t = new Sequence();
            Console.WriteLine(t.save());
            t = Educator.train(t, 5, 1000000);
            Console.WriteLine(t.save());

            Console.WriteLine("Visual brain: ");
            Console.WriteLine(t.build().ToString());

            for (int i = 1; i <= MAX_DIFFICULTY; i++)
            {
                Console.WriteLine("");
                Educator.evaluate(t.build(), i);
            }

            Console.ReadLine();

            return t;
        }

        static void Main(string[] args)
        {
            train();
            Random rand = new Random();
            if (args.Length > 0)
            {

                foreach(string filename in args)
                {
                    if (!File.Exists(filename))
                    {
                        Console.WriteLine("File not found: " + filename);
                        continue;
                    }
                    string latestChampion = "";
                    IEnumerable<String> output = File.ReadLines(filename);
                    if (output.Count() > 0)
                    {
                        latestChampion = File.ReadLines(filename).Last();
                        Sequence champion = new Sequence(latestChampion.Trim());
                        Console.WriteLine("Champion from: " + filename);
                        Console.WriteLine("Current champion: " + champion.ToString());
                        Console.WriteLine("Visual brain: ");
                        Console.WriteLine(champion.build().ToString());

                        for (int i = 1; i <= MAX_DIFFICULTY; i++)
                        {
                            Console.WriteLine("");
                            Educator.evaluate(champion.build(), i);
                        }
                        
                        Console.ReadLine();
                    }
                }
                Console.WriteLine("All Champions read.");
                return;
            }

            if (!File.Exists("champions.txt"))
                File.WriteAllText("champions.txt", "");


            List<Sequence> sequences = new List<Sequence>();
            Console.WriteLine("Loading Sequences");
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
//                Console.Write("Evaluating sequences:");
                for(int sequence_id = lastScores.Count(); sequence_id < sequences.Count(); sequence_id++)
                {
//                    Console.Write(" " + sequence_id.ToString());
                    int score = 0;
                    for (int difficulty = 1; difficulty <= MAX_DIFFICULTY; difficulty++)
                        score += Educator.educate(sequences[sequence_id].build(), difficulty);
                    scores.Add(score);
                }
//                Console.WriteLine("");
//                Console.WriteLine("Score management");
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

//                Console.WriteLine("Sorted score list: " + String.Join('\t', scores));
                Console.WriteLine("Best: " + sequences[0].ToString());

                string latestChampion = "";
                IEnumerable<String> output = File.ReadLines("champions.txt");
                if (output.Count() > 0)
                {
                    latestChampion = File.ReadLines("champions.txt").Last();
                }
                if(latestChampion.Trim() != sequences[0].save())
                    using (StreamWriter sw = File.AppendText("champions.txt"))
                    {
                        sw.WriteLine(sequences[0].save());
                    }
//                Console.WriteLine("Deleting random sequences");

                List<int> deletion = new List<int>(); 
                while (sequences.Count() - deletion.Count() > MAX_SEQUENCES / 2 - COMPLETE_RANDOMS_PER_ROUND)
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
                // Delete the duplicates, if there are any.
                deletion = new List<int>();
                for (int i = 0; i < sequences.Count(); i++)
                    for (int j = 1+i; j < sequences.Count(); j++)
                        if (!deletion.Contains(j)) deletion.Add(j);
                deletion.Sort();
                deletion.Reverse();
                for (int i = 0; i < deletion.Count(); i++)
                {
                    sequences.RemoveAt(deletion[i]);
                    scores.RemoveAt(deletion[i]);
                }

                lastScores = new List<int>(scores);

//                Console.WriteLine("Creating babies.");
                int parents = sequences.Count();
                for (int i = 0; i < parents; i++)
                    sequences.Add(sequences[i].baby());
                while (sequences.Count() < MAX_SEQUENCES)
                    sequences.Add(new Sequence(sequences[0].getComplexity()));

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
