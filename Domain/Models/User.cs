using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class User
    {
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
    }
}
