using Application.UseCase.TripUC.Commands;
using Domain.Enumerations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CreateTripForDependentCommandValidator : AbstractValidator<CreateTripForDependentCommand>
    {
        public CreateTripForDependentCommandValidator()
        {
            RuleFor(x => x.EndLatitude).InclusiveBetween(-90, 90).WithMessage("Vĩ độ kết thúc không hợp lệ.");
            RuleFor(x => x.EndLongitude).InclusiveBetween(-180, 180).WithMessage("Kinh độ kết thúc không hợp lệ.");

            RuleFor(x => x.CartypeId).NotEmpty().WithMessage("ID loại xe không được để trống.");
            RuleFor(x => x.PaymentMethod).NotNull().WithMessage("Phương thức thanh toán không được để trống.");
        }
    }
}
