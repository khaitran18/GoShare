using Application.UseCase.WalletUC.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class UpdateWalletBalanceCommandValidator : AbstractValidator<UpdateWalletBalanceCommand>
    {
        public UpdateWalletBalanceCommandValidator()
        {
            RuleFor(x => x.Balance)
                .NotNull().WithMessage("Số dư không được để trống.")
                .GreaterThanOrEqualTo(0).WithMessage("Số dư phải lớn hơn hoặc bằng 0.");
        }
    }
}
