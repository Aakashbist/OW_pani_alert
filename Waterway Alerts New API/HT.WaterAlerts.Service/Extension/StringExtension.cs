using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HT.WaterAlerts.Service
{
    public static class StringExtension
    {
        public static string RemoveHtmlAttributes(this string content)
        {
            return Regex.Replace(content, @"<[^>]+>|&nbsp;", "").Trim();
        }
       
    }
}
