using FluentValidation;
using Warehouse.Application.DTO;

namespace Warehouse.Application.Validator;

public class AddLocationValidator : AbstractValidator<AddLocationDto>
{
    public AddLocationValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .WithErrorCode("CodeRequired")
            .Matches(@"^[A-Z]-\d{3}$")
            .WithMessage("Code must have the format A-000.")
            .WithErrorCode("IncorectCode");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty()
            .WithMessage("Warehouse Code is required,")
            .WithErrorCode("WarehouseCodeRequired")
            .Matches(@"WH-\d{2}$")
            .WithMessage("Warehouse Code must have the format WH-00.")
            .WithErrorCode("IncorectWarehouseCode");

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
