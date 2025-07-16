using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Serialization
{
    // You must assign this from the outside (before serializing)
    public static Func<NetworkMessage, int> GenerateMessageId = _ => UnityEngine.Random.Range(1, int.MaxValue);

    /// <summary>
    /// Serialize a NetworkMessage into the binary format expected by the Java server.
    /// </summary>
    public static byte[] SerializeMessage(NetworkMessage message)
    {
        int messageId = GenerateMessageId?.Invoke(message) ?? UnityEngine.Random.Range(1, int.MaxValue);

        string payloadJson = JsonUtility.ToJson(message);
        string wrappedJson = "{\"payload\":" + payloadJson + "}";
        byte[] jsonBytes = Encoding.UTF8.GetBytes(wrappedJson);

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((short)0); // placeholder for length

        // Manually write big-endian MessageType (short)
        writer.Write((byte)((message.MessageType >> 8) & 0xFF));
        writer.Write((byte)(message.MessageType & 0xFF));

        // Manually write big-endian MessageId (int)
        writer.Write((byte)((messageId >> 24) & 0xFF));
        writer.Write((byte)((messageId >> 16) & 0xFF));
        writer.Write((byte)((messageId >> 8) & 0xFF));
        writer.Write((byte)(messageId & 0xFF));

        writer.Write(jsonBytes);

        byte[] messageBytes = stream.ToArray();

        // Set correct length (excluding the length prefix itself)
        short actualLength = (short)(messageBytes.Length - 2);
        messageBytes[0] = (byte)((actualLength >> 8) & 0xFF);
        messageBytes[1] = (byte)(actualLength & 0xFF);

        return messageBytes;
    }


    /// <summary>
    /// Deserialize a server response (byte array) into a message structure.
    /// </summary>
    public static NetworkMessage DeserializeMessage(byte[] rawData)
    {
        try
        {
            using MemoryStream stream = new(rawData);
            using BinaryReader reader = new(stream);

            short messageType = reader.ReadInt16();  // 2 bytes
            int messageId = reader.ReadInt32();      // 4 bytes
            short statusCode = reader.ReadInt16();   // 2 bytes

            byte[] payloadBytes = reader.ReadBytes(rawData.Length - 8);
            string json = Encoding.UTF8.GetString(payloadBytes);

            return CreateMessageFromType(messageType, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Deserialization Error] {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Maps short messageType codes to actual message types and deserializes from JSON.
    /// </summary>
    private static NetworkMessage CreateMessageFromType(short messageType, string json)
    {
        return messageType switch
        {
            NetworkMessageTypes.Player.Connected => JsonUtility.FromJson<PlayerConnectedMessage>(json),
            NetworkMessageTypes.Player.Disconnected => JsonUtility.FromJson<PlayerDisconnectedMessage>(json),
            NetworkMessageTypes.Player.Movement => JsonUtility.FromJson<PlayerMoveMessage>(json),
            NetworkMessageTypes.Player.Inventory => JsonUtility.FromJson<PlayerInventoryMessage>(json),
            NetworkMessageTypes.Player.Attack => JsonUtility.FromJson<PlayerAttackMessage>(json),
            NetworkMessageTypes.Player.Craft => JsonUtility.FromJson<PlayerCraftMessage>(json),
            NetworkMessageTypes.Player.Submit => JsonUtility.FromJson<PlayerSubmitMessage>(json),

            NetworkMessageTypes.Station.Update => JsonUtility.FromJson<StationUpdateMessage>(json),
            NetworkMessageTypes.Item.Drop => JsonUtility.FromJson<ItemDropMessage>(json),

            NetworkMessageTypes.Enemy.Spawn => JsonUtility.FromJson<EnemySpawnMessage>(json),
            NetworkMessageTypes.Enemy.Move => JsonUtility.FromJson<EnemyMoveMessage>(json),
            NetworkMessageTypes.Enemy.Death => JsonUtility.FromJson<EnemyDeathMessage>(json),

            NetworkMessageTypes.Resource.Spawn => JsonUtility.FromJson<ResourceSpawnMessage>(json),
            NetworkMessageTypes.Resource.Harvested => JsonUtility.FromJson<ResourceHarvestedMessage>(json),

            NetworkMessageTypes.GameState.ScoreUpdate => JsonUtility.FromJson<GameScoreUpdateMessage>(json),
            NetworkMessageTypes.GameState.TimeUpdate => JsonUtility.FromJson<GameTimeUpdateMessage>(json),

            NetworkMessageTypes.System.Ping => JsonUtility.FromJson<PingMessage>(json),
            NetworkMessageTypes.System.Pong => JsonUtility.FromJson<PongMessage>(json),
            NetworkMessageTypes.System.Kick => JsonUtility.FromJson<KickMessage>(json),

            _ => null
        };
    }
}
