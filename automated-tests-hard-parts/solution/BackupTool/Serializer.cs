namespace BackupTool;

public static class Serializer
{
    public static byte[] Serialize(object obj)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
    }

    public static T Deserialize<T>(byte[] json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
