﻿using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Appfeedback
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
