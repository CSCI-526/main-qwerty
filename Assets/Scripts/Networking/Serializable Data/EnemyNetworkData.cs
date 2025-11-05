using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct EnemyNetworkData : INetworkSerializable, IEquatable<EnemyNetworkData>
{
    public ulong TargetingID;
    public FixedString128Bytes EnemyName;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref TargetingID);
        serializer.SerializeValue(ref EnemyName);
    }

    public bool Equals(EnemyNetworkData other)
    {
        return TargetingID == other.TargetingID;
    }
}
