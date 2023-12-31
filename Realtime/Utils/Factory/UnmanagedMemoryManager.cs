using System.Buffers;
using System.Runtime.InteropServices;
using Realtime.Utils.Buffers;

namespace Realtime.Utils.Factory;

/// <summary>
/// A MemoryManager over a raw pointer
/// </summary>
/// <remarks>The pointer is assumed to be fully unmanaged, or externally pinned - no attempt will be made to pin this data</remarks>
public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T>, IPoolableObject<BufferPointer<T>> where T : unmanaged
{
    private T* _pointer;
    private int _length;

    public UnmanagedMemoryManager(in BufferPointer<T> span) : 
        this(span.Buffer, span.Length)
    {
    }
    public UnmanagedMemoryManager(Span<T> span)
    {
        fixed (T* ptr = &MemoryMarshal.GetReference(span))
        {
            _pointer = ptr;
            _length = span.Length;
        }
    }

    [CLSCompliant(false)]
    public UnmanagedMemoryManager(T* pointer, int length)
    {
        if (length < 0) 
            throw new ArgumentOutOfRangeException(nameof(length));
        _pointer = pointer;
        _length = length;
    }

    /// <summary>
    /// Obtains a span that represents the region
    /// </summary>
    public override Span<T> GetSpan() => new(_pointer, _length);

    /// <summary>
    /// Provides access to a pointer that represents the data (note: no actual pin occurs)
    /// </summary>
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if (elementIndex < 0 || elementIndex >= _length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(_pointer + elementIndex);
    }
    /// <summary>
    /// Has no effect
    /// </summary>
    public override void Unpin() { }

    /// <summary>
    /// Releases all resources associated with this object
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        NativeMemory.Free(_pointer);
        _pointer = null;
        _length = 0;
    }

    public ReadOnlyMemory<T> ForgetMemory
    {
        get
        {
            var mem = Memory;
            _pointer = null;
            return mem;
        }
    }
    public void Initialize(in BufferPointer<T> param)
    {
        var length = param.Length;
        if (length < 0) 
            throw new ArgumentOutOfRangeException(nameof(length));
        _pointer = param.Buffer;
        _length = length;
    }
}