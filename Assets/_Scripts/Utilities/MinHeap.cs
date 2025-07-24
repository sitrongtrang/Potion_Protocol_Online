using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private readonly List<T> _heap = new();

    public int Count => _heap.Count;

    public void Add(T item)
    {
        _heap.Add(item);
        HeapifyUp(_heap.Count - 1);
    }

    public bool TryPeek(out T result)
    {
        if (_heap.Count == 0)
        {
            result = default;
            return false;
        }
        result = _heap[0];
        return true;
    }

    public bool TryPop(out T result)
    {
        if (_heap.Count == 0)
        {
            result = default;
            return false;
        }

        result = _heap[0];
        _heap[0] = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);
        HeapifyDown(0);
        return true;
    }

    public bool Contains(T item)
    {
        foreach (var element in _heap)
        {
            if (element.CompareTo(item) == 0)
                return true;
        }
        return false;
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
        int count = _heap.Count;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left < count && _heap[left].CompareTo(_heap[smallest]) < 0) smallest = left;
            if (right < count && _heap[right].CompareTo(_heap[smallest]) < 0) smallest = right;

            if (smallest == index) break;

            (_heap[index], _heap[smallest]) = (_heap[smallest], _heap[index]);
            index = smallest;
        }
    }

    public void Clear()
    {
        _heap.Clear();
    }
}
