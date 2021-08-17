using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RestockReport.Models
{
	[DataContract]
	public class DataPoint
	{
	


		public DataPoint(string month,int year, int monthid)
		{
			this.Month = month;
			this.Year = year;
			this.MonthId = monthid;
		}

		[DataMember(Name = "month")]
		public string  Month = "";
		[DataMember(Name = "year")]
		public int Year = 0;
		[DataMember(Name = "monthid")]
		public int MonthId = 0;
	}
}