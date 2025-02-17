using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace TonedChat.Web.Utils;

public static class TautSerializer
{
    private static readonly JsonSerializerOptions Options;

    static TautSerializer()
    {
        var options = new JsonOptions();
        ApplyJsonSerializerOptions(options);
        Options = options.SerializerOptions;
    }

    public static void ApplyJsonSerializerOptions(JsonOptions options)
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }
    
    public static string Serialize<T>(T o)
    {
        return JsonSerializer.Serialize(o, Options);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}