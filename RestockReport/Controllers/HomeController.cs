using RestockReport.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [FilterConfig.NoDirectAccess]


    public class HomeController : Controller
    {
        private RestockReportsEntities dbEntities = new RestockReportsEntities();
        [Route("Demo")]
        public ActionResult Index222(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0)
        {
            try
            {
                ViewBag.MonthList = dbEntities.tblMonths.ToList();
                ViewBag.YearList = dbEntities.tblYears.ToList();
                ViewBag.UserId = UserId;
                ViewBag.ddlTerminlKey = ddlTerminalKey;
                ViewBag.ddlMonthKey = ddlmonthlist;
                ViewBag.ddlYearlKey = ddlyearlist;
                //var TransactionCount = (from transactioncounts in dbEntities.Sp_Get_User_Graph_Data(UserId, TerminalKey)
                //                        select transactioncounts).ToList();

                //var TransactionCount = dbEntities.Sp_Get_User_Graph_Data("6", "-1").ToList();
                if (ddlTerminalKey == null)
                {
                    ddlTerminalKey = "0";
                }




                ViewBag.TerminalList = dbEntities.Sp_Get_User_Assigned_Terminal_List(Convert.ToInt32(UserId)).ToList();

                var TransactionCount = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                        select transaction.TransactionCount).ToList();
                var MonthNames = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                  select transaction.Month).ToList();

                //var MonthNames = (from months in dbEntities.GraphDataFilters
                //                    select months.Month).ToList();

                string MonthNameList = string.Join(",", MonthNames);


                string transactioncountlist = string.Join(",", TransactionCount);

                ViewBag.MonthGraphList = MonthNameList.Trim();
                ViewBag.TransactionGraphList = transactioncountlist.Trim();


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

            }

            return View();
        }


       [HttpGet]
        public ActionResult Index(string  UserId,string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0,string FilterType="")
        {
            Sp_Get_Donut_Revenue_Graph_Result spd = new Sp_Get_Donut_Revenue_Graph_Result();
            try
            {
                double noofdays = 0.0;
                double total = 0.0;
                if (FilterType==null || FilterType == "")
                {
                    FilterType = "M";
                }

                DateTime baseDatess = DateTime.Today;
                var thisMonthStart = baseDatess.AddDays(1 - baseDatess.Day);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);


                ViewBag.Checked = "";
                ViewBag.MonthList = dbEntities.tblMonths.ToList();
                ViewBag.YearList = dbEntities.tblYears.ToList();
                ViewBag.UserId = UserId;
                ViewBag.ddlTerminlKey = ddlTerminalKey;
                ViewBag.ddlMonthKey = ddlmonthlist;
                ViewBag.ddlYearlKey = ddlyearlist;
                //var TransactionCount = (from transactioncounts in dbEntities.Sp_Get_User_Graph_Data(UserId, TerminalKey)
                //                        select transactioncounts).ToList();

                //var TransactionCount = dbEntities.Sp_Get_User_Graph_Data("6", "-1").ToList();
                if (ddlTerminalKey == null)
                {
                    ddlTerminalKey = "0";
                }

                


                ViewBag.TerminalList = dbEntities.Sp_Get_User_Assigned_Terminal_List(Convert.ToInt32(UserId)).ToList();

                //var TransactionCount = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                //                        select transaction.TransactionCount).ToList();
                //var MonthNames = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                //                  select transaction.Month).ToList();
                //string MonthNameList = string.Join(",", MonthNames);
                //string transactioncountlist = string.Join(",", TransactionCount);




                
                List<GraphSummaryData> GD = new List<GraphSummaryData>();
                List<GraphColor> GCC = new List<GraphColor>();

                if (FilterType=="M")
                {
                    GD = dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist).Select(a => new GraphSummaryData
                    {
                        TransactionCount = a.TransactionCount,
                        MonthNames = a.Month
                    }).ToList();

                }
                else if (FilterType=="D")
                {
                    //List <sp_Get_da = dbEntities.Sp_Get_Daily_Transaction(thismonthstart, thismonthend);

                    GD = dbEntities.Sp_Get_Daily_Transaction(thisMonthStart, thisMonthEnd,ddlTerminalKey).Select(a => new GraphSummaryData
                    {
                        TransactionCount = a.counts,
                        MonthNames = a.DATE
                    }).ToList();

                }



                if (FilterType=="D")
                {

                    int CurrentMonthDays = DateTime.Now.Day;

                    noofdays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    total = GD.Sum(item => item.TransactionCount??0);

                    for(int i =0;i<noofdays;i++)
                    {
                        GCC.Add(new GraphColor()
                        {
                        Color = @"'#031bfd'"
                        }); 
                    }

                    if (total > 0)
                    {
                        double AverageTransactionCount = (total / CurrentMonthDays);

                        //double AverageTransactionCount = (total / noofdays);
                        ViewBag.AverageTransactionCount = Math.Ceiling(AverageTransactionCount);
                    }
                    else
                    {
                        ViewBag.AverageTransactionCount = 0;
                    }

                    ViewBag.Checked = "checked";
                
                }
                else if (FilterType == "M")
                {
                   
                    noofdays = 12;
                    total = GD.Sum(item => item.TransactionCount ?? 0);
                    for (int i = 0; i <= noofdays; i++)
                    {
                        GCC.Add(new GraphColor()
                        {
                            Color = @"'#7dc576'"
                        });
                    }
                    if (total > 0)
                    {
                        double AverageTransactionCount = (total / noofdays);
                        ViewBag.AverageTransactionCount = Math.Ceiling(AverageTransactionCount);
                    }
                    else
                    {
                        ViewBag.AverageTransactionCount = 0;
                    }
                   
                }

                ViewBag.FilterType = FilterType;



                string ColorList = string.Join(",", GCC.Select(a => a.Color));



                string MonthNameList = string.Join(",", GD.Select(a => a.MonthNames));


                string transactioncountlist = string.Join(",", GD.Select(a => a.TransactionCount));

                ViewBag.ColorList = ColorList;

                ViewBag.MonthGraphList = MonthNameList.Trim();
                ViewBag.TransactionGraphList = transactioncountlist.Trim();


           
                spd = dbEntities.Sp_Get_Donut_Revenue_Graph(UserId, ddlTerminalKey).FirstOrDefault();
                ViewBag.LastMonthRevenue = spd.GrandTotal - spd.LastMonthRevenue;
                ViewBag.ThisMonthLastYearRevenue = spd.GrandTotal - spd.ThisMonthLastYearRevenue;
                ViewBag.ThisMonthToDate = spd.GrandTotal - spd.ThisMonthToDate;
                ViewBag.LastTwelveMonths = spd.GrandTotal - spd.LastTwelveMonths;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

            }

            return View(spd);
        }


        [HttpPost]
        public ActionResult UpdateGraphData(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0,string FilterType="")
        {

            return Json(new { url = Url.Action("Index", "Home", new { UserId = UserId, ddlTerminalKey = ddlTerminalKey, ddlmonthlist = ddlmonthlist, ddlyearlist = ddlyearlist, FilterType= FilterType }) });
        }




        [HttpPost]
        public ActionResult OpenReportPreview(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0)
        {

            return Json(new { url = Url.Action("ReportPreview", "Home", new { target = "_blank" , UserId = UserId, ddlTerminalKey= ddlTerminalKey, ddlmonthlist = ddlmonthlist,  ddlyearlist = ddlyearlist }) });
        }

        public ActionResult ReportPreview(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0)
        {
            try
            {
                //int  Month = Convert.ToInt32( DateTime.Now.ToString("MM"));
                int year = Convert.ToInt32(DateTime.Now.Year.ToString());
              

                string month = DateTime.Now.ToString("MMMM");
                string monthshortname = DateTime.Now.ToString("MMM");
                string CurrentMonthYear = month + " "+year;

                ViewBag.MonthShortName = monthshortname;
                ViewBag.CurrentMonthYear = CurrentMonthYear;

                //string UserPrimaryID = (from users in DbEntity.tblUsers
                //                     where users.UserID == UserId
                //                        select users.User_ID).Single().ToString();




                var WebMoonUser = (from users in dbEntities.tblWebmoonUsers
                                   select users.WebmoonId).Single();
                ViewBag.WebMoonUser = WebMoonUser;


                var TransactionCount = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                        select transaction.TransactionCount).ToList();
                var MonthNames = (from transaction in dbEntities.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                  select transaction.Month).ToList();

                //var MonthNames = (from months in dbEntities.GraphDataFilters
                //                    select months.Month).ToList();

                string MonthNameList = string.Join(",", MonthNames);


                string transactioncountlist = string.Join(",", TransactionCount);

                ViewBag.MonthGraphList = MonthNameList.Trim();
                ViewBag.TransactionGraphList = transactioncountlist.Trim();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();

        }






    }
}