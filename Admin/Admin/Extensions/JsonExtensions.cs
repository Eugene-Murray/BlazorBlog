using System.Text.Json;

namespace Admin.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public static string ToJson(this object obj)
            => JsonSerializer.Serialize(obj, Options);
    }
}
