using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> _heap = new();
    public List<T> Heap => _heap;

    public int Count => _heap.Count;

    public void Add(T item)
    {
        _heap.Add(item);
        HeapifyUp(_heap.Count - 1);
    }

    public T Peek() => _heap[0];

    public T Pop()
    {
        T root = _heap[0];
        _heap[0] = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);
        HeapifyDown(0);
        return root;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_heap[index].CompareTo(_heap[parent]) >= 0) break;
            (_heap[index], _heap[parent]) = (_heap[parent], _heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int last = _heap.Count - 1;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left <= last && _heap[left].CompareTo(_heap[smallest]) < 0) smallest = left;
            if (right <= last && _heap[right].CompareTo(_heap[smallest]) < 0) smallest = right;

            if (smallest == index) break;
            (_heap[index], _heap[smallest]) = (_heap[smallest], _heap[index]);
            index = smallest;
        }
    }
}
