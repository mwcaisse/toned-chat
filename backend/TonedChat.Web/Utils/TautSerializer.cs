using System.Text.Json;
using System.Text.Json.Serialization;
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
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        options.SerializerOptions.AllowOutOfOrderMetadataProperties = true;
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