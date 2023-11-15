using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities
{
    public class HashingUtility
    {
        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        //public static string GetIpAddress()
        //{
        //    string ipAddress;
        //    try
        //    {
        //        ipAddress = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //        if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown") || ipAddress.Length > 45)
        //            ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    catch (Exception ex)
        //    {
        //        ipAddress = "Invalid IP:" + ex.Message;
        //    }

        //    return ipAddress;
        //}
        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }
    }
}
