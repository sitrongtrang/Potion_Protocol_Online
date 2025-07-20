using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class BinarySerializer
{
    public static void Serialize<T>(BinaryWriter writer, T obj)
    {
        foreach (var field in GetOrderedSerializableFields(typeof(T)))
        {
            object value = field.GetValue(obj);
            WriteValue(writer, value, field.FieldType);
        }
    }

    public static T Deserialize<T>(BinaryReader reader) where T : new()
    {
        T obj = new();
        foreach (var field in GetOrderedSerializableFields(typeof(T)))
        {
            object value = ReadValue(reader, field.FieldType);
            field.SetValue(obj, value);
        }
        return obj;
    }

    private static List<FieldInfo> GetOrderedSerializableFields(Type type)
    {
        return GetAllFields(type)
            .Select(f => new
            {
                Field = f,
                Attribute = f.GetCustomAttribute<FieldOrderAttribute>()
            })
            .Where(x => x.Attribute != null)
            .OrderBy(x => x.Attribute.Order)
            .Select(x => x.Field)
            .ToList();
    }

    private static IEnumerable<FieldInfo> GetAllFields(Type type)
    {
        if (type == null || type == typeof(object))
            return Enumerable.Empty<FieldInfo>();

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return type.GetFields(flags)
                   .Where(f => !f.IsStatic)
                   .Concat(GetAllFields(type.BaseType));
    }

    private static void WriteValue(BinaryWriter writer, object value, Type type)
    {
        if (type == typeof(int))
        {
            WriteInt32BigEndian(writer, (int)value);
        }
        else if (type == typeof(short))
        {
            WriteInt16BigEndian(writer, (short)value);
        }
        else if (type == typeof(float))
        {
            WriteFloat32BigEndian(writer, (float)value);
        }
        else if (type == typeof(bool))
        {
            writer.Write((byte)((bool)value ? 1 : 0));
        }
        else if (type.IsArray)
        {
            Array array = (Array)value;
            writer.Write((short)(array?.Length ?? 0));
            foreach (var item in array)
                WriteValue(writer, item, type.GetElementType());
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            var list = (IList)value;
            writer.Write((short)(list?.Count ?? 0));
            Type elementType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            foreach (var item in list)
                WriteValue(writer, item, elementType);
        }
        else
        {
            Serialize(writer, value);
        }
    }

    private static object ReadValue(BinaryReader reader, Type type)
    {
        if (type == typeof(int))
            return ReadInt32BigEndian(reader);
        if (type == typeof(short))
            return ReadInt16BigEndian(reader);
        if (type == typeof(float))
            return ReadFloat32BigEndian(reader);
        if (type == typeof(bool))
            return reader.ReadByte() != 0;

        if (type.IsArray)
        {
            short length = reader.ReadInt16();
            Type elementType = type.GetElementType();
            Array array = Array.CreateInstance(elementType, length);
            for (int i = 0; i < length; i++)
                array.SetValue(ReadValue(reader, elementType), i);
            return array;
        }

        if (typeof(IList).IsAssignableFrom(type))
        {
            short length = reader.ReadInt16();
            Type elementType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            for (int i = 0; i < length; i++)
                list.Add(ReadValue(reader, elementType));
            return list;
        }

        return typeof(BinarySerializer)
            .GetMethod(nameof(Deserialize), BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(type)
            .Invoke(null, new object[] { reader });
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

    private static void WriteFloat32BigEndian(BinaryWriter writer, float value)
    {
        uint intValue = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        writer.Write((byte)((intValue >> 24) & 0xFF));
        writer.Write((byte)((intValue >> 16) & 0xFF));
        writer.Write((byte)((intValue >> 8) & 0xFF));
        writer.Write((byte)(intValue & 0xFF));
    }

    private static short ReadInt16BigEndian(BinaryReader reader)
    {
        return (short)((reader.ReadByte() << 8) | reader.ReadByte());
    }

    private static int ReadInt32BigEndian(BinaryReader reader)
    {
        return (reader.ReadByte() << 24) |
               (reader.ReadByte() << 16) |
               (reader.ReadByte() << 8) |
               reader.ReadByte();
    }

    private static float ReadFloat32BigEndian(BinaryReader reader)
    {
        byte[] bytes = new byte[4];
        bytes[0] = reader.ReadByte();
        bytes[1] = reader.ReadByte();
        bytes[2] = reader.ReadByte();
        bytes[3] = reader.ReadByte();
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class FieldOrderAttribute : Attribute
{
    public int Order { get; }

    public FieldOrderAttribute(int order)
    {
        Order = order;
    }
}

