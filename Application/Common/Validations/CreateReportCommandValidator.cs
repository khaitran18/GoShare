using Application.UseCase.ReportUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
    {
        public CreateReportCommandValidator()
        {
            RuleFor(x => x.TripId)
                .NotEmpty().WithMessage("Mã chuyến đi không được trống.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được trống.")
                .Length(1, 100).WithMessage("Tiêu đề phải từ 1 đến 100 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được trống.")
                .Length(1, 500).WithMessage("Mô tả phải từ 1 đến 500 ký tự.");
        }
    }
}
