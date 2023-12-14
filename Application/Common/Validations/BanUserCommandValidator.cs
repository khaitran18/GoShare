using Application.UseCase.UserUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class BanUserCommandValidator : AbstractValidator<BanUserCommand>
    {
        public BanUserCommandValidator()
        {
            RuleFor(x => x.DisabledReason)
                .NotEmpty().WithMessage("Lý do vô hiệu hóa không được để trống.")
                .Length(1, 500).WithMessage("Lý do vô hiệu hóa phải từ 1 đến 500 ký tự.");
        }
    }
}
