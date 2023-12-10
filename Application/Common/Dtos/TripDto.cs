using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public string PassengerName { get; set; } = null!;
        public string? PassengerPhoneNumber { get; set; }
        public Guid? DriverId { get; set; }
        public Guid StartLocationId { get; set; }
        public Guid EndLocationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PickupTime { get; set; }
        public double Distance { get; set; }
        public double Price { get; set; }
        public Guid CartypeId { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid BookerId { get; set; }
        public string? Note { get; set; }
        public TripType Type { get; set; }

        public UserDto? Driver { get; set; }
        public UserDto? Passenger { get; set; }
        public UserDto Booker { get; set; } = null!;
        public LocationDto EndLocation { get; set; } = null!;
        public LocationDto StartLocation { get; set; } = null!;
        public CartypeDto Cartype { get; set; } = null!;
        public List<TripImageDto> TripImages { get; set; } = new List<TripImageDto>();
    }

    public class TripEndDto
    {
        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public string PassengerName { get; set; } = null!;
        public string? PassengerPhoneNumber { get; set; }
        public Guid? DriverId { get; set; }
        public Guid StartLocationId { get; set; }
        public Guid EndLocationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PickupTime { get; set; }
        public double Distance { get; set; }
        public double Price { get; set; }
        public Guid CartypeId { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid BookerId { get; set; }
        public string? Note { get; set; }
        public TripType Type { get; set; }
        public double? SystemCommission { get; set; }

        public UserDto? Driver { get; set; }
        public UserDto? Passenger { get; set; }
        public UserDto Booker { get; set; } = null!;
        public LocationDto EndLocation { get; set; } = null!;
        public LocationDto StartLocation { get; set; } = null!;
        public CartypeDto Cartype { get; set; } = null!;
    }
}
