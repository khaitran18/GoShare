using Application.Common.Utilities;
using Application.UseCase.AuthUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Phone)
                .Matches("\\+84\\d{9}").WithMessage("Số điện thoại không đúng định dạng");
            RuleFor(x => x.Gender).InclusiveBetween((short)0, (short)1).WithMessage("Giới tính không đúng định dạng");
            RuleFor(x => x.Name)
                .MinimumLength(4).WithMessage("Tên phải có ít nhất 4 ký tự")
                .MaximumLength(50).WithMessage("Tên của bạn quá dài");
        }
    }
}
