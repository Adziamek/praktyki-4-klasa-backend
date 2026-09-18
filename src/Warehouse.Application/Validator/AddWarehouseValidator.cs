using FluentValidation;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Validator;

public class AddWarehouseValidator : AbstractValidator<AddWarehouseDto>
{
    public AddWarehouseValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .WithErrorCode("CodeRequired")
            .Matches(@"WH-\d{2}$")
            .WithMessage("Code must have the format WH-00.")
            .WithErrorCode("IncorectCode");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .WithErrorCode("NameRequired")
            .MinimumLength(3)
            .WithMessage("Name must contain at least 3 characters.")
            .WithErrorCode("NameTooShort")
            .MaximumLength(75)
            .WithMessage("Name cannot contain more than 50 characters.")
            .WithErrorCode("NameTooLong");
    }
}