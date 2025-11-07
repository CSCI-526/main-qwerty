using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerNetworkData : INetworkSerializable, IEquatable<PlayerNetworkData>
{
    public ulong TargetingID;
    public FixedString128Bytes PlayerName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref TargetingID);
        serializer.SerializeValue(ref PlayerName);
    }

    public bool Equals(PlayerNetworkData other)
    {
        return TargetingID == other.TargetingID;
    }
}
