using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

// The educator will test a group of AI and give them survivability based on incentives and punishments
// The educator will test until the AI does nothing, until it submits, until it prints beyond answer size (with a punishment for doing so), or after 100 * difficulty cycles without a print (with a punishment).
/** Incentives and Punishments:
 * +10000 10^4 Prints a correct value
 * -1000 -10^3 Does nothing (no changes to active or queue) and ends program
 * -1000 -10^3 Prints beyond answer size
 * +100 10^2 Prints a value
 * +1000 10^3 if it prints all the values (regardless of sorted order) and no more/less.
 * -1000 10^3 if the system wasn't fully traversed.
 * -1 10^0 doesn't print on an iteration prior to ending
 * -100 -10^2 Faulty runs (a run where there's remainder values in a path).
 * 
 * */


namespace AI_Sorting
{
    public static class Educator
    {
        private static Random rand = new Random();

        public static void evaluate(AI ai, int difficulty)
        {
            int[] arr = new int[(difficulty + 3)];
            for (int n = 0; n < arr.Length; n++)
                arr[n] = (100 * difficulty) - getRandom(100 * difficulty);

            ai.loadInput(arr);
            int[] answer = (int[])arr.Clone();
            Array.Sort(answer);

            Console.WriteLine("Following array given: " + string.Join('\t', arr));
            Console.WriteLine("Answer: " + string.Join('\t', answer));

            List<int> prints = new List<int>();
            int lastPrintCounter = 0;
            ulong iterations = 0;
            AIEnumerator life = ai.getEnum();
            while (true)
            {
                AIEnumeratorValues loop = life.next();
                if (loop.error)
                {
                    break;
                }
                if (loop.print)
                {
                    lastPrintCounter = 0;
                    prints.Add(loop.value);
                    if (prints.Count() > answer.Length)
                        break;
                }
                if (loop.end)
                    break;
                if (!loop.state_changed && !loop.print)
                {
                    break;
                }
                iterations++;
                lastPrintCounter++;
                if (lastPrintCounter > difficulty * 100)
                {
                    break;
                }
            }

            Console.WriteLine("AI Reply: " + string.Join('\t', prints));
            int correct = 0;
            for (int a = 0; a < prints.Count() && a < answer.Count(); a++)
            {
                if (prints[a] == answer[a]) correct++;
            }
            Console.WriteLine(correct.ToString() + " correct replies");

        }

        private static int score(AI ai, int[] answer, bool show_output = false)
        {
            List<int> prints = new List<int>();
            int lastPrintCounter = 0;
            ulong iterations = 0;
            bool broke = false;
            AIEnumerator life = ai.getEnum();
            int score = 0;
            int endingLoops = 0;

            while (true)
            {
                AIEnumeratorValues loop = life.next();
                if (loop.error)
                {
                    broke = true;
                    break;
                }
                if (loop.print)
                {
                    endingLoops = 0;
                    lastPrintCounter = 0;
                    prints.Add(loop.value);

                    if (prints.Count() > 2*answer.Length)
                        break;
                }
                if (loop.faulty)
                {
                    score -= 100;
                }
                if (loop.end)
                    break;
                if (!loop.state_changed && !loop.print)
                {
                    broke = true;
                    break;
                }
                iterations++;
                lastPrintCounter++;
                if (lastPrintCounter > (answer.Length - 3) * 100)
                {
                    broke = true;
                    break;
                }
                endingLoops++;
            }

            if (int.MaxValue < iterations)
                return int.MinValue;

            score -= endingLoops;
            if (!life.fullyTraversed()) score -= 1000;
            if (broke) score -= 1000;
            score += 100 * Math.Min(prints.Count(), answer.Length);
            if (prints.Count() > answer.Length) score -= 100;
            else if (prints.Count() == answer.Length)
            {
                int[] printSorted = (int[])prints.ToArray().Clone();
                Array.Sort(printSorted);
                bool bonus = true;
                for (int a = 0; a < prints.Count() && bonus; a++) bonus = prints[a] == answer[a];
                if (bonus) score += 1000;
            }
            for (int a = 0; a < prints.Count() && a < answer.Length; a++)
                if (prints[a] == answer[a])
                    score += 10000;

            if (show_output) Console.WriteLine("AI Reply: " + string.Join('\t', prints));
            return score;

        }

        public static int educate(AI ai, int difficulty)
        {
            int[] arr = new int[(difficulty + 3)];
            for (int n = 0; n < arr.Length; n++)
                arr[n] = (100* difficulty) - getRandom(100*difficulty);

            ai.loadInput(arr);
            int[] answer = (int[])arr.Clone();
            Array.Sort(answer);

            return score(ai, answer);
        }

        public static Sequence train(Sequence seq, int difficulty, int iterations)
        {
            Sequence current = seq.copy();
            int[] arr = new int[(difficulty + 3)];
            for (int n = 0; n < arr.Length; n++)
                arr[n] = (100 * difficulty) - getRandom(100 * difficulty);
            int[] ans = (int[])arr.Clone();
            Array.Sort(ans);

            int lastScore = Int32.MinValue;

            Console.WriteLine("Total iterations: " + iterations.ToString());
            for (; iterations > 0; iterations--)
            {
                AI ai = current.build();
                ai.loadInput(arr);
                int val = score(ai, ans);
                if (val > lastScore)
                {
                    Console.WriteLine("Improvement at " + iterations.ToString() + " remaining: " + val);
                    Console.WriteLine("\t" + string.Join('\t', ans));
                    score(ai, ans, true);
                    Console.WriteLine("Current brain: " + current.save());
                    lastScore = val;
                    seq = current.copy();
                }
                current = seq.baby();
            }


            return current;
        }

        private static int getRandom(int max)
        {
            return rand.Next(0, max-1) % max;
        }
    }
}
