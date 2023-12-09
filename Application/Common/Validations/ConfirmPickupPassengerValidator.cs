using Application.UseCase.DriverUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class ConfirmPickupPassengerValidator : AbstractValidator<ConfirmPickupPassengerCommand>
    {
        public ConfirmPickupPassengerValidator()
        {
            RuleFor(x => x.DriverLatitude).InclusiveBetween(-90, 90).WithMessage("Vĩ độ tài xế không hợp lệ.");
            RuleFor(x => x.DriverLongitude).InclusiveBetween(-180, 180).WithMessage("Kinh độ tài xế không hợp lệ.");
        }
    }
}
