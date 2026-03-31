using FluentValidation;
using projectBackend.Model.RequestModel;
using System.Text.RegularExpressions;

namespace projectBackend.Validators;

public class UserRequestModelValidator : AbstractValidator<UserRequestModel>
{
    public UserRequestModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required")
            .MinimumLength(2).WithMessage("Surname must be at least 2 characters")
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
            .Must(BeValidEmail).WithMessage("Invalid email format. Example: user@domain.com");

        RuleFor(x => x.Occupation)
            .NotEmpty().WithMessage("Occupation is required")
            .MaximumLength(100).WithMessage("Occupation cannot exceed 100 characters");

        RuleFor(x => x.Organization)
            .NotEmpty().WithMessage("Organization is required")
            .MaximumLength(200).WithMessage("Organization cannot exceed 200 characters");
    }

    private bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Strict email regex pattern
        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
}
