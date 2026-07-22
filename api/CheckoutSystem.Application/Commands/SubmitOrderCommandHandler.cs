using CheckoutSystem.Application.Abstractions.Commands;
using CheckoutSystem.Application.Abstractions.Events;
using CheckoutSystem.Application.Abstractions.Persistence;
using CheckoutSystem.Application.Abstractions.Services;
using CheckoutSystem.Application.Events;
using CheckoutSystem.Application.Exceptions;
using CheckoutSystem.Application.Models;
using CheckoutSystem.Domain;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CheckoutSystem.Application.Commands;

/// <summary>
/// Handles submit-order command execution.
/// </summary>
public sealed class SubmitOrderCommandHandler : ISubmitOrderCommandHandler
{
    private const string OperationName = "orders-submit-v1";
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly ICheckoutService _checkoutService;
    private readonly IProductRepository _productRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitOrderCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="idempotencyRepository">The idempotency repository.</param>
    /// <param name="checkoutService">The checkout service.</param>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="domainEventDispatcher">The domain event dispatcher.</param>
    public SubmitOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IIdempotencyRepository idempotencyRepository,
        ICheckoutService checkoutService,
        IProductRepository productRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _unitOfWork = unitOfWork;
        _idempotencyRepository = idempotencyRepository;
        _checkoutService = checkoutService;
        _productRepository = productRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    /// <inheritdoc/>
    public async Task<OrderSubmissionResultModel> HandleAsync(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var requestHash = ComputeRequestHash(command.Request);
            var existing = await _idempotencyRepository.GetAsync(command.IdempotencyKey, OperationName, ct);
            if (existing is not null)
            {
                if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    throw new IdempotencyConflictException("Idempotency-Key was already used with a different request payload.");
                }

                var replay = new OrderCalculationResultModel(
                    existing.Subtotal,
                    existing.DiscountApplied,
                    existing.Tax,
                    existing.Total,
                    CalculateSplitShares(existing.Total));
                return new OrderSubmissionResultModel(replay, true);
            }

            var totals = await _checkoutService.SubmitOrderAsync(command.Request, ct);
            var productIds = command.Request.LineItems.Select(line => line.ProductId).Distinct().ToArray();
            var products = await _productRepository.GetByIdsAsync(productIds, ct);
            var productLookup = products.ToDictionary(product => product.Id);

            var lineSnapshots = command.Request.LineItems
                .Select(line =>
                {
                    var product = productLookup[line.ProductId];
                    return new OrderLineSnapshotModel(
                        line.ProductId,
                        product.Name,
                        product.UnitPrice,
                        product.IsTaxable,
                        line.Quantity,
                        product.Version);
                })
                .ToArray();

            var orderId = Guid.NewGuid();
            await _domainEventDispatcher.DispatchAsync(
                new OrderSubmittedDomainEvent(
                    orderId,
                    command.IdempotencyKey,
                    command.CorrelationId,
                    totals.Subtotal,
                    totals.DiscountApplied,
                    totals.Tax,
                    totals.Total,
                    lineSnapshots),
                ct);

            var saveResult = await _idempotencyRepository.SaveIfAbsentAsync(
                new IdempotencyResultModel(
                    command.IdempotencyKey,
                    OperationName,
                    requestHash,
                    totals.Subtotal,
                    totals.DiscountApplied,
                    totals.Tax,
                    totals.Total),
                ct);

            if (saveResult.Status == IdempotencySaveStatus.ExistingDifferentRequest)
            {
                throw new IdempotencyConflictException("Idempotency-Key was already used with a different request payload.");
            }

            if (saveResult.Status == IdempotencySaveStatus.ExistingSameRequest && saveResult.ExistingRecord is not null)
            {
                var replay = new OrderCalculationResultModel(
                    saveResult.ExistingRecord.Subtotal,
                    saveResult.ExistingRecord.DiscountApplied,
                    saveResult.ExistingRecord.Tax,
                    saveResult.ExistingRecord.Total,
                    CalculateSplitShares(saveResult.ExistingRecord.Total));
                return new OrderSubmissionResultModel(replay, true);
            }

            return new OrderSubmissionResultModel(totals, false);
        }, cancellationToken);
    }

    private static string ComputeRequestHash(OrderCalculationRequestModel request)
    {
        var payload = JsonSerializer.Serialize(request);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes);
    }

    private static IReadOnlyCollection<OrderSplitShareModel> CalculateSplitShares(decimal total)
    {
        var totalWholeUnits = decimal.ToInt32(Math.Round(total, 0, MidpointRounding.ToEven));
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