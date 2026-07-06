namespace Jartisan.Domain.Entities;

public record MakeOptions(
    bool IsCrud = false,
    bool IsForce = false,
    IReadOnlyDictionary<string, string>? ExtraFlags = null
);
