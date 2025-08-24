namespace CW.Core;

public static class JsonSerializationSettings
{
    public static readonly System.Text.Json.JsonSerializerOptions Instance = new(System.Text.Json.JsonSerializerDefaults.Web);
}
