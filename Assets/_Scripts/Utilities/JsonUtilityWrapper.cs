using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class JsonUtilityWrapper
{
    #region Case-Insensitive Deserialization

    public static T FromJson<T>(string json) where T : new()
    {
        try
        {
            var result = FromJsonCaseInsensitive<T>(json);
            if (result != null) return result;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Enhanced JSON parsing failed, falling back to JsonUtility: {e.Message}");
        }

        return JsonUtility.FromJson<T>(json);
    }

    private static T FromJsonCaseInsensitive<T>(string json) where T : new()
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        var jsonDict = ParseToDictionary(json);
        var result = new T();
        var type = typeof(T);

        ProcessMembersRecursive(jsonDict, result, type);

        return result;
    }

    private static void ProcessMembersRecursive<T>(Dictionary<string, object> jsonDict, T obj, Type type)
    {
        if (type == null || type == typeof(object))
            return;

        ProcessMembersRecursive(jsonDict, obj, type.BaseType);

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) ||
                field.Name.Contains("k__BackingField"))
                continue;

            var attr = field.GetCustomAttribute<JsonPropertyAttribute>();
            if (attr == null)
                continue;

            ProcessMember(jsonDict, obj, field, attr.PropertyName, field.FieldType, (o, val) => field.SetValue(o, val));
        }

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (prop.CanWrite && prop.GetSetMethod(true) != null)
            {
                var attr = prop.GetCustomAttribute<JsonPropertyAttribute>();
                if (attr == null)
                    continue;

                ProcessMember(jsonDict, obj, prop, attr.PropertyName, prop.PropertyType, (o, val) => prop.SetValue(o, val));
            }
        }
    }

    private static void ProcessMember<T>(Dictionary<string, object> jsonDict, T obj,
        MemberInfo member, string jsonKey, Type memberType, Action<T, object> setter)
    {
        if (!jsonDict.TryGetValue(jsonKey, out object value))
        {
            if (!memberType.IsValueType)
            {
                setter(obj, null);
            }
            return;
        }

        try
        {
            object convertedValue = ConvertValue(value, memberType);
            setter(obj, convertedValue);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to set {member.Name}: {e.Message}");
        }
    }

    #endregion

    #region Enhanced Serialization

    public static string ToJson(object obj, bool prettyPrint = false)
    {
        try
        {
            return ToJsonWithNaming(obj, prettyPrint);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Enhanced JSON serialization failed, falling back to JsonUtility: {e.Message}");
        }

        return JsonUtility.ToJson(obj, prettyPrint);
    }

    private static string ToJsonWithNaming(object obj, bool prettyPrint)
    {
        if (obj == null)
            return "null";

        var type = obj.GetType();
        var sb = new StringBuilder();
        var serializedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        sb.Append("{");

        bool first = true;
        string newLine = prettyPrint ? "\n" : "";
        string indent = prettyPrint ? "  " : "";

        SerializeMembersRecursive(sb, obj, type, ref first, prettyPrint, newLine, indent, serializedKeys);

        if (prettyPrint) sb.Append(newLine);
        sb.Append("}");
        return sb.ToString();
    }

    private static void SerializeMembersRecursive(StringBuilder sb, object obj, Type type,
        ref bool first, bool prettyPrint, string newLine, string indent, HashSet<string> serializedKeys)
    {
        if (type == null || type == typeof(object))
            return;

        SerializeMembersRecursive(sb, obj, type.BaseType, ref first, prettyPrint, newLine, indent, serializedKeys);

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) ||
                field.Name.Contains("k__BackingField"))
                continue;

            var attr = field.GetCustomAttribute<JsonPropertyAttribute>();
            if (attr == null) continue;

            string jsonName = attr.PropertyName;
            if (!serializedKeys.Add(jsonName)) continue;

            if (!first) sb.Append(",");
            first = false;

            if (prettyPrint) sb.Append(newLine + indent);

            object value = field.GetValue(obj);
            sb.Append($"\"{jsonName}\":");
            AppendJsonValue(sb, value, prettyPrint, indent);
        }

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (prop.CanRead && prop.GetGetMethod(true) != null)
            {
                var attr = prop.GetCustomAttribute<JsonPropertyAttribute>();
                if (attr == null) continue;

                string jsonName = attr.PropertyName;
                if (!serializedKeys.Add(jsonName)) continue;

                if (!first) sb.Append(",");
                first = false;

                if (prettyPrint) sb.Append(newLine + indent);

                object value = prop.GetValue(obj);
                sb.Append($"\"{jsonName}\":");
                AppendJsonValue(sb, value, prettyPrint, indent);
            }
        }
    }

    #endregion

    #region Core Utilities

    private static Dictionary<string, object> ParseToDictionary(string json)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        json = json.Trim().Trim('{', '}');
        var pairs = json.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var kv = pair.Split(new[] { ':' }, 2);
            if (kv.Length == 2)
            {
                string key = kv[0].Trim().Trim('"');
                string value = kv[1].Trim();
                result[key] = ParseJsonValue(value);
            }
        }

        return result;
    }

    private static object ParseJsonValue(string value)
    {
        value = value.Trim();

        if (value.StartsWith("{") && value.EndsWith("}"))
        {
            return value;
        }

        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            return value;
        }

        if (value.StartsWith("\"") && value.EndsWith("\""))
            return value.Trim('"');

        if (long.TryParse(value, out long longVal))
            return longVal;

        if (double.TryParse(value, out double doubleVal))
            return doubleVal;

        if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;

        if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;

        if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;

        return value;
    }

    private static object ConvertValue(object value, Type targetType)
    {
        if (value == null) return null;

        try
        {
            if (targetType.IsEnum)
            {
                if (value is string str)
                    return Enum.Parse(targetType, str, true);
                return Enum.ToObject(targetType, value);
            }

            if (targetType == typeof(string))
                return Convert.ToString(value);

            if (targetType.IsClass && value is string strValue && strValue.StartsWith("{"))
            {
                MethodInfo fromJsonMethod = typeof(JsonUtilityWrapper)
                    .GetMethod(nameof(FromJson), BindingFlags.Public | BindingFlags.Static)
                    ?.MakeGenericMethod(targetType);

                return fromJsonMethod?.Invoke(null, new object[] { strValue });
            }

            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            Debug.LogWarning($"Failed to convert {value} to {targetType.Name}");
            return value;
        }
    }

    private static void AppendJsonValue(StringBuilder sb, object value, bool prettyPrint, string indent)
    {
        if (value == null)
        {
            sb.Append("null");
        }
        else if (value is string str)
        {
            sb.Append($"\"{str}\"");
        }
        else if (value is bool b)
        {
            sb.Append(b ? "true" : "false");
        }
        else if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            sb.Append("[");
            bool first = true;
            foreach (var item in enumerable)
            {
                if (!first) sb.Append(",");
                first = false;
                AppendJsonValue(sb, item, prettyPrint, indent + "  ");
            }
            sb.Append("]");
        }
        else if (value.GetType().IsClass && value.GetType() != typeof(string))
        {
            sb.Append(ToJsonWithNaming(value, prettyPrint));
        }
        else
        {
            sb.Append(Convert.ToString(value).ToLower());
        }
    }

    #endregion
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonPropertyAttribute : Attribute
{
    public string PropertyName { get; }

    public JsonPropertyAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }
}
