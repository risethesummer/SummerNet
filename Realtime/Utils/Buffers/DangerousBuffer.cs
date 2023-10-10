using System.Runtime.InteropServices;
using Realtime.Utils.Extensions;

namespace Realtime.Utils.Buffers;

public readonly unsafe struct DangerousBuffer : IDisposable
{
    private readonly void* _buffer;
    public DangerousBuffer(void* buffer)
    {
        _buffer = buffer;
    }
    public void Dispose()
    {
        NativeMemory.Free(_buffer);
    }
}