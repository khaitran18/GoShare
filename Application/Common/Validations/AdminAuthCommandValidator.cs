using Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class AdminAuthCommandValidator : AbstractValidator<AdminAuthCommand>
    {
        public AdminAuthCommandValidator()
        {
            RuleFor(x=>x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(6);
        }
    }
}
