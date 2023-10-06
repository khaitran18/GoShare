using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class AppfeedbackModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Time { get; set; }
    }
}
