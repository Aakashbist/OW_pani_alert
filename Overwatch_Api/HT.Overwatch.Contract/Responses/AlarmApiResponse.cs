using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Overwatch.Contract.Responses
{
    public sealed class AlarmApiResponse
    {
        public string SiteName { get; set; } = null!;
        public float Value { get; set; }
        public DateTime DateSent { get; set; }
    }
}
