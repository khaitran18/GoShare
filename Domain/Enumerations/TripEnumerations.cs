﻿using System;
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

    public enum TripType : short
    {
        SELF_BOOK = 0,
        BOOK_FOR_DEP_WITH_APP = 1,
        BOOK_FOR_DEP_NO_APP = 2
    }

    public enum TripImageType : short
    {
        PICK_UP = 0,
        END_TRIP = 1
    }
}
