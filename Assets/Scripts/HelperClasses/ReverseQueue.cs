using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReverseQueueUtil<T> where T : Object
{
    public static void ReverseQueue(ref Queue<T> inQueue)
    {

        Stack<T> stack = new Stack<T>();

        while (inQueue.Count > 0)
        {
            stack.Push(inQueue.Dequeue());
        }
        while (stack.Count > 0)
        {
            inQueue.Enqueue(stack.Peek());
            stack.Pop();
        }
    }
}
