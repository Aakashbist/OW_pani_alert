using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Overwatch.Domain.Model
{
    public class TimeseriesDataRetrival
    {
        public string Metric { get; set; }
        public string Parameter { get; set; }
        public string Location { get; set; }
        public string Unit { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }

    }
}
