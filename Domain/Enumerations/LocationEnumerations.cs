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
        HOME = 1,
        PAST_DESTINATION = 2
    }
}
