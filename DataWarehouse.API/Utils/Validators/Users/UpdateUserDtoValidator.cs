using FluentValidation;
using DataWarehouse.Models.DTOs;

namespace DataWarehouse.API.Utils.Validators.Users
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.LastName));

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

            RuleFor(x => x.NewPassword)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword))
                .WithMessage("New password must be at least 6 characters.");

            RuleFor(x => x.Role)
                .Must(role => new[] { "Owner", "Admin", "User" }.Contains(role))
                .When(x => !string.IsNullOrWhiteSpace(x.Role))
                .WithMessage("Role must be one of: Owner, Admin, User.");
        }
    }
}