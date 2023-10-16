using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    public enum TripStatus : short
    {
        PENDING = 0,
        GOING_TO_PICKUP = 1,
        GOING = 2,
        COMPLETED = 3,
        CANCELED = 4,
        TIMEDOUT = 5
    }
}
