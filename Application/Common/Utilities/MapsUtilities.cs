using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities
{
    public static class MapsUtilities
    {
        public static double GetDistance(Location loc1, Location loc2)
        {
            const double R = 6371; //km
            double lat1 = (double)loc1.Latitude * Math.PI / 180;
            double lat2 = (double)loc2.Latitude * Math.PI / 180;
            double lon1 = (double)loc1.Longtitude * Math.PI / 180;
            double lon2 = (double)loc2.Longtitude * Math.PI / 180;
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;
        }
    }
}
