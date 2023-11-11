using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class UserClaims : IDisposable
    {
        public Guid? id { get; set; } 
        public string? name { get; set; }
        public string? phone { get; set; }
        public UserRoleEnumerations Role { get; set; }

        public void Dispose()
        {
        }
    }
}
