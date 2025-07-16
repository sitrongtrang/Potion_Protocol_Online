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
        // First try our enhanced parser
        try
        {
            var result = FromJsonCaseInsensitive<T>(json);
            if (result != null) return result;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Enhanced JSON parsing failed, falling back to JsonUtility: {e.Message}");
        }

        // Fallback to standard JsonUtility
        return JsonUtility.FromJson<T>(json);
    }

    private static T FromJsonCaseInsensitive<T>(string json) where T : new()
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        var jsonDict = ParseToDictionary(json);
        var result = new T();
        var type = typeof(T);

        // Handle fields
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            ProcessMember(jsonDict, result, field.Name, field.FieldType, 
                (obj, val) => field.SetValue(obj, val));
        }

        // Handle properties
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (prop.CanWrite)
            {
                ProcessMember(jsonDict, result, prop.Name, prop.PropertyType, 
                    (obj, val) => prop.SetValue(obj, val, null));
            }
        }

        return result;
    }

    private static void ProcessMember<T>(Dictionary<string, object> jsonDict, T obj, 
        string memberName, Type memberType, Action<T, object> setter)
    {
        // Check for JsonProperty attribute
        var memberInfo = obj.GetType().GetMember(memberName)[0];
        var jsonProp = memberInfo.GetCustomAttribute<JsonPropertyAttribute>();

        string jsonKey = jsonProp?.PropertyName ?? memberName;

        if (jsonDict.TryGetValue(jsonKey, out object value))
        {
            try
            {
                object convertedValue = ConvertValue(value, memberType);
                setter(obj, convertedValue);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to set {memberName}: {e.Message}");
            }
        }
    }

    #endregion

    #region Enhanced Serialization

    public static string ToJson(object obj, bool prettyPrint = false)
    {
        // First try our enhanced serializer
        try
        {
            return ToJsonWithNaming(obj, prettyPrint);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Enhanced JSON serialization failed, falling back to JsonUtility: {e.Message}");
        }

        // Fallback to standard JsonUtility
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    private static string ToJsonWithNaming(object obj, bool prettyPrint)
    {
        var type = obj.GetType();
        var sb = new StringBuilder();
        sb.Append("{");

        bool first = true;
        string newLine = prettyPrint ? "\n" : "";
        string indent = prettyPrint ? "  " : "";

        // Handle fields
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!first) sb.Append(",");
            first = false;

            if (prettyPrint) sb.Append(newLine + indent);

            string jsonName = GetJsonName(field);
            object value = field.GetValue(obj);

            sb.Append($"\"{jsonName}\":");
            AppendJsonValue(sb, value, prettyPrint, indent);
        }

        // Handle properties
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (prop.CanRead && prop.CanWrite)
            {
                if (!first) sb.Append(",");
                first = false;

                if (prettyPrint) sb.Append(newLine + indent);

                string jsonName = GetJsonName(prop);
                object value = prop.GetValue(obj);

                sb.Append($"\"{jsonName}\":");
                AppendJsonValue(sb, value, prettyPrint, indent);
            }
        }

        if (prettyPrint) sb.Append(newLine);
        sb.Append("}");
        return sb.ToString();
    }

    private static string GetJsonName(MemberInfo member)
    {
        var jsonProp = member.GetCustomAttribute<JsonPropertyAttribute>();
        return jsonProp?.PropertyName ?? member.Name;
    }

    #endregion

    #region Core Utilities

    private static Dictionary<string, object> ParseToDictionary(string json)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        
        // Simple JSON parser - for more complex cases you might want to use JsonUtility
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