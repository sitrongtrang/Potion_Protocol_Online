using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Serialization
{
    // You must assign this from the outside (before serializing)
    public static Func<ClientMessage, int> GenerateMessageId = _ => UnityEngine.Random.Range(1, int.MaxValue);

    /// <summary>
    /// Serialize a NetworkMessage into the binary format expected by the Java server.
    /// </summary>
    public static byte[] SerializeMessage(ClientMessage message)
    {
        try
        {
            int messageId = GenerateMessageId?.Invoke(message) ?? UnityEngine.Random.Range(1, int.MaxValue);

            // 1. Wrap and encode JSON
            string payloadJson = JsonUtilityWrapper.ToJson(message);
            string wrappedJson = "{\"payload\":" + payloadJson + "}";
            byte[] payloadBytes = Encoding.UTF8.GetBytes(wrappedJson);

            // 2. Calculate message length (excluding the 2-byte length field itself)
            short messageLength = (short)(2 + 4 + payloadBytes.Length); // type + id + payload

            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            // 3. Write fields (all in big-endian)
            WriteInt16BigEndian(writer, messageLength);
            WriteInt16BigEndian(writer, message.MessageType);
            WriteInt32BigEndian(writer, messageId);
            writer.Write(payloadBytes);

            return stream.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Serialization Error] {e.Message}");
            return null;
        }
    }
    private static void WriteInt16BigEndian(BinaryWriter writer, short value)
    {
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }
    private static void WriteInt32BigEndian(BinaryWriter writer, int value)
    {
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    /// <summary>
    /// Deserialize a server response (byte array) into a message structure.
    /// </summary>
    public static ServerMessage DeserializeMessage(byte[] rawData)
    {
        try
        {
            using MemoryStream stream = new(rawData);
            using BinaryReader reader = new(stream);

            short messageLength = ReadInt16BigEndian(reader);

            short messageType = ReadInt16BigEndian(reader);
            int messageId = ReadInt32BigEndian(reader);
            short statusCode = ReadInt16BigEndian(reader);

            byte[] payloadBytes = reader.ReadBytes(messageLength - (2 + 4 + 2));
            string json = Encoding.UTF8.GetString(payloadBytes);

            // Debug.Log($"[Deserialize] type={messageType}, id={messageId}, status={statusCode}, json={json}");
            ServerMessage server = CreateMessageFromType(messageType, json);
            return CreateMessageFromType(messageType, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Deserialization Error] {e.Message}");
            return null;
        }
    }
    private static short ReadInt16BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(2);
        return (short)((bytes[0] << 8) | bytes[1]);
    }
    private static int ReadInt32BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(4);
        return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
    }

    /// <summary>
    /// Maps short messageType codes to actual message types and deserializes from JSON.
    /// </summary>
    private static ServerMessage CreateMessageFromType(short messageType, string json)
    {
        return messageType switch
        {
            // Only have cases for server broadcast json
            NetworkMessageTypes.System.AuthSuccess => JsonUtilityWrapper.FromJson<AuthSuccessMessage>(json),

            NetworkMessageTypes.Player.Connected => JsonUtilityWrapper.FromJson<PlayerConnectedMessage>(json),
            NetworkMessageTypes.Player.Disconnected => JsonUtilityWrapper.FromJson<PlayerDisconnectedMessage>(json),
            NetworkMessageTypes.Player.Movement => JsonUtilityWrapper.FromJson<PlayerMoveMessage>(json),
            NetworkMessageTypes.Player.Inventory => JsonUtilityWrapper.FromJson<PlayerInventoryMessage>(json),
            NetworkMessageTypes.Player.Attack => JsonUtilityWrapper.FromJson<PlayerAttackMessage>(json),
            NetworkMessageTypes.Player.Craft => JsonUtilityWrapper.FromJson<PlayerCraftMessage>(json),
            NetworkMessageTypes.Player.Submit => JsonUtilityWrapper.FromJson<PlayerSubmitMessage>(json),

            NetworkMessageTypes.Station.Update => JsonUtilityWrapper.FromJson<StationUpdateMessage>(json),
            NetworkMessageTypes.Item.Drop => JsonUtilityWrapper.FromJson<ItemDropMessage>(json),

            NetworkMessageTypes.Enemy.Spawn => JsonUtilityWrapper.FromJson<EnemySpawnMessage>(json),
            NetworkMessageTypes.Enemy.Move => JsonUtilityWrapper.FromJson<EnemyMoveMessage>(json),
            NetworkMessageTypes.Enemy.Death => JsonUtilityWrapper.FromJson<EnemyDeathMessage>(json),

            NetworkMessageTypes.Resource.Spawn => JsonUtilityWrapper.FromJson<ResourceSpawnMessage>(json),
            NetworkMessageTypes.Resource.Harvested => JsonUtilityWrapper.FromJson<ResourceHarvestedMessage>(json),

            NetworkMessageTypes.GameState.ScoreUpdate => JsonUtilityWrapper.FromJson<GameScoreUpdateMessage>(json),
            NetworkMessageTypes.GameState.TimeUpdate => JsonUtilityWrapper.FromJson<GameTimeUpdateMessage>(json),

            NetworkMessageTypes.System.Pong => JsonUtilityWrapper.FromJson<PongMessage>(json),
            NetworkMessageTypes.System.Kick => JsonUtilityWrapper.FromJson<KickMessage>(json),

            _ => null
        };
    }
}
