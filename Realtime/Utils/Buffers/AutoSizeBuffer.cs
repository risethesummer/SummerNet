using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Realtime.Utils.Buffers;

public class AutoSizeBuffer<T> where T : unmanaged
{
    // private byte[] _buffer;
    private readonly SafeBuffer _buffer;
    private int _endIndex = -1;
    public int Length => _endIndex + 1;
    public uint Capacity { get; private set; }
    private const int SingleLength = 1;
    public AutoSizeBuffer(uint capacity)
    {
        _buffer = new SafeMemoryMappedViewHandle();
        Capacity = capacity;
        Resize(Capacity);
    }
    private static readonly ulong Size = (ulong)Unsafe.SizeOf<T>();
    public void Resize(uint size)
    {
        if (size == Length)
            return;
        var keepSize = Math.Min(size, Length);
        Span<T> currentElement = stackalloc T[(int)keepSize];
        _buffer.ReadSpan(0, currentElement); //restore old element
        _buffer.Initialize<T>(size);
        _buffer.WriteSpan<T>(0, currentElement);
        Capacity = size;
    }

    public void Clear()
    {
        _endIndex = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] AllocArray()
    {
        var res = new T[Length];
        _buffer.ReadArray(0, res, 0, res.Length);
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong CheckToResizeIfNecessary(in int length)
    {
        var oldLength = Length;
        var remainingLength = Capacity - oldLength;
        var newLength = oldLength + length;
        if (length > remainingLength)
            Resize((uint)newLength);
        return Size * (ulong)(_endIndex + SingleLength);
    }

    public void Write(T data)
    {
        var appendPos = CheckToResizeIfNecessary(SingleLength);
        _buffer.Write(appendPos, data);
        _endIndex += SingleLength;
    }
    public void Write(in ReadOnlySpan<T> data)
    {
        var appendPos = CheckToResizeIfNecessary(data.Length);
        _buffer.WriteSpan(appendPos, data);
        _endIndex += data.Length;
    }
}