using System.Text.Json.Serialization;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Implementations.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

[JsonSerializable(typeof(FolderMap))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class JartisanJsonContext : JsonSerializerContext
{
    
}