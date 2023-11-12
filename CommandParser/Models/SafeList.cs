namespace CommandParser.Models;

public class SafeList<T> : List<T>
{
    public SafeList(IEnumerable<T> source) : base(source)
    {
    }

    private T? TryGet(int index)
        => index >= 0 && index < Count ? base[index] : default;

    public new T? this[int index] => TryGet(index);
}