using Application.UseCase.PaymentUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CreateTopUpRequestValidator : AbstractValidator<CreateTopUpRequestCommand>
    {
        public CreateTopUpRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(10000).WithMessage("Số tiền nạp vào ví phải nhiều hơn 10000");
        }
    }
}
