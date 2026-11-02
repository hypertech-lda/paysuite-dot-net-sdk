using PaySuite.Sdk.Internal;
using PaySuite.Sdk.Models;

namespace PaySuite.Sdk.Resources;

public sealed class PaymentLinksResource
{
    private const string Path = "payment-links";
    private readonly ApiClient _api;

    internal PaymentLinksResource(ApiClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Cria um link de pagamento. Requer a ability payment-link:write.
    /// </summary>
    public Task<ApiResponse<PaymentLink>> CreateAsync(
        CreatePaymentLinkRequest request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guard.NotEmpty(request.Title, nameof(request.Title));
        ValidateAmountMode(request.AmountMode, request.Amount, amountRequiredIfFixed: true);

        if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            Guard.Url(request.ReturnUrl, nameof(request.ReturnUrl));

        return _api.GetDataAsync<PaymentLink>(HttpMethod.Post, Path, request, ct);
    }

    /// <summary>
    /// Obtém um link de pagamento pelo ID (ULID). Requer payment-link:read.
    /// </summary>
    public Task<ApiResponse<PaymentLink>> GetAsync(
        string id,
        CancellationToken ct = default)
    {
        return _api.GetDataAsync<PaymentLink>(
            HttpMethod.Get,
            $"{Path}/{Guard.Id(id)}",
            ct: ct);
    }

    /// <summary>
    /// Lista paginada de links de pagamento. Requer payment-link:read.
    /// </summary>
    public Task<PagedResponse<PaymentLink>> ListAsync(
        int page = 1,
        int limit = 20,
        CancellationToken ct = default)
    {
        Guard.Range(page, 1, int.MaxValue, nameof(page));
        Guard.Range(limit, 1, 100, nameof(limit));

        return _api.SendAsync<PagedResponse<PaymentLink>>(
            HttpMethod.Get,
            $"{Path}?page={page}&limit={limit}",
            ct: ct);
    }

    /// <summary>
    /// Actualiza um link de pagamento (PATCH). Requer payment-link:write.
    /// </summary>
    public Task<ApiResponse<PaymentLink>> UpdateAsync(
        string id,
        UpdatePaymentLinkRequest request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Title is not null)
            Guard.NotEmpty(request.Title, nameof(request.Title));

        if (request.AmountMode is not null)
            ValidateAmountMode(request.AmountMode, request.Amount, amountRequiredIfFixed: false);

        if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            Guard.Url(request.ReturnUrl, nameof(request.ReturnUrl));

        return _api.GetDataAsync<PaymentLink>(
            HttpMethod.Patch,
            $"{Path}/{Guard.Id(id)}",
            request,
            ct);
    }

    private static void ValidateAmountMode(string? mode, decimal? amount, bool amountRequiredIfFixed)
    {
        if (!PaymentLinkAmountModes.IsValid(mode))
            throw new ArgumentException(
                "amount_mode deve ser 'fixed' ou 'open'.", nameof(mode));

        if (mode == PaymentLinkAmountModes.Fixed && amountRequiredIfFixed && amount is null)
            throw new ArgumentException(
                "O valor (Amount) é obrigatório quando AmountMode é 'fixed'.", nameof(amount));

        if (amount is not null)
            Guard.Range(amount.Value, 0.01m, 1_000_000m, nameof(amount));
    }
}