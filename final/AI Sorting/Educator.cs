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
 * -100 -10^2 Prints beyond answer size
 * +100 10^2 Prints a value
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

            Console.WriteLine("Following array given: " + string.Join(' ', arr));
            Console.WriteLine("Answer: " + string.Join(' ', answer));

            List<int> prints = new List<int>();
            int lastPrintCounter = 0;
            ulong iterations = 0;
            bool broke = false;
            AIEnumerator life = ai.getEnum();
            while (true)
            {
                AIEnumeratorValues loop = life.next();
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
                    broke = true;
                    break;
                }
                iterations++;
                lastPrintCounter++;
                if (lastPrintCounter > difficulty * 100)
                {
                    broke = true;
                    break;
                }
            }

            Console.WriteLine("AI Reply: " + string.Join(' ', prints));
            int correct = 0;
            for (int a = 0; a < prints.Count() && a < answer.Count(); a++)
            {
                if (prints[a] == answer[a]) correct++;
            }
            Console.WriteLine(correct.ToString() + " correct replies");

        }
        public static int educate(AI ai, int difficulty)
        {
            int[] arr = new int[(difficulty + 3)];
            for (int n = 0; n < arr.Length; n++)
                arr[n] = (100* difficulty) - getRandom(100*difficulty);

            ai.loadInput(arr);
            int[] answer = (int[])arr.Clone();
            Array.Sort(answer);

            List<int> prints = new List<int>();
            int lastPrintCounter = 0;
            ulong iterations = 0;
            bool broke = false;
            AIEnumerator life = ai.getEnum();
            while (true)
            {
                AIEnumeratorValues loop = life.next();
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
                    broke = true;
                    break;
                }
                iterations++;
                lastPrintCounter++;
                if (lastPrintCounter > difficulty * 100)
                {
                    broke = true;
                    break;
                }
            }

            if (int.MaxValue < iterations)
                return int.MinValue;

            int score = 0;
            if (broke) score -= 1000;
            score += 100*Math.Min(prints.Count(), answer.Length);
            if (prints.Count() > answer.Length) score -= 100;
            for (int a = 0; a < prints.Count() && a < answer.Length; a++)
                if (prints[a] == answer[a])
                    score += 100000;

            return score;
        }

        static int current;
        private static int getRandom(int max)
        {
            return rand.Next(0, max-1) % max;
        }
    }
}
