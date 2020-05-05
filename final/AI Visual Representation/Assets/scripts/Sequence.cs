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
// When building, start from the bottom of the stack and work up. Example from above:
// bottom O,0|I,0|A,1|P,2|L,2...


namespace AI_Sorting
{
    public class Sequence
    {
        Stack<SequenceItem> sequence;
        private string currentBrain;
        static Random rand = new Random();
//      For future goal:
//        static private int sequentialIndex = 0;

        public Sequence(int complexity = -1)
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

            if (complexity == -1) complexity = rand.Next(1, 30);
            else complexity = rand.Next(1, complexity * complexity);
            for (int a = complexity; a > 0; a--) addRandomElement();
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

        public Sequence baby(int complexity = -1)
        {
            Sequence ret = copy();
            if (complexity == -1) complexity = rand.Next(1, Math.Max(30, ret.getComplexity()));
            for (; complexity > 0; complexity --) ret.addRandomElement();
            return ret;
        }

        public int getComplexity()
        {
            return sequence.Count();
        }

        public void deleteElement(int index)
        {
            // This is literally the worst thing I've ever done to a stack.

            Queue<SequenceItem> temp = new Queue<SequenceItem>();
            for (index = sequence.Count - index - 1; index > 0; index--)
            {
                temp.Enqueue(sequence.Pop());
            }
            sequence.Pop();
            while(temp.Count > 0)
                sequence.Push(temp.Dequeue());
        }

        public void addRandomElement()
        {

            int index = rand.Next(0, currentBrain.Length);
            int r = rand.Next((sequence.Count > 0) ? -1 : 0, LogicTree.questions.Length + LogicTree.commands.Length);
            char c;
            if (r < 0)
            {
                for (int a = rand.Next(0, sequence.Count); a > 0; a--)
                    deleteElement(rand.Next(0, sequence.Count));
                return;
            }
            else if (r >= LogicTree.questions.Length)
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

        public void addElement(char c, int index)
        {
            addElement(new SequenceItem(c, index));
        }

        public void addElement(SequenceItem top)
        {
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
    public class SequenceItem
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
