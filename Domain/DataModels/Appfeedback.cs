using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Appfeedback
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Time { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
