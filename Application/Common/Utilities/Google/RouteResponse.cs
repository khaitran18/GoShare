using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities.Google
{
    public class RouteResponse
    {
        public List<Route> Routes { get; set; } = null!;
    }

    public class Route
    {
        public int DistanceMeters { get; set; }
        public string Duration { get; set; } = null!;
        public Polyline Polyline { get; set; } = null!;
    }

    public class Polyline
    {
        public string EncodedPolyline { get; set; } = null!;
    }
}
