using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class UserDto
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
