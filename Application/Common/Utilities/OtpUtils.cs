using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities
{
    public class OtpUtils
    {
        public static string Generate()
        {
            var random = new Random();
            var otp = "";
            for (int i = 0; i < 6; i++)
            {
                otp += random.Next(0, 9).ToString();
            }
            return otp;
        }
    }
}
