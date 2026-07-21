using CheckoutSystem.Application.Abstractions.Events;
using CheckoutSystem.Application.Events;
using CheckoutSystem.Infrastructure.Persistence;
using System.Text.Json;

namespace CheckoutSystem.Infrastructure.Events;

/// <summary>
/// Persists immutable order snapshots and append-only status history for submitted orders.
/// </summary>
public sealed class OrderSubmittedAuditHandler : IDomainEventHandler<OrderSubmittedDomainEvent>
{
    private readonly CheckoutDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderSubmittedAuditHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The db context.</param>
    public OrderSubmittedAuditHandler(CheckoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(OrderSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var orderRecord = new OrderRecord
        {
            Id = domainEvent.OrderId,
            IdempotencyKey = domainEvent.IdempotencyKey,
            CorrelationId = domainEvent.CorrelationId,
            Subtotal = domainEvent.Subtotal,
            DiscountApplied = domainEvent.DiscountApplied,
            Tax = domainEvent.Tax,
            Total = domainEvent.Total,
            CreatedAtUtc = domainEvent.OccurredAtUtc,
            LineSnapshots = domainEvent.LineSnapshots
                .Select(snapshot => new OrderLineSnapshotRecord
                {
                    Id = Guid.NewGuid(),
                    ProductId = snapshot.ProductId,
                    ProductName = snapshot.ProductName,
                    UnitPrice = snapshot.UnitPrice,
                    IsTaxable = snapshot.IsTaxable,
                    Quantity = snapshot.Quantity,
                    ProductVersion = snapshot.ProductVersion,
                })
                .ToList(),
            StatusHistory =
            [
                new OrderStatusHistoryRecord
                {
                    Id = Guid.NewGuid(),
                    Status = "Submitted",
                    ChangedAtUtc = domainEvent.OccurredAtUtc,
                    Note = "Order created.",
                },
            ],
        };

        _dbContext.Orders.Add(orderRecord);

        _dbContext.OutboxMessages.Add(new OutboxMessageRecord
        {
            Id = Guid.NewGuid(),
            EventType = nameof(OrderSubmittedDomainEvent),
            AggregateId = domainEvent.OrderId.ToString(),
            Payload = JsonSerializer.Serialize(domainEvent),
            CreatedAtUtc = domainEvent.OccurredAtUtc,
            ProcessedAtUtc = null,
        });

        await Task.CompletedTask;
    }
}