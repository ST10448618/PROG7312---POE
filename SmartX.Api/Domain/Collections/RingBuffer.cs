using System.Collections;

namespace SmartX.Api.Domain.Collections;

/// Fixed-capacity circular buffer. O(1) insert, automatically overwrites the
/// oldest entry once full — used as the live rolling window for anomaly scoring
/// instead of an ever-growing List&lt;T&gt;.
public class RingBuffer<T> : IEnumerable<T>
{
    private readonly T[] _buffer;
    private int _head;
    private int _count;

    public RingBuffer(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _buffer = new T[capacity];
    }

    public int Capacity => _buffer.Length;
    public int Count => _count;

    public void Add(T item)
    {
        _buffer[_head] = item;
        _head = (_head + 1) % _buffer.Length;
        if (_count < _buffer.Length) _count++;
    }

    public T[] ToArray()
    {
        var result = new T[_count];
        var start = _count < _buffer.Length ? 0 : _head;
        for (var i = 0; i < _count; i++)
            result[i] = _buffer[(start + i) % _buffer.Length];
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in ToArray()) yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}