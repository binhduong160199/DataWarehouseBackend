using FluentValidation;
using DataWarehouse.Models.DTOs;

namespace DataWarehouse.API.Utils.Validators.Users
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.JobTitle)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.JobTitle));

            RuleFor(x => x.Department)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Department));

            RuleFor(x => x.ProfileImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl))
                .WithMessage("ProfileImageUrl must be a valid URL.");

            RuleFor(x => x.Role)
                .Must(role => new[] { "Owner", "Admin", "User" }.Contains(role))
                .WithMessage("Role must be one of: Owner, Admin, User.");
        }
    }
}