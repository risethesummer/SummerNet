using System.Numerics;

namespace Realtime.Controllers.Transporters.Payloads;

public readonly struct NetworkedVector3
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }

    public static implicit operator Vector3(in NetworkedVector3 networkedVector3) =>
        new(networkedVector3.X, networkedVector3.Y, networkedVector3.Z);
    public static implicit operator NetworkedVector3(in Vector3 networkedVector3) => new() 
    {
            X = networkedVector3.X,
            Y = networkedVector3.Y,
            Z = networkedVector3.Z 
    };
}