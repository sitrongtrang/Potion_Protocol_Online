using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Serialization
{
    public static byte[] SerializeMessage(NetworkMessage message)
    {
        BinaryFormatter formatter = new();
        using MemoryStream stream = new();
        formatter.Serialize(stream, message);
        return stream.ToArray();
    }

    public static NetworkMessage DeserializeMessage(byte[] data)
    {
        BinaryFormatter formatter = new();
        using MemoryStream stream = new(data);
        try
        {
            return (NetworkMessage)formatter.Deserialize(stream);
        }
        catch (Exception e)
        {
            Debug.LogError($"Deserialization error: {e.Message}");
            return null;
        }
    }
}