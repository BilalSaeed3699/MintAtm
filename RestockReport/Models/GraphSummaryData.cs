using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestockReport.Models
{
    public class GraphSummaryData
    {
        public Nullable<int>  TransactionCount { get; set; }
        public string MonthNames { get; set; }
    }
}