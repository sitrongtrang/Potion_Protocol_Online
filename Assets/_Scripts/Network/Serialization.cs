using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;


public static class Serialization
{
    #region Core

    /// <summary>
    /// Serialize a NetworkMessage into the binary format expected by the Java server.
    /// </summary>
    public static byte[] SerializeMessage(ClientMessage message)
    {
        try
        {
            byte[] payloadBytes = CreateByteFromType(message);

            short messageLength = (short)(2 + payloadBytes.Length);

            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            BinarySerializer.WriteInt16BigEndian(writer, messageLength);
            BinarySerializer.WriteInt16BigEndian(writer, message.MessageType);
            writer.Write(payloadBytes);

            return stream.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Serialization Error] {e.Message}");
            return null;
        }
    }

    private static byte[] CreateByteFromType(ClientMessage message)
    {
        return message.MessageType switch
        {
            NetworkMessageTypes.Client.Authentication.TryAuth => BinarySerializer.SerializeToBytes((AuthMessage)message),
            NetworkMessageTypes.Client.Authentication.TryReconnect => BinarySerializer.SerializeToBytes((ReconnectMessage)message),

            NetworkMessageTypes.Client.Pregame.RequestSpawn => BinarySerializer.SerializeToBytes((PlayerSpawnRequest)message),

            NetworkMessageTypes.Client.Ingame.Input => BinarySerializer.SerializeToBytes((PlayerInputMessage)message),

            NetworkMessageTypes.Client.System.Ping => BinarySerializer.SerializeToBytes((PingMessage)message),

            _ => null
        };
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

            short messageLength = BinarySerializer.ReadInt16BigEndian(reader);

            short messageType = BinarySerializer.ReadInt16BigEndian(reader);

            short statusCode = BinarySerializer.ReadInt16BigEndian(reader);

            byte[] payloadBytes = reader.ReadBytes(messageLength - (2 + 2));

            return CreateMessageFromType(messageType, payloadBytes);
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
    private static ServerMessage CreateMessageFromType(short messageType, byte[] payloadBytes)
    {
        return messageType switch
        {
            // Only have cases for server broadcast json
            NetworkMessageTypes.Server.System.AuthSuccess => BinarySerializer.DeserializeFromBytes<AuthSuccessMessage>(payloadBytes),
            NetworkMessageTypes.Server.System.Pong => BinarySerializer.DeserializeFromBytes<PongMessage>(payloadBytes),
            // NetworkMessageTypes.System.Kick => BinarySerializer.DeserializeFromBytes<KickMessage>(payloadBytes)

            NetworkMessageTypes.Server.Player.Spawn => BinarySerializer.DeserializeFromBytes<PlayerSpawnMessage>(payloadBytes),
            NetworkMessageTypes.Server.Player.Connected => BinarySerializer.DeserializeFromBytes<PlayerConnectedMessage>(payloadBytes),
            NetworkMessageTypes.Server.Player.Disconnected => BinarySerializer.DeserializeFromBytes<PlayerDisconnectedMessage>(payloadBytes),

            NetworkMessageTypes.Server.GameState.StateUpdate => BinarySerializer.DeserializeFromBytes<GameStatesUpdate>(payloadBytes),
            _ => null
        };
    }
    #endregion

    #region Utilities
    // private static void WriteInt16BigEndian(BinaryWriter writer, short value)
    // {
    //     writer.Write((byte)((value >> 8) & 0xFF));
    //     writer.Write((byte)(value & 0xFF));
    // }
    // private static void WriteInt32BigEndian(BinaryWriter writer, int value)
    // {
    //     writer.Write((byte)((value >> 24) & 0xFF));
    //     writer.Write((byte)((value >> 16) & 0xFF));
    //     writer.Write((byte)((value >> 8) & 0xFF));
    //     writer.Write((byte)(value & 0xFF));
    // }
    // private static void WriteFloat32BigEndian(BinaryWriter writer, float value)
    // {
    //     byte[] bytes = BitConverter.GetBytes(value);
    //     if (BitConverter.IsLittleEndian)
    //         Array.Reverse(bytes);
    //     writer.Write(bytes);
    // }

    // private static short ReadInt16BigEndian(BinaryReader reader)
    // {
    //     byte[] bytes = reader.ReadBytes(2);
    //     return (short)((bytes[0] << 8) | bytes[1]);
    // }
    // private static int ReadInt32BigEndian(BinaryReader reader)
    // {
    //     byte[] bytes = reader.ReadBytes(4);
    //     return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
    // }
    // private static float ReadFloat32BigEndian(BinaryReader reader)
    // {
    //     byte[] bytes = reader.ReadBytes(4);
    //     if (BitConverter.IsLittleEndian)
    //         Array.Reverse(bytes);
    //     return BitConverter.ToSingle(bytes, 0);
    // }
    #endregion
}
