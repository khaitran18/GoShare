using Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CalculateFeesForTripValidator : AbstractValidator<CalculateFeesForTripCommand>
    {
        public CalculateFeesForTripValidator()
        {
            RuleFor(x => x.StartLatitude).InclusiveBetween(-90, 90).WithMessage("StartLatitude must be between -90 and 90.");
            RuleFor(x => x.StartLongitude).InclusiveBetween(-180, 180).WithMessage("StartLongitude must be between -180 and 180.");

            RuleFor(x => x.EndLatitude).InclusiveBetween(-90, 90).WithMessage("EndLatitude must be between -90 and 90.");
            RuleFor(x => x.EndLongitude).InclusiveBetween(-180, 180).WithMessage("EndLongitude must be between -180 and 180.");
        }
    }
}
