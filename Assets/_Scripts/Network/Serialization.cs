using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using Encoding = System.Text.Encoding;

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

    /// <summary>
    /// Converts a message into a binary format compatible with the Java server.
    /// </summary>
    public static byte[] SerializeMessage(short methodCode, int messageId, string jsonPayload)
    {
        byte[] payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);

        int bodyLength = 2 + 4 + payloadBytes.Length;
        short totalLength = (short)bodyLength;

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write(totalLength); 
        writer.Write(methodCode);
        writer.Write(messageId);
        writer.Write(payloadBytes);

        return stream.ToArray();
    }

    /// <summary>
    /// Deserializes a server response from the Java server.
    /// </summary>
    public static bool DeserializeMessage(byte[] rawData, out short responseType, out int messageId, out short statusCode, out string payload)
    {
        responseType = 0;
        messageId = 0;
        statusCode = 0;
        payload = null;

        try
        {
            using MemoryStream stream = new(rawData);
            using BinaryReader reader = new(stream);

            responseType = reader.ReadInt16();
            messageId = reader.ReadInt32();
            statusCode = reader.ReadInt16();

            int payloadLength = rawData.Length - 8;
            byte[] payloadBytes = reader.ReadBytes(payloadLength);
            payload = Encoding.UTF8.GetString(payloadBytes);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Deserialize error: {e.Message}");
            return false;
        }
    }
}