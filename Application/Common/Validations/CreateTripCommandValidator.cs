using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCase.TripUC.Commands;
using FluentValidation;

namespace Application.Common.Validations
{
    public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
    {
        public CreateTripCommandValidator()
        {
            RuleFor(x => x.StartLatitude).InclusiveBetween(-90, 90).WithMessage("Vĩ độ bắt đầu không hợp lệ.");
            RuleFor(x => x.StartLongitude).InclusiveBetween(-180, 180).WithMessage("Kinh độ bắt đầu không hợp lệ.");

            RuleFor(x => x.EndLatitude).InclusiveBetween(-90, 90).WithMessage("Vĩ độ kết thúc không hợp lệ.");
            RuleFor(x => x.EndLongitude).InclusiveBetween(-180, 180).WithMessage("Kinh độ kết thúc không hợp lệ.");

            RuleFor(x => x.CartypeId).NotEmpty().WithMessage("ID loại xe không được để trống.");
            RuleFor(x => x.PaymentMethod).NotNull().WithMessage("Phương thức thanh toán không được để trống.");
        }
    }
}
