using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

// This file will be used to handle the AI itself.
/** Required Data-Types include:
 * Priority Queue
 * Stack
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



// Output String example:
// [Queue Starting Elements, comma delim],
// [Priority Queue Starting Elements, comma delim],
// Active Element Starting Value,
// Question[YES STRING]NO STRING
// Do Symbol

// Sample:
// [0,7,13,100],5,IALPALPDllGDgO
// Queue (q): 0, 7, 13, 100
// Active Element (i): 5
// i = input[i]; i++; if (i < q.front) {p.queue(i); i++; if (i < q.front) {p.queue(i); i = q.dequeue()}} else {if (i < q.front) {i = q.dequeue()} else {print(i)}

// Stack changes:
// Every stack is a letter and a location. If the letter is a question, the stack is a double
// Special letter for assignments: X (Add to queue, or set active if queue is empty)
// When building, start from the bottom of the stack and work up. Example from above:
// bottom | O0 | I0 | A1 | P2 | L2,4... | top

// Output file has 3 lines:
// <The AI loading string, before top stack element>
// <Top stack element, aka what can be editted>
// <Whole stack>

namespace AI_Sorting
{
    public class AI : IEnumerable<AIEnumerator>
    {
        string[] starting_queue;
        string starting_active;
        string logic;
        int[] input;
        LogicTree brain;

        public AI(string brain)
        {
            String[] n = brain.Split(']');
            starting_queue = n[0].Substring(1).Split(',');
            n = n[1].Split(',');
            starting_active = n[0];
            logic = n[1];
            this.brain = new LogicTree(logic);
        }

        public void loadInput(int[] input)
        {
            this.input = input;
        }

        public IEnumerator<AIEnumerator> GetEnumerator()
        {
            State.load(starting_queue, starting_active);
            bool end;
            do
            {
                bool print = false;
                int value = 0;
                bool state_changed = false;
                end = false;

                foreach (char c in brain)
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
                    switch(c)
                    {
                        case 'Q':
                            state_changed = true;
                            State.queue.Enqueue(State.active);
                            break;
                        case 'P':
                            state_changed = true;
                            State.priority.Enqueue(State.active);
                            break;
                        case 'D':
                            state_changed = true;
                            State.active = State.Dequeue();
                            break;
                        case 'A':
                            state_changed = true;
                            State.active++;
                            break;
                        case 'S':
                            state_changed = true;
                            State.active--;
                            break;
                        case 'O':
                            print = true;
                            value = State.active;
                            break;
                        case 'I':
                            if (input.Length > 0)
                            {
                                state_changed = true;
                                State.active = input[State.active % input.Length];
                            }
                            break;
                        case 'F':
                            end = true;
                            break;
                    }
                }


                yield return new AIEnumerator(print, value, state_changed, end);
            } while (!end);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class AIEnumerator
    {
        public bool print;
        public int value;
        public bool state_changed;
        public bool end;

        public AIEnumerator(bool print, int value, bool state_changed, bool end)
        {
            this.print = print;
            this.value = value;
            this.state_changed = state_changed;
            this.end = end;
        }
    }

    static class State
    {
        public static Queue<int> queue = new Queue<int>();
        public static Queue<int> priority = new Queue<int>();
        public static int active = 0;

        public static void load(string[] queue_strings, string active_string)
        {
            queue = new Queue<int>();
            priority = new Queue<int>();
            active = 0;
            for (int n = 0; n < queue_strings.Length; n++)
                if (queue_strings[n].Length > 0)
                    queue.Enqueue(Int32.Parse(queue_strings[n]));
            if (active_string.Length > 0)
                active = Int32.Parse(active_string);
        }

        public static int getFront()
        {
            if (priority.Count > 0)
                return priority.Peek();
            else if (queue.Count > 0)
                return queue.Peek();
            else
                return 0;
        }

        public static int Dequeue()
        {
            if (priority.Count > 0)
                return priority.Dequeue();
            else if (queue.Count > 0)
                return queue.Dequeue();
            else
                return 0;
        }

    }

    class LogicTree : IEnumerable<char>
    {
        public static char[] questions = { 'G', 'L', 'E' };
        public static char[] questionEnds = { 'g', 'l', 'e' };

        string logic;
        TreeNode root = null;
        public LogicTree(string logic)
        {
            this.logic = logic;
            root = new TreeNode(logic);
        }

        public IEnumerator<char> GetEnumerator()
        {
            TreeNode current = root;
            while(current != null)
            {
                for (int n = 0; n < current.vals.Length; n++)
                {
                    char ret = current.vals[n];
                    yield return ret;
                }
                if (questions.Contains(current.question))
                {
                    // do question logic
                    // rewriting code because it's 1:17am and I just don't want to bother simplifying a copy/paste right now.
                    if (current.question == 'G')
                    {
                        if (State.active > State.getFront())
                            current = current.right;
                        else
                            current = current.left;
                    }
                    else if (current.question == 'L')
                    {
                        if (State.active < State.getFront())
                            current = current.right;
                        else
                            current = current.left;
                    }
                    else if (current.question == 'E')
                    {
                        if (State.active == State.getFront())
                            current = current.right;
                        else
                            current = current.left;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class TreeNode
        {
            public String vals;
            public char question = '_';
            public TreeNode left, right;

            public TreeNode(string tree)
            {
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
