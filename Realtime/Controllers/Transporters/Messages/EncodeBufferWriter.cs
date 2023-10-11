using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Realtime.Utils.Buffers;

namespace Realtime.Controllers.Transporters.Messages;

public unsafe struct EncodeBufferWriter : IBufferWriter<byte>, IDisposable
{
    private void* _buffer;
    private readonly int _prepend;
    public int Size { get; private set; }

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
        if (Size >= actualSize)
            return;
        Dispose();
        Size = actualSize;
        _buffer = NativeMemory.Alloc((nuint)Size);
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
        var result = new Span<byte>(_buffer, Size);
        for (var i = 0; i < _prepend; i++)
            result[i] = prepend[i];
        
        var res = new BufferPointer<byte>((byte*)_buffer, Size);
        // Need to return for another usage, so not try to free the memory
        _buffer = null;
        Dispose();
        return res;
    }
    public void Dispose()
    {
        if (_buffer is not null)
            NativeMemory.Free(_buffer);
        Size = default;
    }
}