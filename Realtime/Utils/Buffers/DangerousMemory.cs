using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Realtime.Utils.Extensions;

namespace Realtime.Utils.Buffers;

public readonly struct DangerousMemory<T> : IDisposable where T : unmanaged
{
    public ReadOnlyMemory<T> Memory { get; }
    public DangerousMemory(in ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }
    public void Dispose() => Memory.Free();
}