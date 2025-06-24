namespace Learning;

public class NumberList : INumberList
{
    private List<int> _numbers = new List<int>();

    public void Add(int number)
    {
        _numbers.Add(number);
    }

    public bool Remove(int number)
    {
        return _numbers.Remove(number);
    }

    public void Clear()
    {
        _numbers.Clear();
    }

    public int Count()
    {
        return _numbers.Count;
    }

    public int Sum()
    {
        if (_numbers.Count == 0)
            return 0;

        return _numbers.Sum();
    }

    public int Max()
    {
        if (_numbers.Count == 0)
            throw new InvalidOperationException("Cannot get maximum of empty list");

        return _numbers.Max();
    }

    public int Min()
    {
        if (_numbers.Count == 0)
            throw new InvalidOperationException("Cannot get minimum of empty list");

        return _numbers.Min();
    }

    public double Average()
    {
        if (_numbers.Count == 0)
            throw new InvalidOperationException("Cannot calculate average of empty list");

        return _numbers.Average();
    }

    public bool Contains(int number)
    {
        return _numbers.Contains(number);
    }
}