using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;


public static class Serialization
{
    // You must assign this from the outside (before serializing)
    public static Func<ClientMessage, int> GenerateMessageId = _ => UnityEngine.Random.Range(1, int.MaxValue);
    public static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new JsonPropertyOnlyContractResolver(),
        // MissingMemberHandling = MissingMemberHandling.Ignore
    };

    #region Core

    /// <summary>
    /// Serialize a NetworkMessage into the binary format expected by the Java server.
    /// </summary>
    public static byte[] SerializeMessage(ClientMessage message)
    {
        try
        {
            int messageId = GenerateMessageId?.Invoke(message) ?? UnityEngine.Random.Range(1, int.MaxValue);

            // 1. Wrap and encode JSON
            string payloadJson = JsonConvert.SerializeObject(message);
            string wrappedJson = "{\"payload\":" + payloadJson + "}";
            if (message.MessageType == NetworkMessageTypes.Client.Ingame.Input)
            {
                BatchPlayerInputMessage mess = (BatchPlayerInputMessage)message;
                if (mess.PlayerInputMessages.Length != 0)
                {
                    Debug.Log(payloadJson);
                }
            }
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
    private static ServerMessage CreateMessageFromType(short messageType, string json)
    {
        return messageType switch
        {
            // Only have cases for server broadcast json
            NetworkMessageTypes.Server.System.AuthSuccess => JsonConvert.DeserializeObject<AuthSuccessMessage>(json, Settings),
            NetworkMessageTypes.Server.System.Pong => JsonConvert.DeserializeObject<PongMessage>(json, Settings),
            // NetworkMessageTypes.System.Kick => JsonConvert.DeserializeObject<KickMessage>(json,settings),settings,

            NetworkMessageTypes.Server.Player.Spawn => JsonConvert.DeserializeObject<PlayerSpawnMessage>(json, Settings),
            NetworkMessageTypes.Server.Player.Connected => JsonConvert.DeserializeObject<PlayerConnectedMessage>(json, Settings),
            NetworkMessageTypes.Server.Player.Disconnected => JsonConvert.DeserializeObject<PlayerDisconnectedMessage>(json, Settings),

            NetworkMessageTypes.Server.GameState.StateUpdate => gameStatesUpdate(json),
            NetworkMessageTypes.Server.GameState.ScoreUpdate => JsonConvert.DeserializeObject<GameScoreUpdateMessage>(json, Settings),
            NetworkMessageTypes.Server.GameState.TimeUpdate => JsonConvert.DeserializeObject<GameTimeUpdateMessage>(json, Settings),


            _ => null
        };
    }
    #endregion

    public static GameStatesUpdate gameStatesUpdate(string json)
    {
        var outerWrapper = JsonConvert.DeserializeObject<OuterGameStatesWrapper>(json, Settings);
        return JsonConvert.DeserializeObject<GameStatesUpdate>(outerWrapper.GameStatesJson, Settings);
    }

    #region Utilities
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
    private static void WriteFloat32BigEndian(BinaryWriter writer, float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        writer.Write(bytes);
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
    private static float ReadFloat32BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }
    #endregion
}
