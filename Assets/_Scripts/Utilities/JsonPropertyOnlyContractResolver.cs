using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class JsonPropertyOnlyContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var allProps = base.CreateProperties(type, memberSerialization);

        return allProps.Where(p =>
        {
            // Try to find the corresponding member (field or property)
            var member = type.GetMember(p.UnderlyingName, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .FirstOrDefault();

            return member?.GetCustomAttribute<JsonPropertyAttribute>() != null;
        }).ToList();
    }
}
