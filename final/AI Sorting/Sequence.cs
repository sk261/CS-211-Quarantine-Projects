using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;

// This class is primarily used to hold the sequence of growth of the AI
// It contains a stack for the sake of editting
// However, for the sake of building an AI, the sequence flips the stack.

// Stack changes:
// Every stack is a letter and a location. If the letter is a question, the stack is a double
// Special letter for assignments: X (Add to queue, or set active if queue is empty)
// When building, start from the bottom of the stack and work up. Example from above:
// bottom O,0|I,0|A,1|P,2|L,2...


namespace AI_Sorting
{
    class Sequence
    {
        Stack<SequenceItem> sequence;
        private string currentBrain;
        static Random rand = new Random();
//      For future goal:
//        static private int sequentialIndex = 0;

        public Sequence()
        {
            // FUTURE GOAL: Instead of random, make this sequential.
            // For future goals:
            // All possible combinations at a single point = LogicTree.questions.Length + LogicTree.commands.Length [q + c]
            // All possible indexes = currentBrain.Length [b]
            // At first iteration, maximum combinations = q+c
            // At second iteration, maximum combinations = [2*c+3*q]*[q+c]
            // At third iteration, maximum combinations = [3*c+4*[q+c]+5*q]*[q+c]
            // At 4th iteration, maximum combinations = [3*c+4*[q+c]+5*q]*[q+c]

            sequence = new Stack<SequenceItem>();
            currentBrain = "";

            for (int a = rand.Next(1, 30); a > 0; a--)
            {
                int index = rand.Next(0, currentBrain.Length);
                int r = rand.Next(0, LogicTree.questions.Length + LogicTree.commands.Length);
                char c;
                if (r >= LogicTree.questions.Length)
                    c = LogicTree.commands[r - LogicTree.questions.Length];
                else
                    c = LogicTree.questions[r];
                addElement(c, index);
            }
        }

        public Sequence(string text)
        {
            sequence = new Stack<SequenceItem>();
            currentBrain = "";
            string[] elements = text.Split('|');
            foreach (string el in elements)
                addElement(char.Parse(el.Split(',')[0]), Int32.Parse(el.Split(',')[1]));
        }

        public Sequence copy()
        {
            return new Sequence(save());
        }

        public Sequence baby()
        {
            Sequence ret = copy();
            ret.addRandomElement();
            return ret;
        }

        public void addRandomElement()
        {
            bool overwrite = (rand.Next(0, 2) == 1);

            int index = rand.Next(0, currentBrain.Length);
            int r = rand.Next(0, LogicTree.questions.Length + LogicTree.commands.Length);
            char c;
            if (r >= LogicTree.questions.Length)
                c = LogicTree.commands[r - LogicTree.questions.Length];
            else
                c = LogicTree.questions[r];
            addElement(c, index);
        }

        public string save()
        {
            string text = "";
            Stack<SequenceItem> sequence = new Stack<SequenceItem>(this.sequence);
            while(sequence.Count > 0)
            {
                if (text.Length > 0) text += "|";
                text += sequence.Pop().ToString();
            }
            return text;
        }

        public void addElement(char c, int index, bool overwrite = false)
        {
            addElement(new SequenceItem(c, index), overwrite);
        }

        public void addElement(SequenceItem top, bool overwrite = false)
        {
            if (overwrite)
            {
                SequenceItem n = sequence.Pop();
                currentBrain.Remove(n.index);
                if (LogicTree.questions.Contains(n.c))
                    currentBrain.Remove(n.index);
            }
            sequence.Push(top);
            string val = top.c.ToString();
            if (LogicTree.questions.Contains(top.c))
                val += LogicTree.questionEnds[Array.IndexOf(LogicTree.questions, top.c)];
            if (currentBrain.Length > 0)
                currentBrain = currentBrain.Insert(Math.Abs(top.index) % currentBrain.Length, val);
            else
                currentBrain = val;
        }

        public AI build()
        {
            return new AI(this.currentBrain);
        }
        public override string ToString()
        {
            return this.currentBrain;
        }

    }
    class SequenceItem
    {
        public char c;
        public int index;
        public SequenceItem(char c, int index)
        {
            this.c = c;
            this.index = index;
        }

        public override string ToString()
        {
            return c.ToString() + "," + index.ToString();
        }
    }

}
