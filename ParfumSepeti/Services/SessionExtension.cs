using System.Text.Json;

namespace ParfumSepeti.Services;

public static class SessionExtension
{
    public static T? Get<T>(this ISession session, string key)
    {
        var str = session.GetString(key);

        if (string.IsNullOrWhiteSpace(str))
            return default;

        return JsonSerializer.Deserialize<T>(str);
    }
    
    public static void Set<T>(this ISession session, string key, T obj)
    {
        var str = JsonSerializer.Serialize(obj);
        session.SetString(key, str);
    }
}
