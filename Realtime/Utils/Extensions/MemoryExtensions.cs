using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Realtime.Utils.Extensions;

public static class MemoryExtensions
{
    public static void Dispose<T>(this Memory<T> enumerable) where T : IDisposable
    {
        foreach (var t in enumerable.Span)
            t.Dispose();
    }
    public static void Dispose<T>(this ReadOnlyMemory<T> enumerable) where T : IDisposable
    {
        foreach (var t in enumerable.Span)
            t.Dispose();
    }
    public static unsafe void Free<T>(this ReadOnlyMemory<T> memory)
    {
        var buffer = memory.AsPointer();
        NativeMemory.Free(buffer);
    }
    public static unsafe void* AsPointer<T>(this Memory<T> memory)
    {
        ref var memoryRef = ref MemoryMarshal.GetReference(memory.Span);
        var buffer = Unsafe.AsPointer(ref memoryRef);
        return buffer;
    }
    public static unsafe void* AsPointer<T>(this ReadOnlyMemory<T> memory)
    {
        ref var memoryRef = ref MemoryMarshal.GetReference(memory.Span);
        var buffer = Unsafe.AsPointer(ref memoryRef);
        return buffer;
    }
    public static void Free<T>(this Memory<T> memory) => ((ReadOnlyMemory<T>)memory).Free();
}