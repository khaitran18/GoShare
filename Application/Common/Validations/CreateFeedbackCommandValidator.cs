using Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
    {
        public CreateFeedbackCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tiêu đề không được để trống.");
            RuleFor(x => x.Title).MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được để trống.");
            RuleFor(x => x.Content).MaximumLength(1000).WithMessage("Nội dung phản hồi không được vượt quá 1000 ký tự.");
        }
    }
}
