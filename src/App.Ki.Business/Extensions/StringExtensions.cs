using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.Ki.Business.Extensions;

// build
public static class StringExtensions
{
    private const string EmptyObject = "{}";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConcurrentDictionary<int, string>>>
        Cache = new();

    public static string ToJson(this object data)
    {
        return data is null ? EmptyObject : JsonSerializer.Serialize(data, JsonOptions);
    }

    public static T FromJson<T>(this string data)
    {
        return JsonSerializer.Deserialize<T>(string.IsNullOrWhiteSpace(data) ? EmptyObject : data, JsonOptions);
    }

    public static T FromJsonBase64<T>(this string data)
    {
        if (!string.IsNullOrWhiteSpace(data) && data.FromBase64(out var bytes))
            return JsonSerializer.Deserialize<T>(bytes, JsonOptions);

        return JsonSerializer.Deserialize<T>(EmptyObject, JsonOptions);
    }

    public static byte[] ToBytes(this string text)
        => Encoding.UTF8.GetBytes(text);

    public static string FromBytes(this byte[] buffer)
        => Encoding.UTF8.GetString(buffer);

    public static string ToBase64(this byte[] buffer)
        => Convert.ToBase64String(buffer);

    public static bool FromBase64(this string text, out byte[] data)
    {
        try
        {
            text = text.PadRight(text.Length + (4 - text.Length % 4) % 4, '=');
            data = Convert.FromBase64String(text);
            return true;
        }
        catch (Exception)
        {
            data = text.ToBytes();
            return false;
        }
    }

    public static string EnumToString(this Enum @enum)
    {
        return @enum.EnumToString<DescriptionAttribute>(e => e.Description);
    }

    public static string EnumToString<T>(this Enum @enum, Func<T, string> getter) where T : Attribute
    {
        var enumType = @enum.GetType();
        var attrType = typeof(T);
        var key = Convert.ToInt32(@enum);

        if (Cache.ContainsKey(enumType) &&
            Cache[enumType].ContainsKey(attrType) &&
            Cache[enumType][attrType].ContainsKey(key))
        {
            return Cache[enumType][attrType][key];
        }

        Cache.TryAdd(enumType, new ConcurrentDictionary<Type, ConcurrentDictionary<int, string>>());
        Cache[enumType].TryAdd(attrType, new ConcurrentDictionary<int, string>());

        var values = Enum.GetValues(enumType);
        foreach (var value in values)
        {
            var memberInfo = enumType.GetMember(value.ToString() ?? string.Empty);
            var attributes = memberInfo.First().GetCustomAttribute<T>();

            Cache[enumType][attrType].TryAdd(
                Convert.ToInt32(value),
                attributes is not null ? getter(attributes) : value.ToString());
        }

        return @enum.EnumToString(getter);
    }
    
    public static T Coalesce<T>(this T checking, T @default) where T : class
        => checking ?? @default;


    public static string Numbers(this string text)
        => string.IsNullOrWhiteSpace(text) ? text : new string(text.Where(char.IsDigit).ToArray());

    public static string MaskText(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        if (text.Length <= 10)
            return string.Join(string.Empty, Enumerable.Repeat("*", text.Length));

        var result = $"{text[..6]}{string.Join("", Enumerable.Repeat('*', text.Length - 10))}{text[^4..]}";
        return result;
    }
}