using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class WalletMonthStatistic
    {
        public int Month { get; set; }
        public double MonthTotal { get; set; }
        public double WeekAverage { get; set; }
        public double CompareToLastMonth { get; set; } = 0;
    }
}
