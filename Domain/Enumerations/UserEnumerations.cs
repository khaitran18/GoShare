using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    public enum UserStatus : short
    {
        BUSY = 0,
        ACTIVE = 1,
        INACTIVE = 2,
        BANNED = 3,
        REJECTED = 4,
        SUSPENDED = 5
    }
}
