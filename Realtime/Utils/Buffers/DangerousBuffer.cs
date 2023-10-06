namespace Realtime.Utils.Buffers;

public unsafe struct DangerousBuffer<T> where T : unmanaged
{
    public readonly T* Buffer;
    public readonly int Length;
    public DangerousBuffer(T* buffer, int length)
    {
        Buffer = buffer;
        Length = length;
    }
}