﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class AdminUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool Isdriver { get; set; }
        public bool Isverify { get; set; }
        public string? DisabledReason { get; set; }
        public string? AvatarUrl { get; set; }
        public Guid? GuardianId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public short? Gender { get; set; }
        public DateTime Birth { get; set; }
        public double Balance { get; set; }
        public CarDto? Car { get; set; }
        public UserDto? Guardian { get; set; }
    }
}
