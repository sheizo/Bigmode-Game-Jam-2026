using System;
using System.Collections;
using System.Collections.Generic;

public class MoveToEndFIFO<T> : IEnumerable<T>
{
    private Queue<T> queue;

    public MoveToEndFIFO()
    {
        queue = new Queue<T>();
    }

    public void Enqueue(T item)
    {
        queue.Enqueue(item);
    }

    // Dequeue: Removes an item from the front and moves it to the end
    public T Dequeue()
    {
        if (queue.Count == 0)
        {
            throw new InvalidOperationException("The structure is empty.");
        }

        // Remove the item from the front
        T dequeuedItem = queue.Dequeue();

        // Add it to the back
        queue.Enqueue(dequeuedItem);

        return dequeuedItem;
    }

        // Implement IEnumerable<T> to make this class iterable
    public IEnumerator<T> GetEnumerator()
    {
        // This returns an enumerator that allows you to iterate through the queue
        foreach (var item in queue)
        {
            yield return item;
        }
    }

    // Explicitly implement the non-generic GetEnumerator for IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}