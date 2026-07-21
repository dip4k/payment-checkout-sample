using CheckoutSystem.Application.Abstractions.Persistence;
using CheckoutSystem.Application.Abstractions.Services;
using CheckoutSystem.Application.Abstractions.Validation;
using CheckoutSystem.Application.Exceptions;
using CheckoutSystem.Application.Models;
using CheckoutSystem.Domain;
using Microsoft.Extensions.Logging;

namespace CheckoutSystem.Application.Services;

/// <summary>
/// Implements checkout application operations.
/// </summary>
public sealed class CheckoutService : ICheckoutService
{
    private const decimal StandardTaxRate = 0.20m;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<OrderCalculationRequestModel> _validator;
    private readonly ILogger<CheckoutService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutService"/> class.
    /// </summary>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="validator">The request validator.</param>
    public CheckoutService(
        IProductRepository productRepository,
        IValidator<OrderCalculationRequestModel> validator,
        ILogger<CheckoutService> logger)
    {
        _productRepository = productRepository;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<CatalogueItemModel>> GetCatalogueAsync(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products
            .Select(product => new CatalogueItemModel(product.Id, product.Name, product.UnitPrice, product.IsTaxable, product.Version))
            .ToArray();
    }

    /// <inheritdoc/>
    public async Task<OrderCalculationResultModel> CalculateOrderAsync(OrderCalculationRequestModel request, CancellationToken cancellationToken)
    {
        return await CalculateInternalAsync(request, cancellationToken, enforceConcurrency: false);
    }

    /// <inheritdoc/>
    public async Task<OrderCalculationResultModel> SubmitOrderAsync(OrderCalculationRequestModel request, CancellationToken cancellationToken)
    {
        return await CalculateInternalAsync(request, cancellationToken, enforceConcurrency: true);
    }

    private async Task<OrderCalculationResultModel> CalculateInternalAsync(
        OrderCalculationRequestModel request,
        CancellationToken cancellationToken,
        bool enforceConcurrency)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var requestedProductIds = request.LineItems
            .Select(line => line.ProductId)
            .Distinct()
            .ToArray();

        var products = await _productRepository.GetByIdsAsync(requestedProductIds, cancellationToken);
        var productLookup = products.ToDictionary(product => product.Id);

        var missingProductIds = requestedProductIds.Where(id => !productLookup.ContainsKey(id)).ToArray();
        if (missingProductIds.Length > 0)
        {
            throw new NotFoundException($"Products not found: {string.Join(", ", missingProductIds)}");
        }

        if (enforceConcurrency)
        {
            foreach (var lineItem in request.LineItems)
            {
                if (!lineItem.ProductVersion.HasValue)
                {
                    throw new ConcurrencyConflictException($"Missing productVersion for product {lineItem.ProductId}.");
                }

                var currentVersion = productLookup[lineItem.ProductId].Version;
                if (lineItem.ProductVersion.Value != currentVersion)
                {
                    throw new ConcurrencyConflictException(
                        $"Product {lineItem.ProductId} is stale. Expected version {currentVersion}, received {lineItem.ProductVersion.Value}.");
                }
            }
        }

        var subtotal = request.LineItems.Sum(line => productLookup[line.ProductId].UnitPrice * line.Quantity);
        var discountAmount = CalculateDiscountAmount(subtotal, request.Discount);

        var taxableSubtotal = request.LineItems
            .Where(line => productLookup[line.ProductId].IsTaxable)
            .Sum(line => productLookup[line.ProductId].UnitPrice * line.Quantity);

        var taxableAfterDiscount = subtotal == 0m
            ? 0m
            : taxableSubtotal - (discountAmount * (taxableSubtotal / subtotal));

        if (taxableAfterDiscount < 0m)
        {
            taxableAfterDiscount = 0m;
        }

        var tax = taxableAfterDiscount * StandardTaxRate;
        var total = subtotal - discountAmount + tax;

        _logger.LogInformation(
            "Order totals calculated. Subtotal {Subtotal}, Discount {DiscountApplied}, Tax {Tax}, Total {Total}, ConcurrencyEnforced {ConcurrencyEnforced}",
            subtotal,
            discountAmount,
            tax,
            total,
            enforceConcurrency);

        var roundedSubtotal = RoundCurrency(subtotal);
        var roundedDiscount = RoundCurrency(discountAmount);
        var roundedTax = RoundCurrency(tax);
        var roundedTotal = RoundCurrency(total);

        return new OrderCalculationResultModel(
            roundedSubtotal,
            roundedDiscount,
            roundedTax,
            roundedTotal,
            CalculateSplitShares(roundedTotal));
    }

    private static decimal CalculateDiscountAmount(decimal subtotal, DiscountModel? discount)
    {
        if (discount is null || discount.Type == DiscountType.None || discount.Value == 0m)
        {
            return 0m;
        }

        var rawDiscount = discount.Type switch
        {
            DiscountType.Percentage => subtotal * (discount.Value / 100m),
            DiscountType.FixedAmount => discount.Value,
            _ => 0m,
        };

        if (rawDiscount < 0m)
        {
            return 0m;
        }

        return rawDiscount > subtotal ? subtotal : rawDiscount;
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static IReadOnlyCollection<OrderSplitShareModel> CalculateSplitShares(decimal total)
    {
        var totalWholeUnits = decimal.ToInt32(Math.Round(total, 0, MidpointRounding.AwayFromZero));
        var baseShareWholeUnits = totalWholeUnits / 3;
        var remainderWholeUnits = totalWholeUnits % 3;

        return new[]
        {
            new OrderSplitShareModel(1, baseShareWholeUnits + remainderWholeUnits),
            new OrderSplitShareModel(2, baseShareWholeUnits),
            new OrderSplitShareModel(3, baseShareWholeUnits),
        };
    }
}