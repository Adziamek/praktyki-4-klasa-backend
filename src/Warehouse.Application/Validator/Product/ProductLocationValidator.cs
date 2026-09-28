using FluentValidation;
using Warehouse.Application.DTO.Product;

namespace Warehouse.Application.Validator.Product;

public class ProductLocationValidator : AbstractValidator<ProductLocationDto>
{
    public ProductLocationValidator()
    {
        RuleFor(x => x.LocationId)
            .GreaterThan(0)
            .WithMessage("Location ID must be a positive integer.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative.");
    }
}