using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class DriverInformationResponse
    {
        public int RatingNum { get; set; }
        public double Rating { get; set; }
        public double DailyIncome { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
