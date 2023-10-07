using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Realtime.Utils.Buffers;

namespace Realtime.Networks;

public unsafe struct EncodeBufferWriter : IBufferWriter<byte>, IDisposable
{
    private void* _buffer;
    private int _size;
    private readonly int _prepend;
    public EncodeBufferWriter(int prepend)
    {
        _prepend = prepend;
    }
    public void Advance(int count)
    {
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckSize(in int newSize)
    {
        var actualSize = newSize + _prepend;
        if (_size >= actualSize)
            return;
        Dispose();
        _size = actualSize;
        _buffer = NativeMemory.Alloc((nuint)_size);
    }
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        Console.WriteLine($"Use {nameof(GetMemory)}");
        return Memory<byte>.Empty;
    }
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        CheckSize(sizeHint);
        return new Span<byte>(_buffer, sizeHint)[_prepend..];
    }
    public BufferPointer<byte> PrependAndGet(in Span<byte> prepend)
    {
        var result = new Span<byte>(_buffer, _size);
        for (var i = 0; i < _prepend; i++)
            result[i] = prepend[i];
        
        var res = new BufferPointer<byte>((byte*)_buffer, _size);
        // Need to return for another usage, so not try to free the memory
        _buffer = null;
        Dispose();
        return res;
    }
    public void Dispose()
    {
        if (_buffer is not null)
            NativeMemory.Free(_buffer);
        _size = default;
    }
}