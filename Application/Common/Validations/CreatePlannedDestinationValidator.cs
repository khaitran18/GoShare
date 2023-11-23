using Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validations
{
    public class CreatePlannedDestinationValidator : AbstractValidator<CreatePlannedDestinationCommand>
    {
        public CreatePlannedDestinationValidator()
        {
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("StartLatitude must be between -90 and 90.");
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("StartLongitude must be between -180 and 180.");
        }
    }
}
