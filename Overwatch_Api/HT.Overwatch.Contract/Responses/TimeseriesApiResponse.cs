using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Overwatch.Contract.Responses
{
    public class TimeseriesApiResponse
    {
        public string Metric { get; set; }
        public int VariableId { get; set; }
        public string VariableName { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int ParamaterId { get; set; }
        public string ParamaterName { get; set; }
        public string ParameterUnits { get; set; }
    }
}
