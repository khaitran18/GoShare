using Application.Queries;
using FluentValidation;

namespace Application.Common.Validations
{
    public class TestQueryValidator : AbstractValidator<TestQuery>
    {
        public TestQueryValidator()
        {
            RuleFor(query => query.phone)
                .NotEmpty().WithMessage("Phone number must not be empty")
                .MinimumLength(10).WithMessage("Phone number must be at least 10 numbers")
                .NotNull().WithErrorCode("400").WithMessage("Cant detect phone number field");
        }
    }
}
