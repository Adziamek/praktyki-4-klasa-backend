using FluentValidation;
using Warehouse.Application.DTO.Product;

namespace Warehouse.Application.Validator.Product;

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Ean)
            .NotEmpty()
            .WithMessage("EAN is required.")
            .Matches(@"^\d{13}$")
            .WithMessage("EAN must contain exactly 13 digits.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required.")
            .GreaterThan(0)
            .WithMessage("Category ID must be a positive integer.");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .WithMessage("Brand id is required.")
            .GreaterThan(0)
            .WithMessage("Category ID must be a positive integer.");
    }
}