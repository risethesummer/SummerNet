using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Utils.Buffers;

public unsafe struct AutoSizeBuffer<T> : IPoolableObject<uint> where T : unmanaged
{
    // private byte[] _buffer;
    private void* _buffer;
    private int _endIndex = -1;
    public int Length => _endIndex + 1;
    public uint Capacity { get; private set; }
    private static readonly nuint Size = (nuint)Unsafe.SizeOf<T>();
    public AutoSizeBuffer(uint capacity)
    {
        Resize(capacity);
    }
    public AutoSizeBuffer(in ReadOnlyMemory<T> data)
    {
        Resize((uint)data.Length);
        Write(data);
    }

    public static BufferPointer<T> GetBufferPointer(in ReadOnlyMemory<T> data)
    {
        var buffer = new AutoSizeBuffer<T>(data);
        return buffer.BufferPointer;
    }
    public void Resize(uint size)
    {
        if (size == Length)
            return;
        Capacity = size;
        _buffer = NativeMemory.Realloc(_buffer, Size * Capacity);
    }

    public void Clear()
    {
        _endIndex = -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckToResizeIfNecessary(in int inc)
    {
        var oldLength = Length;
        var remainingLength = Capacity - oldLength;
        var newLength = oldLength + inc;
        if (inc > remainingLength)
            Resize((uint)newLength);
    }

    public void Write(in T data)
    {
        CheckToResizeIfNecessary(1);
        Unsafe.Write(Unsafe.Add<T>(_buffer, Length), data);
        _endIndex += 1;
    }
    
    public void Write(in ReadOnlyMemory<T> data)
    {
        var length = data.Length;
        CheckToResizeIfNecessary(length);
        NativeMemory.Copy(Unsafe.Add<T>(_buffer, Length), data.AsPointer(), (nuint)length * Size);
        Unsafe.Write(Unsafe.Add<T>(_buffer, Length), data);
        _endIndex += length;
    }


    public BufferPointer<T> BufferPointer => new((T*)_buffer, Length);
    
    public void Initialize(in uint param)
    {
        Resize(param);
    }

    public void Dispose()
    {
        Clear();
        NativeMemory.Free(_buffer);
        Capacity = 0;
    }
}