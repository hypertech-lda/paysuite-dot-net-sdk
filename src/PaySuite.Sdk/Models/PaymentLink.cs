namespace PaySuite.Sdk.Models;

public static class PaymentLinkAmountModes
{
    public const string Fixed = "fixed";
    public const string Open = "open";

    public static bool IsValid(string? mode) => mode is Fixed or Open;
}

public sealed record PaymentLink
{
    public string Id { get; init; } = "";
    public string Title { get; init; } = "";
    public string? Description { get; init; }
    public string AmountMode { get; init; } = "";
    public decimal? Amount { get; init; }
    public string Currency { get; init; } = "MZN";
    public string? Method { get; init; }
    public bool IsActive { get; init; }
    public string Url { get; init; } = "";
    public string? ReturnUrl { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
}

public sealed record CreatePaymentLinkRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string AmountMode { get; init; }   // fixed | open
    public decimal? Amount { get; init; }              // obrigatório se AmountMode == fixed
    public string? Method { get; init; }               // ex.: mpesa
    public string? ReturnUrl { get; init; }
    public bool? IsActive { get; init; }
}

/// <summary>Todos os campos são opcionais: só o que for preenchido é enviado.</summary>
public sealed record UpdatePaymentLinkRequest
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? AmountMode { get; init; }
    public decimal? Amount { get; init; }
    public string? Method { get; init; }
    public string? ReturnUrl { get; init; }
    public bool? IsActive { get; init; }
}