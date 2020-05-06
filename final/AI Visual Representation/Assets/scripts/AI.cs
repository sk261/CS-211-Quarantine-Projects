// Originally created by Shay Konradsdottir - 5/1/2020

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

// This file will be used to handle the AI itself.
/** Required Data-Types include:
 * 3 Priority Queues
 * Tree
 * */

/**     Tools and Abilities:
 * PriorityQueue
 * Active Element
 *      Questions:
 * G/g Active Element >. Queue Front
 * L/l Active Element <. Queue Front
 * V/v Active Element is valid input location
 *      Actions:
 * Q Add Active Element to Queue
 * P Add Active Element to Priority Queue
 * N Get the next Priority Queue
 * D Dequeue into Active Element
 * A Dequeue and add to Active Element
 * S Dequeue and subtract to Active Element
 * + +1 to Active Element
 * - -1 to Active Element
 * O print Active Element
 * I next item from the input, give to Active Element
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
        string brainString;
        Dictionary<char, string> decode = new Dictionary<char, string>();

        public AI(string brain)
        {
            brainString = brain;
            this.brain = new LogicTree(brain);
            /**     Tools and Abilities:
             * PriorityQueue
             * Active Element
             *      Questions:
             * G/g Active Element >. Queue Front
             * L/l Active Element <. Queue Front
             * V/v Active Element is valid input location
             *      Actions:
             * Q Add Active Element to Queue
             * P Add Active Element to Priority Queue
             * N Get the next priority queue
             * W Active Element = length of input
             * D Dequeue into Active Element
             * + +1 to Active Element
             * - -1 to Active Element
             * O print Active Element
             * I get the Active Element'th item from the input, give to Active Element
             * F End/Submit
             * */

            decode['G'] = "if (active > current.peek()) {";
            decode['g'] = "}";
            decode['L'] = "if (active < current.peek()) {";
            decode['l'] = "}";
            decode['V'] = "if (active >= 0 && active < input.length) {";
            decode['v'] = "}";
            decode['Q'] = "current.enqueue(active);";
            decode['P'] = "current.priorityEnqueue(active);";
            decode['N'] = "current = nextQueue();";
            decode['D'] = "active = current.dequeue();";
            decode['A'] = "active += current.dequeue();";
            decode['+'] = "active++;";
            decode['-'] = "active--;";
            decode['O'] = "print(active)";
            decode['I'] = "active = input[active];";
            decode['F'] = "return;";

        }

        public void loadInput(int[] input)
        {
            this.input = input;
        }

        public AIEnumerator getEnum()
        {
            return new AIEnumerator(brain, input);
        }

        private string traverseTreeUnity(LogicTree.TreeNode current, int indent, int maxWalk)
        {
            if (current is null) return "";
            string color = "FF0000";
            if (current.timesWalked > 0)
            {
                int _c = 100 + ((155 * current.timesWalked) / maxWalk);
                color = "00" + _c.ToString("X") + "00";
                if (color.Length < 6) color = "0" + color;
            }

            string ret = "<color=#" + color + ">";
            foreach (char n in current.vals)
            {
                ret += new String('\t', indent) + decode[n] + "\n";
                if (n == 'O' || n == 'F')
                {
                    ret += "</color><color=#FF0000>";
                    color = "FF0000";
                }
            }

            if (decode.ContainsKey(current.question))
            {
                ret += new String('\t', indent) + decode[current.question];
                ret += "</color> \n";
                ret += traverseTreeUnity(current.right, indent + 1, maxWalk);
                ret += new String('\t', indent) + "<color=#" + color + "> } else { </color> \n";
                ret += traverseTreeUnity(current.left, indent + 1, maxWalk);
                ret += new String('\t', indent) + "<color=#" + color + "> } \n";
            }
            ret += "</color>";
            return ret;
        }

        public string traverseTreeWithUnityCharacteristics()
        {
            string ret = "current = priorityQueues(); // handles multiple priority queues\nactive = 0;\n// Additionally, there is an int[] called input\n\nwhile(!quit) // Can be stopped if need be.\n{\n";
            ret += traverseTreeUnity(brain.root, 1, brain.root.timesWalked);
            return ret + "}";
        }

        public override string ToString()
        {

            string ret = "current = priorityQueues(); // handles multiple priority queues\nactive = 0;\n// Additionally, there is an int[] called input\n\nwhile(!quit) // Can be stopped if need be.\n{\n";
            int indent = 1;

            foreach (char n in brainString)
            {
                if (decode[n].EndsWith("}")) indent--;
                ret += new String('\t', indent) + decode[n] + "\n";
                if (decode[n].EndsWith("{")) indent += 1;
            }

            return ret + "}";
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

        public bool fullyTraversed()
        {
            return fullyTraversed(brain.root);
        }

        private bool fullyTraversed(LogicTree.TreeNode current)
        {
            if (current is null) return true;
            if (current.timesWalked == 0) return false;
            return fullyTraversed(current.left) && fullyTraversed(current.right);
        }

        public AIEnumeratorValues next()
        {
            bool print = false;
            int value = 0;
            bool state_changed = false;
            bool end = false;
            bool error = false;
            bool faulty = false;

            LogicTree.TreeNode current = brain.root;
            int valLength = 0;

            while (current != null && !error && !print)
            {
                string vals = current.vals;
                valLength = vals.Length;
                current.timesWalked++;
                foreach (char c in current.vals)
                {
                    valLength--;
                    // Commands are here
                    /**
                    * Q Add Active Element to Queue
                    * P Add Active Element to Priority Queue
                    * D Dequeue into Active Element
                    * N Get next Priority Queue
                    * W Active Element = length of input
                    * A Dequeue and add to current element
                    * + +1 to Active Element
                    * - -1 to Active Element
                    * O print Active Element
                    * I get the Active Element'th item from the input, give to Active Element
                    * F End/Submit
                    * */
                    switch (c)
                    {
                        case 'Q':
                            state_changed = true;
                            state.queue().Enqueue(state.active);
                            break;
                        case 'P':
                            state_changed = true;
                            state.priority().Enqueue(state.active);
                            break;
                        case 'N':
                            state_changed = true;
                            state.nextQueue();
                            break;
                        case 'D':
                            state_changed = true;
                            if (!state.canDequeue()) { error = true; break; }
                            state.active = state.Dequeue();
                            break;
                        case 'A':
                            state_changed = true;
                            if (!state.canDequeue()) { error = true; break; }
                            state.active += state.Dequeue();
                            break;
                        case '+':
                            state_changed = true;
                            state.active++;
                            break;
                        case '-':
                            state_changed = true;
                            state.active--;
                            break;
                        case 'O':
                            value = state.active;
                            print = true;
                            break;
                        case 'I':
                            if (input.Length > 0)
                            {
                                if (!(state.active < input.Length && state.active >= 0)) { error = true; break; }
                                state_changed = true;
                                state.active = input[state.active];
                            }
                            break;
                        case 'F':
                            end = true;
                            break;
                    }
                    if (error || print || end) break;
                }
                if (error || print || end) break;
                // do question logic
                // rewriting code because it's 1:17am and I just don't want to bother simplifying a copy/paste right now.
                if (current.question == 'G')
                {
                    if (!state.canDequeue()) { error = true; break; }
                    if (state.active > state.getFront())
                        current = current.right;
                    else
                        current = current.left;
                }
                else if (current.question == 'L')
                {
                    if (!state.canDequeue()) { error = true; break; }
                    if (state.active < state.getFront())
                        current = current.right;
                    else
                        current = current.left;
                }
                else if (current.question == 'V')
                {
                    if (state.active >= 0 && state.active < input.Length)
                        current = current.right;
                    else
                        current = current.left;
                }
                else
                    break;
            }
            if (valLength > 0) faulty = true;
            return new AIEnumeratorValues(print, value, state_changed, end, error, faulty);
        }
    }

    public class AIEnumeratorValues
    {
        public bool print;
        public int value;
        public bool state_changed;
        public bool end;
        public bool error;
        public bool faulty;

        public AIEnumeratorValues(bool print, int value, bool state_changed, bool end, bool error, bool faulty)
        {
            this.print = print;
            this.value = value;
            this.state_changed = state_changed;
            this.end = end;
            this.error = error;
            this.faulty = faulty;
        }
    }



    public class State
    {
        public Queue<int>[] queues = new Queue<int>[] { new Queue<int>(), new Queue<int>(), new Queue<int>() };
        public Queue<int>[] priorities = new Queue<int>[] { new Queue<int>(), new Queue<int>(), new Queue<int>() };
        public int active = 0;
        private int index = 0;

        public void reset()
        {
            queues = new Queue<int>[] { new Queue<int>(), new Queue<int>(), new Queue<int>() };
            priorities = new Queue<int>[] { new Queue<int>(), new Queue<int>(), new Queue<int>() };
            active = 0;
            index = 0;
        }

        public Queue<int> priority()
        {
            return priorities[index];
        }

        public Queue<int> queue()
        {
            return queues[index];
        }

        public int getFront()
        {
            if (priority().Count > 0)
                return priority().Peek();
            else if (queue().Count > 0)
                return queue().Peek();
            else
                return 0;
        }

        public bool canDequeue()
        {
            return (priority().Count + queue().Count != 0);
        }

        public int Dequeue()
        {
            if (priority().Count > 0)
                return priority().Dequeue();
            else if (queue().Count > 0)
                return queue().Dequeue();
            else
                return 0;
        }

        public void nextQueue()
        {
            index = (index + 1) % queues.Length;
        }

    }

    public class LogicTree
    {
        public static char[] questions = { 'G', 'L', 'V' };
        public static char[] questionEnds = { 'g', 'l', 'v' };
        // Commands also here
        public static char[] commands = { 'Q', 'P', 'A', 'D', '+', '-', 'O', 'I', 'F', 'N' };

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
            public int timesWalked = 0;
            public bool broken = false;

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
                        tree = tree.Substring(right.vals.Length + 1);
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
