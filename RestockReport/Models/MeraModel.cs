using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestockReport.Models
{
    public class MeraModel
    {
        public string AtmTerminalKey { get; set; }
        public Nullable<int> ID { get; set; }
        public string[] MonthNames { get; set; }
        public string[] Color { get; set; }
        public int[] Month { get; set; }
        public Nullable<int> DateYear { get; set; }
        public Nullable<int> TransactionCount { get; set; }
        public string Entityname { get; set; }
        public string MailingAddress { get; set; }
        public string Entityregion { get; set; }
        public string Entitypostalcode { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public Nullable<decimal> Totalterminalsurchageamount { get; set; }
        public Nullable<int> SurchargesplitId1 { get; set; }
        public Nullable<int> SurchargesplitId2 { get; set; }
        public Nullable<int> SurchargesplitId3 { get; set; }
        public Nullable<int> SurchargesplitId4 { get; set; }
        public Nullable<int> SurchargesplitId5 { get; set; }


        public Nullable<int> Surchargesettlementsource { get; set; }

        public Nullable<int> Surchargesettlementsource1 { get; set; }
        public Nullable<int> Surchargesettlementsource2 { get; set; }
        public Nullable<int> Surchargesettlementsource3 { get; set; }
        public Nullable<int> Surchargesettlementsource4 { get; set; }
        public Nullable<int> Surchargesettlementsource5 { get; set; }


        public Nullable<decimal> Surchargesplitamount1 { get; set; }
        public Nullable<decimal> Surchargesplitamount2 { get; set; }
        public Nullable<decimal> Surchargesplitamount3 { get; set; }
        public Nullable<decimal> Surchargesplitamount4 { get; set; }
        public Nullable<decimal> Surchargesplitamount5 { get; set; }
    }
}