using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

// The educator will test a group of AI and give them survivability based on incentives and punishments
// The educator will test until the AI does nothing, until it submits, until it prints beyond answer size (with a punishment for doing so), or after 100 * difficulty cycles without a print (with a punishment).


namespace AI_Sorting
{
    public static class Educator
    {
        private static Random rand = new Random();
        public static bool debugMode = false;

        public static void evaluate(AI ai, int difficulty)
        {
            int[] arr = new int[(difficulty + 3)];
            for (int n = 0; n < arr.Length; n++)
                arr[n] = (100 * difficulty) - getRandom(100 * difficulty);

            ai.loadInput(arr);
            int[] answer = (int[])arr.Clone();
            Array.Sort(answer);

            if (debugMode) Console.WriteLine("Following array given: " + string.Join("\t", arr));
            if (debugMode) Console.WriteLine("Answer: " + string.Join("\t", answer));

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

            if (debugMode) Console.WriteLine("AI Reply: " + string.Join("\t", prints));
            int correct = 0;
            for (int a = 0; a < prints.Count() && a < answer.Count(); a++)
            {
                if (prints[a] == answer[a]) correct++;
            }
            if (debugMode) Console.WriteLine(correct.ToString() + " correct replies");

        }

        private static string lastOutput;
        private static string lastOutputFormatted;
        private static int lastTier = 0;
        private static int score(AI ai, int[] answer)
        {
            lastOutput = "";
            lastOutputFormatted = "";
            List<int> prints = new List<int>();
            int lastPrintCounter = 0;
            ulong iterations = 0;
            bool broke = false;
            AIEnumerator life = ai.getEnum();
            int endingLoops = 0;
            int faultyRuns = 0;

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

                    if (prints.Count() > 2 * answer.Length)
                        break;
                }
                if (loop.faulty)
                    faultyRuns++;
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

            /** Incentives and Punishments:
             * > Grant a reward depending on each tier. IE, if all incentives and no punishments of a tier are met, move to next tier.
             * T1 (No nothings):
             * -1000 -10^3 Does nothing (no changes to active or queue) and ends program
             * T2 (Give me values):
             * +100 10^2 Prints a value
             * +1000 10^3 if it prints a correct value (regardless of sorted order)
             * +10000 10^4 Prints a correct value in the correct place
             * T3 (Exact length and values):
             * -1000 -10^3 Prints beyond answer size
             * -1000 -10^3 Prints the wrong value
             * T3 (Optimization): (+10000 10^4 when entering this tier)
             * -1000 10^3 if the system wasn't fully traversed.
             * -1 10^0 doesn't print on an iteration prior to ending
             * -100 -10^2 Faulty runs (a run where there's remainder values in a path).
             * 
             * */


            int score = 0;
            // T1
            int tier = 1;
            if (prints.Count() <= 0 || broke)
                score -= 1000;
            else
                tier = 2;
            // T2
            if (tier == 2)
            {
                score += 100 * prints.Count();
                bool perfectScore = answer.Length <= prints.Count();
                for (int a = 0; a < answer.Length && a < prints.Count(); a++)
                {
                    if (!prints.Contains(answer[a])) score += 1000;
                    if (answer[a] == prints[a]) score += 10000;
                    else perfectScore = false;
                }
                if (perfectScore) tier = 3;
            }
            if (tier == 3)
            {
                score += 1000;
                if (!life.fullyTraversed()) score -= 1000;
                score -= faultyRuns * 100;
                score -= endingLoops * 1;
            }

            if (debugMode) Console.WriteLine("AI Reply: " + string.Join("\t", prints));
            lastOutput = string.Join("\t", prints);
            for (int a = 0; a < prints.Count(); a++)
            {
                string add = "<color=#";
                if (a < answer.Length && prints[a] == answer[a])
                    add += "00FF00";
                else
                    add += "FF0000";
                add += ">";
                lastOutputFormatted += add + prints[a] + "</color>\t";
            }
            lastTier = tier;

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

        private static int[] TickTrainVals = null;
        public static int getConstTrainVal(Sequence seq, int difficulty = 1)
        {

            AI temp = getConstTain(seq, difficulty);
            int[] ans = (int[])TickTrainVals.Clone();
            Array.Sort(ans);
            return score(temp, ans);
        }

        public static AI getConstTrainAI(Sequence seq, int difficulty = 1)
        {
            AI temp = getConstTain(seq, difficulty);
            int[] ans = (int[])TickTrainVals.Clone();
            Array.Sort(ans);
            score(temp, ans);
            return temp;
        }

        public static string getConstGoal(int difficulty = 1, bool reset = false)
        {
            if (TickTrainVals is null || TickTrainVals.Length - 3 < difficulty || reset)
            {
                TickTrainVals = new int[(difficulty + 3)];
                for (int n = 0; n < TickTrainVals.Length; n++)
                    TickTrainVals[n] = (100) - getRandom(100);
            }

            return string.Join("\t", TickTrainVals);
        }

        public static string getConstReply()
        {
            return lastOutputFormatted;
        }

        public static int getConstTier()
        {
            return lastTier;
        }

        private static AI getConstTain(Sequence seq, int difficulty = 1)
        {
            getConstGoal(difficulty);

            Sequence current = seq.copy();
            AI ai = current.build();
            ai.loadInput((int[])TickTrainVals.Clone());

            return ai;
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

            if (debugMode) Console.WriteLine("Total iterations: " + iterations.ToString());
            for (; iterations > 0; iterations--)
            {
                AI ai = current.build();
                ai.loadInput(arr);
                int val = score(ai, ans);
                if (val > lastScore)
                {
                    if (debugMode) Console.WriteLine("Improvement at " + iterations.ToString() + " remaining: " + val);
                    if (debugMode) Console.WriteLine("\t" + string.Join("\t", ans));
                    score(ai, ans);
                    if (debugMode) Console.WriteLine("Current brain: " + current.save());
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
