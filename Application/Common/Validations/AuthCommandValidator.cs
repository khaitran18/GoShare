using Application.UseCase.AuthUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class AuthCommandValidator : AbstractValidator<AuthCommand>
    {
        public AuthCommandValidator()
        {
            RuleFor(x => x.Phone)
                .Matches("\\+84\\d{9}").WithMessage("Số điện thoại không đúng định dạng");
            RuleFor(x => x.Passcode)
                .Matches("^\\d{6}$").WithMessage("Passcode sai định dạng");
        }
    }
}
