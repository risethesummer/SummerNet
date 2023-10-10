using System.Buffers;
using System.Runtime.InteropServices;
using Realtime.Utils.Factory;

namespace Realtime.Utils.Buffers;

public readonly unsafe struct BufferPointer<T> : IDisposable where T : unmanaged
{
    public readonly T* Buffer;
    public readonly int Length;
    public BufferPointer(T* buffer, int length)
    {
        Buffer = buffer;
        Length = length;
    }

    
    public ReadOnlyMemory<T> GetMemory(in UnmanagedMemoryManager<T> memoryManager)
    {
        memoryManager.Initialize(this);
        return memoryManager.ForgetMemory;
    }
    public void Dispose()
    {
        NativeMemory.Free(Buffer);
    }
}