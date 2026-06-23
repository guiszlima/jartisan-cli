using System.Text.Json.Serialization;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Services;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FolderMap))]
public partial class JartisanJsonContext : JsonSerializerContext
{
}