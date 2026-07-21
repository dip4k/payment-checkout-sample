using CheckoutSystem.Application.Abstractions.Validation;
using CheckoutSystem.Application.Models;
using CheckoutSystem.Domain;

namespace CheckoutSystem.Application.Validation;

/// <summary>
/// Validates order calculation requests.
/// </summary>
public sealed class OrderCalculationRequestValidator : IValidator<OrderCalculationRequestModel>
{
    /// <inheritdoc/>
    public ValidationResult Validate(OrderCalculationRequestModel model)
    {
        var result = new ValidationResult();

        if (model.LineItems.Count == 0)
        {
            result.AddError("line_items_required", "lineItems", "At least one order line item is required.");
        }

        var index = 0;
        foreach (var lineItem in model.LineItems)
        {
            if (lineItem.ProductId == Guid.Empty)
            {
                result.AddError("product_id_required", $"lineItems[{index}].productId", "Product id is required.");
            }

            if (lineItem.Quantity <= 0)
            {
                result.AddError("quantity_positive", $"lineItems[{index}].quantity", "Quantity must be greater than zero.");
            }

            index++;
        }

        if (model.Discount is null)
        {
            return result;
        }

        if (model.Discount.Value < 0m)
        {
            result.AddError("discount_non_negative", "discount.value", "Discount value cannot be negative.");
        }

        if (model.Discount.Type == DiscountType.Percentage && model.Discount.Value > 100m)
        {
            result.AddError("discount_percentage_range", "discount.value", "Percentage discount must be between 0 and 100.");
        }

        if (model.Discount.Type == DiscountType.None && model.Discount.Value != 0m)
        {
            result.AddError("discount_none_value", "discount.value", "Discount value must be zero when discount type is None.");
        }

        return result;
    }
}