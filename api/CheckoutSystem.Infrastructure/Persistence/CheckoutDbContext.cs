using CheckoutSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents the EF Core database context for checkout persistence.
/// </summary>
public sealed class CheckoutDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutDbContext"/> class.
    /// </summary>
    /// <param name="options">The context options.</param>
    public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the products catalogue set.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets the idempotency records set.
    /// </summary>
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    /// <summary>
    /// Gets submitted order records.
    /// </summary>
    public DbSet<OrderRecord> Orders => Set<OrderRecord>();

    /// <summary>
    /// Gets immutable line snapshots.
    /// </summary>
    public DbSet<OrderLineSnapshotRecord> OrderLineSnapshots => Set<OrderLineSnapshotRecord>();

    /// <summary>
    /// Gets append-only order status history records.
    /// </summary>
    public DbSet<OrderStatusHistoryRecord> OrderStatusHistory => Set<OrderStatusHistoryRecord>();

    /// <summary>
    /// Gets outbox message records.
    /// </summary>
    public DbSet<OutboxMessageRecord> OutboxMessages => Set<OutboxMessageRecord>();

    /// <inheritdoc/>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyProductVersionIncrement();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override int SaveChanges()
    {
        ApplyProductVersionIncrement();
        return base.SaveChanges();
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.UnitPrice).HasPrecision(18, 2);
            entity.Property(product => product.IsTaxable).IsRequired();
            entity.Property(product => product.Version).HasDefaultValue(1);
            entity.Property(product => product.Version).IsConcurrencyToken();
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.ToTable("IdempotencyRecords");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Key).HasMaxLength(200).IsRequired();
            entity.Property(record => record.OperationName).HasMaxLength(100).IsRequired();
            entity.Property(record => record.RequestHash).HasMaxLength(128).IsRequired();
            entity.Property(record => record.Subtotal).HasPrecision(18, 2);
            entity.Property(record => record.DiscountApplied).HasPrecision(18, 2);
            entity.Property(record => record.Tax).HasPrecision(18, 2);
            entity.Property(record => record.Total).HasPrecision(18, 2);
            entity.Property(record => record.CreatedAtUtc).IsRequired();
            entity.HasIndex(record => new { record.Key, record.OperationName }).IsUnique();
        });

        modelBuilder.Entity<OrderRecord>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.IdempotencyKey).HasMaxLength(200).IsRequired();
            entity.Property(order => order.CorrelationId).HasMaxLength(100).IsRequired();
            entity.Property(order => order.Subtotal).HasPrecision(18, 2);
            entity.Property(order => order.DiscountApplied).HasPrecision(18, 2);
            entity.Property(order => order.Tax).HasPrecision(18, 2);
            entity.Property(order => order.Total).HasPrecision(18, 2);
            entity.Property(order => order.CreatedAtUtc).IsRequired();
            entity.HasIndex(order => order.IdempotencyKey).IsUnique();
            entity.HasMany(order => order.LineSnapshots)
                .WithOne(snapshot => snapshot.Order)
                .HasForeignKey(snapshot => snapshot.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(order => order.StatusHistory)
                .WithOne(history => history.Order)
                .HasForeignKey(history => history.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderLineSnapshotRecord>(entity =>
        {
            entity.ToTable("OrderLineSnapshots");
            entity.HasKey(snapshot => snapshot.Id);
            entity.Property(snapshot => snapshot.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(snapshot => snapshot.UnitPrice).HasPrecision(18, 2);
            entity.Property(snapshot => snapshot.IsTaxable).IsRequired();
            entity.Property(snapshot => snapshot.Quantity).IsRequired();
            entity.Property(snapshot => snapshot.ProductVersion).IsRequired();
        });

        modelBuilder.Entity<OrderStatusHistoryRecord>(entity =>
        {
            entity.ToTable("OrderStatusHistory");
            entity.HasKey(history => history.Id);
            entity.Property(history => history.Status).HasMaxLength(100).IsRequired();
            entity.Property(history => history.ChangedAtUtc).IsRequired();
            entity.Property(history => history.Note).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<OutboxMessageRecord>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(message => message.Id);
            entity.Property(message => message.EventType).HasMaxLength(200).IsRequired();
            entity.Property(message => message.AggregateId).HasMaxLength(100).IsRequired();
            entity.Property(message => message.Payload).IsRequired();
            entity.Property(message => message.CreatedAtUtc).IsRequired();
            entity.Property(message => message.ProcessedAtUtc).IsRequired(false);
            entity.HasIndex(message => message.ProcessedAtUtc);
        });

        modelBuilder.Entity<Product>().HasData(
            new Product(new Guid("11111111-1111-1111-1111-111111111111"), "Hot Coffee", 3.50m, true, 1),
            new Product(new Guid("22222222-2222-2222-2222-222222222222"), "Cold Sandwich", 5.25m, false, 1),
            new Product(new Guid("33333333-3333-3333-3333-333333333333"), "Chocolate Cake", 4.75m, true, 1));
    }

    private void ApplyProductVersionIncrement()
    {
        var productEntries = ChangeTracker
            .Entries<Product>()
            .Where(entry => entry.State == EntityState.Modified);

        foreach (var productEntry in productEntries)
        {
            var originalVersion = productEntry.OriginalValues.GetValue<long>(nameof(Product.Version));
            productEntry.Property(nameof(Product.Version)).CurrentValue = originalVersion + 1;
        }
    }
}