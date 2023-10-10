using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Realtime.Utils.Buffers;

public readonly unsafe struct DangerousMemory<T> : IDisposable where T : unmanaged
{
    public ReadOnlyMemory<T> Memory { get; }
    public DangerousMemory(in ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }
    public void Dispose()
    {
        ref var memoryRef = ref MemoryMarshal.GetReference(Memory.Span);
        var buffer = Unsafe.AsPointer(ref memoryRef);
        NativeMemory.Free(buffer);
    }
}