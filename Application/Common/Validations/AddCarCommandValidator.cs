using Application.UseCase.DriverUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class AddCarCommandValidator : AbstractValidator<AddCarCommand>
    {
        public AddCarCommandValidator()
        {
            RuleFor(x => x.Car.LicensePlate)
                .MaximumLength(9).WithMessage("Biển số xe phải có nhiều nhất 9 ký tự")
                .MinimumLength(8).WithMessage("Biển số xe phải có ít nhất 8 ký tự");
            RuleFor(x=>x.Car.Model)
                .MaximumLength(20).WithMessage("Dòng xe phải có nhiều nhất 20 ký tự")
                .MinimumLength(3).WithMessage("Dòng xe phải có ít nhất 3 ký tự");
            RuleFor(x=>x.Car.Make)
                .MaximumLength(20).WithMessage("Hãng sản xuất phải có nhiều nhất 20 ký tự")
                .MinimumLength(3).WithMessage("Hãng sản xuất phải có ít nhất 3 ký tự");
            RuleFor(x => x.Capacity)
                 .LessThanOrEqualTo((short)9).WithMessage("Số chỗ ngồi trên xe phải ít hơn hoặc bằng 9")
                 .GreaterThanOrEqualTo((short)2).WithMessage("Số chỗ ngồi trên xe phải nhiều hơn hoặc bằng 2");
        }
    }
}
