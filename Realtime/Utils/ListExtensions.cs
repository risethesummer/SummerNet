using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MemoryPack;

namespace Realtime.Utils;

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

    public static byte[] SerializeWith<T>(in T value, in ReadOnlySpan<byte> prepend)
    {
        var array = GC.AllocateUninitializedArray<byte>(Unsafe.SizeOf<T>() + prepend.Length);
        prepend.CopyTo(array.AsSpan()[..prepend.Length]);
        ref var arrayRef = ref MemoryMarshal.GetReference(array.AsSpan()[prepend.Length..]);
        Unsafe.WriteUnaligned(ref arrayRef, value);
        return array;
    }
}