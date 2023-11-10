using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    public enum LocationType : short
    {
        CURRENT_LOCATION = 0,
        PLANNED_DESTINATION = 1,
        PAST_DESTINATION = 2,
        PAST_ORIGIN = 3
    }
}
