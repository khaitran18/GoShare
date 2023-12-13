using Application.UseCase.UserUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không được trống.")
                .MinimumLength(4).WithMessage("Tên phải có ít nhất 4 ký tự")
                .MaximumLength(50).WithMessage("Tên của bạn quá dài");

            RuleFor(x => x.Gender)
                .InclusiveBetween((short)0, (short)1).WithMessage("Giới tính không đúng định dạng");

            RuleFor(x => x.Birth)
                .NotEmpty().WithMessage("Ngày sinh không được trống.")
                .LessThan(DateTime.Now).WithMessage("Ngày sinh phải trước ngày hiện tại.");
        }
    }
}
