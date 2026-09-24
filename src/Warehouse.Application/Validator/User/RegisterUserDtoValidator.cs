using FluentValidation;
using Warehouse.Application.DTO.User;

namespace Warehouse.Application.Validator.User;

public class RegisterUserDtoValidator
    : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .WithErrorCode("UsernameRequired")
            .MinimumLength(3)
            .WithMessage("Username must contain at least 3 characters.")
            .WithErrorCode("UsernameTooShort")
            .MaximumLength(50)
            .WithMessage("Username cannot contain more than 50 characters.")
            .WithErrorCode("UsernameTooLong");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .WithErrorCode("PasswordRequired")
            .MinimumLength(5)
            .WithMessage("Password must contain at least 5 characters.")
            .WithErrorCode("PasswordTooShort");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .WithErrorCode("EmailRequired")
            .EmailAddress()
            .WithMessage("Invalid email address.")
            .WithErrorCode("EmailInvalid");
    }
}
