using System;
using System.Collections.Generic;
using System.Text;

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
    class AI
    {
    }
}
