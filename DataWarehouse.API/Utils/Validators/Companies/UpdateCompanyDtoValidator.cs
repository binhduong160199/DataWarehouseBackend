using FluentValidation;
using DataWarehouse.Models.DTOs;

namespace DataWarehouse.API.Utils.Validators.Companies
{
    public class UpdateCompanyDtoValidator : AbstractValidator<UpdateCompanyDto>
    {
        public UpdateCompanyDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Company name must be at most 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.ContactEmail)
                .EmailAddress().WithMessage("Contact email must be a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must be at most 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Website)
                .MaximumLength(100).WithMessage("Website URL must be at most 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Website));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address must be at most 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.Industry)
                .MaximumLength(100).WithMessage("Industry must be at most 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Industry));

            RuleFor(x => x.TaxId)
                .MaximumLength(50).WithMessage("Tax ID must be at most 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.TaxId));

            RuleFor(x => x.LogoUrl)
                .MaximumLength(250).WithMessage("Logo URL must be at most 250 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
        }
    }
}