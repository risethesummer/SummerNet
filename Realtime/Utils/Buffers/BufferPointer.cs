namespace Realtime.Utils.Buffers;

public readonly unsafe struct BufferPointer<T> where T : unmanaged
{
    public readonly T* Buffer;
    public readonly int Length;
    public BufferPointer(T* buffer, int length)
    {
        Buffer = buffer;
        Length = length;
    }
}