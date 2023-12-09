using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities.SignalR
{
    public static class SignalRUtilities
    {
        public static string GetGroupNameForUser(User user, Trip trip)
        {
            // If the user is a dependent and the booker is the guardian
            if (user.GuardianId != null && user.GuardianId == trip.BookerId)
            {
                return $"{user.Id}-{user.GuardianId}";
            }

            // Driver
            return $"{user.Id}";
        }
    }
}
