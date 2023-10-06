using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Realtime.Utils.Extensions;

public static class MemoryPackExtensions
{
    public static byte[] SerializeWith<T>(in T value, in ReadOnlySpan<byte> prepend) where T : unmanaged
    {
        var array = GC.AllocateUninitializedArray<byte>(Unsafe.SizeOf<T>() + prepend.Length);
        prepend.CopyTo(array.AsSpan()[..prepend.Length]);
        ref var arrayRef = ref MemoryMarshal.GetReference(array.AsSpan()[prepend.Length..]);
        Unsafe.WriteUnaligned(ref arrayRef, value);
        return array;
    }
    public static void Dispose<T>(this Memory<T> enumerable) where T : IDisposable
    {
        foreach (var t in enumerable.Span)
            t.Dispose();
    }
}