using System.Text.Json;

namespace TonedChat.Web.Utils;

public static class TautSerializer
{

    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public static string Serialize<T>(T o)
    {
        return JsonSerializer.Serialize(o, _options);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
    
    
}