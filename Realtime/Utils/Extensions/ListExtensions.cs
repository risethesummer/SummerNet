namespace Realtime.Utils.Extensions;

public static class ListExtensions
{
    public static List<T> Add<T>(this List<T> list, in Span<T> data)
    {
        list.EnsureCapacity(list.Count + data.Length);
        foreach (var element in data)
            list.Add(element);
        return list;
    }
    public static List<T> Add<T>(this List<T> list, in Memory<T> data)
    {
        return list.Add(data.Span);
    }
}