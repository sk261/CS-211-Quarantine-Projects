// Originally created by Shay Konradsdottir - 5/1/2020

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

// This file will be used to handle the AI itself.
/** Required Data-Types include:
 * Priority Queue
 * Tree
 * */

/**     Tools and Abilities:
 * PriorityQueue
 * Active Element
 *      Questions:
 * G/g Active Element >. Queue Front
 * L/l Active Element <. Queue Front
 * E/e Active Element =. Queue Front
 *      Actions:
 * Q Add Active Element to Queue
 * P Add Active Element to Priority Queue
 * D Dequeue into Active Element
 * A +1 to Active Element
 * S -1 to Active Element
 * O print Active Element
 * I get the Active Element'th item from the input, give to Active Element
 * F End/Submit
 * */

// Sample:
// IALPALPDllGDgO
// i = input[i]; i++; if (i < q.front) {p.queue(i); i++; if (i < q.front) {p.queue(i); i = q.dequeue()}} else {if (i < q.front) {i = q.dequeue()} else {print(i)}

namespace AI_Sorting
{
    public class AI
    {
        int[] input;
        LogicTree brain;

        public AI(string brain)
        {
            this.brain = new LogicTree(brain);
        }

        public void loadInput(int[] input)
        {
            this.input = input;
        }

        public AIEnumerator getEnum()
        {
            return new AIEnumerator(brain, input);
        }

    }

    public class AIEnumerator
    {
        public State state;
        public LogicTree brain;
        public int[] input;

        public AIEnumerator(LogicTree brain, int[] input)
        {
            state = new State();
            this.brain = brain;
            this.input = input;
        }

        public AIEnumeratorValues next()
        {
            bool print = false;
            int value = 0;
            bool state_changed = false;
            bool end = false;

            LogicTree.TreeNode current = brain.root;

            while (current != null)
            {
                foreach (char c in current.vals)
                {
                    // Commands are here
                    /**
                    * Q Add Active Element to Queue
                    * P Add Active Element to Priority Queue
                    * D Dequeue into Active Element
                    * A +1 to Active Element
                    * S -1 to Active Element
                    * O print Active Element
                    * I get the Active Element'th item from the input, give to Active Element
                    * F End/Submit
                    * */
                    switch (c)
                    {
                        case 'Q':
                            state_changed = true;
                            state.queue.Enqueue(state.active);
                            break;
                        case 'P':
                            state_changed = true;
                            state.priority.Enqueue(state.active);
                            break;
                        case 'D':
                            state_changed = true;
                            state.active = state.Dequeue();
                            break;
                        case 'A':
                            state_changed = true;
                            state.active++;
                            break;
                        case 'S':
                            state_changed = true;
                            state.active--;
                            break;
                        case 'O':
                            print = true;
                            value = state.active;
                            break;
                        case 'I':
                            if (input.Length > 0)
                            {
                                state_changed = true;
                                state.active = input[Math.Abs(state.active) % input.Length];
                            }
                            break;
                        case 'F':
                            end = true;
                            break;
                    }
                }
                // do question logic
                // rewriting code because it's 1:17am and I just don't want to bother simplifying a copy/paste right now.
                if (current.question == 'G')
                {
                    if (state.active > state.getFront())
                        current = current.right;
                    else
                        current = current.left;
                }
                else if (current.question == 'L')
                {
                    if (state.active < state.getFront())
                        current = current.right;
                    else
                        current = current.left;
                }
                else if (current.question == 'E')
                {
                    if (state.active == state.getFront())
                        current = current.right;
                    else
                        current = current.left;
                }
                else
                    break;
            }
            return new AIEnumeratorValues(print, value, state_changed, end);
        }
    }

    public class AIEnumeratorValues
    {
        public bool print;
        public int value;
        public bool state_changed;
        public bool end;

        public AIEnumeratorValues(bool print, int value, bool state_changed, bool end)
        {
            this.print = print;
            this.value = value;
            this.state_changed = state_changed;
            this.end = end;
        }
    }



    public class State
    {
        public Queue<int> queue = new Queue<int>();
        public Queue<int> priority = new Queue<int>();
        public int active = 0;

        public void reset()
        {
            queue = new Queue<int>();
            priority = new Queue<int>();
            active = 0;
        }

        public int getFront()
        {
            if (priority.Count > 0)
                return priority.Peek();
            else if (queue.Count > 0)
                return queue.Peek();
            else
                return 0;
        }

        public int Dequeue()
        {
            if (priority.Count > 0)
                return priority.Dequeue();
            else if (queue.Count > 0)
                return queue.Dequeue();
            else
                return 0;
        }

    }

    public class LogicTree
    {
        public static char[] questions = { 'G', 'L', 'E' };
        public static char[] questionEnds = { 'g', 'l', 'e' };
        // Commands also here
        public static char[] commands = { 'Q', 'P', 'D', 'A', 'S', 'O', 'I', 'F' };

        string logic;
        public TreeNode root = null;
        public LogicTree(string logic)
        {
            this.logic = logic;
            root = new TreeNode(logic);
        }

        public class TreeNode
        {
            public String vals;
            public char question = '_';
            public TreeNode left = null, right = null;

            public TreeNode(string tree)
            {
                vals = "";
                while(tree.Length > 0)
                {
                    char c = tree[0];
                    tree = tree.Substring(1);
                    if (LogicTree.questions.Contains(c))
                    {
                        question = c;
                        right = new TreeNode(tree);
                        tree = tree.Substring(right.vals.Length);
                        left = new TreeNode(tree);
                        break;
                    }
                    else if (LogicTree.questionEnds.Contains(c))
                    {
                        break;
                    }
                    vals += c;
                }
            }

        }
    }
}
