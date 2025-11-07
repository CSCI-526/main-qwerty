using System;
using Unity.Netcode;
using UnityEngine;

public struct ProjectileNetworkData : INetworkSerializable, IEquatable<ProjectileNetworkData>
{
    public ulong TargetingID;
    public ulong Damage;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref TargetingID);
        serializer.SerializeValue(ref Damage);
    }

    public bool Equals(ProjectileNetworkData other)
    {
        return TargetingID == other.TargetingID;
    }
}
