using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Realtime.Utils.Factory;

namespace Realtime.Utils.Buffers;

public unsafe struct AutoSizeBuffer<T> : IPoolableObject<uint> where T : unmanaged
{
    // private byte[] _buffer;
    private void* _buffer;
    private int _endIndex = -1;
    public int Length => _endIndex + 1;
    public uint Capacity { get; private set; }
    private const int SingleLength = 1;
    public AutoSizeBuffer(uint capacity)
    {
        Resize(capacity);
    }
    private static readonly nuint Size = (nuint)Unsafe.SizeOf<T>();
    public void Resize(uint size)
    {
        if (size == Length)
            return;
        _buffer = NativeMemory.Realloc(_buffer, Size);
        Capacity = size;
    }

    public void Clear()
    {
        _endIndex = -1;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public T[] AllocArray()
    // {
    //     var res = new T[Length];
    //     _buffer.ReadArray(0, res, 0, res.Length);
    //     return res;
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckToResizeIfNecessary(in int length)
    {
        var oldLength = Length;
        var remainingLength = Capacity - oldLength;
        var newLength = oldLength + length;
        if (length > remainingLength)
            Resize((uint)newLength);
    }

    public void Write<TData>(in TData data, int length = SingleLength)
    {
        CheckToResizeIfNecessary(length);
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