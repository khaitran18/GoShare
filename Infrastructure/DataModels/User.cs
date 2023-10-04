using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public partial class User
    {
        public User()
        {
            Appfeedbacks = new HashSet<Appfeedback>();
            Cars = new HashSet<Car>();
            ChatReceiverNavigations = new HashSet<Chat>();
            ChatSenderNavigations = new HashSet<Chat>();
            RatingRateeNavigations = new HashSet<Rating>();
            RatingRaterNavigations = new HashSet<Rating>();
            Trips = new HashSet<Trip>();
        }

        public Guid Id { get; set; }
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? CompanyAddress { get; set; }
        public DateTime AddressUpdateDate { get; set; }
        public bool? Isdriver { get; set; }
        public bool? Isverify { get; set; }
        public bool? Isactive { get; set; }
        public string? DisabledReason { get; set; }

        public virtual ICollection<Appfeedback> Appfeedbacks { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Chat> ChatReceiverNavigations { get; set; }
        public virtual ICollection<Chat> ChatSenderNavigations { get; set; }
        public virtual ICollection<Rating> RatingRateeNavigations { get; set; }
        public virtual ICollection<Rating> RatingRaterNavigations { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
