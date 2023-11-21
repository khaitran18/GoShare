using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;

        public Guid? Id { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string Role { get; set; } = null!;
        public TripDto? CurrentTrip;
    }
}
