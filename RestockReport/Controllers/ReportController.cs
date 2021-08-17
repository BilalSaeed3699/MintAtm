using Newtonsoft.Json;
using RestockReport.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    //[FilterConfig.NoDirectAccess]
    public class ReportController : Controller
    {




        private RestockReportsEntities DbEntity = new RestockReportsEntities();


        [FilterConfig.AuthorizeActionFilter]
        [FilterConfig.NoDirectAccess]
        [HttpGet]
        // GET: Report
        public ActionResult Index(string UserID, DateTime ReportDate, string ddlTerminalKey, int? IsAllRecord = 0, int? optradio = 1)
        {

            try
            {


                var User_ID = (from users in DbEntity.tblUsers
                               where users.UserID == UserID
                               select users.User_ID).Single();



                //ReportDate = ReportDate.AddDays(-6);
                ViewBag.ReportDate = ReportDate;
                ViewBag.UserID = UserID;
                ViewBag.TerminalList = DbEntity.Sp_Get_User_Assigned_Terminal_List(User_ID).ToList();


                ViewBag.ddlTerminlKey = ddlTerminalKey;



                var ReportData = DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == "").ToList();

                ViewBag.ReportData = ReportData;


                if (IsAllRecord != 1)
                {
                    if (ddlTerminalKey == null || ddlTerminalKey == "0")
                    {
                        ddlTerminalKey = "";
                        //ReportData =DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == UserID && DbFunctions.TruncateTime(rd.ReportDate) == DbFunctions.TruncateTime(ReportDate)).ToList();

                        ViewBag.ReportData = DbEntity.Sp_Get_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).OrderByDescending(a => a.TransactionSequenceNumber).ToList();


                        //ViewBag.ReportData = ReportData;

                    }

                    else if (ddlTerminalKey != null)
                    {
                        //  ReportData =
                        //                       DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == UserID && DbFunctions.TruncateTime(rd.ReportDate)
                        //                   == DbFunctions.TruncateTime(ReportDate)
                        //&& ddlTerminalKey.Contains(rd.AtmTerminalKey)
                        //    ).ToList();
                        //  ViewBag.ReportData = ReportData;

                        ViewBag.ReportData = DbEntity.Sp_Get_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).OrderByDescending(a => a.TransactionSequenceNumber).ToList();

                    }

                }
                else if (IsAllRecord == 1)
                {
                    //ReportData =DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == UserID).ToList();
                    ViewBag.ReportData = DbEntity.Sp_Get_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).OrderByDescending(a => a.TransactionSequenceNumber).ToList();
                    ViewBag.ReportData = ReportData;
                }

                ViewBag.Option = 1;

                if (optradio == 2)
                {
                    //ViewBag.Option = 2;
                    //Method1
                    return new ActionAsPdf("DailyTransactionReportPreview", new { UserID = UserID, ReportDate = ReportDate, ddlTerminalKey = ddlTerminalKey })
                    {
                        FileName = "DailyTransactionReportPreview" + ".pdf"

                    };

                }





                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while getting Terminals list : " + ex.Message;
            }
            return View();
        }


        //[FilterConfig.AuthorizeActionFilter]
        //[FilterConfig.NoDirectAccess]
        public ActionResult MonthlyStatementReport(string UserId, int? ddlBankList = 0, int? ddlSettlementType = 0, string ddlTerminalKey = "", int? ddlmonthlist = 0, int? ddlyearlist = 0, int? optradio = 1)
        {

            ViewBag.UserBankAccountList = DbEntity.Sp_Get_User_Bank_List(Convert.ToInt32(UserId)).ToList();
            ViewBag.ddlBankList = ddlBankList;
            ViewBag.ddlSettlementType = ddlSettlementType;

            ViewBag.NewsImage = DbEntity.tblNewletters.ToList();

            List<tblBankAccountDetail> BDA = new List<tblBankAccountDetail>();
            try
            {

                int monthid = Convert.ToInt32(ddlmonthlist);

                string monthName = "";



                if (ddlmonthlist == 0)
                {
                    monthName = "";
                }
                else
                {
                    monthName = new DateTime(2010, monthid, 1).ToString("MMM", CultureInfo.InvariantCulture);

                }



                ViewBag.TerminalLocation = "";
                ViewBag.TerminalAddress = "";
                ViewBag.Region = "";
                ViewBag.PostalCode = "";
                if (ddlTerminalKey != null && ddlTerminalKey != "" && ddlTerminalKey != "0")
                {
                    ViewBag.TerminalLocation = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                                select terminal.LocationName
                                                ).Single();

                    ViewBag.TerminalAddress = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                               select terminal.Address
                            ).SingleOrDefault();

                    ViewBag.Region = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                      select terminal.Region
                           ).SingleOrDefault();

                    ViewBag.PostalCode = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                          select terminal.PostalCode
                           ).SingleOrDefault();

                    ViewBag.Commission = (from terminal in DbEntity.tblTermsDetails.Where(a => a.TerminalKey == ddlTerminalKey)
                                          select terminal.Totalterminalsurchageamount
                           ).SingleOrDefault();

                    //BDA =
                    var BankDetail = (from termsdetails in DbEntity.tblTermsDetails
                                      join bankaccountdetails in DbEntity.tblBankAccountDetails
                                   on termsdetails.VaultCashID equals bankaccountdetails.BankAccountId
                                      where termsdetails.TerminalKey == ddlTerminalKey
                                      select new
                                      {
                                          Entityname = bankaccountdetails.Entityname,
                                          MailingAddress = bankaccountdetails.MailingAddress,
                                          Entityregion = bankaccountdetails.Entityregion,
                                          Entitypostalcode = bankaccountdetails.Entitypostalcode
                                      }).ToList();


                    foreach (var item in BankDetail)

                    {
                        BDA.Add(new tblBankAccountDetail()
                        {
                            Entityname = item.Entityname,
                            MailingAddress = item.MailingAddress,
                            Entityregion = item.Entityregion,
                            Entitypostalcode = item.Entitypostalcode
                        });
                    }
                    ViewBag.MerchantDetails = BDA;

                    ViewBag.GrossCommission = DbEntity.Sp_Get_Gross_Commision(ddlTerminalKey).FirstOrDefault();
                }
                ViewBag.MerchantDetails = BDA;


                ViewBag.MonthName = monthName;
                ViewBag.MonthYear = monthName + " " + ddlyearlist;
                //August
                var WebMoonUser = (from users in DbEntity.tblWebmoonUsers
                                   select users.WebmoonId).Single();
                ViewBag.WebMoonUser = WebMoonUser;


                //var WebMoonUser = (from users in DbEntity.tblWebmoonUsers
                //                   select users.WebmoonId).Single();


                int month = System.DateTime.Now.Month;
                int year = System.DateTime.Now.Year;

                ViewBag.optradio = optradio;
                ViewBag.MonthList = DbEntity.tblMonths.ToList();
                ViewBag.YearList = DbEntity.tblYears.ToList();
                ViewBag.UserId = UserId;
                ViewBag.ddlTerminlKey = ddlTerminalKey;




                if (ddlmonthlist == 0)
                {
                    ddlmonthlist = month;
                }

                if (ddlyearlist == 0)
                {
                    ddlyearlist = year;
                }
                ViewBag.ddlMonthKey = ddlmonthlist;
                ViewBag.ddlYearlKey = ddlyearlist;
                //var TransactionCount = (from transactioncounts in dbEntities.Sp_Get_User_Graph_Data(UserId, TerminalKey)
                //                        select transactioncounts).ToList();

                //var TransactionCount = dbEntities.Sp_Get_User_Graph_Data("6", "-1").ToList();
                if (ddlTerminalKey == null)
                {
                    ddlTerminalKey = "0";
                }




                ViewBag.TerminalList = DbEntity.Sp_Get_User_Assigned_Terminal_List(Convert.ToInt32(UserId)).ToList();


                var TransactionCount = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                        select transaction.TransactionCount).ToList();

                var monthtranscount = 0;
                foreach (var coun in TransactionCount)
                {
                    if (coun != 0)
                    {
                        monthtranscount = coun;
                    }
                }
                ViewBag.monthtranscount = monthtranscount;

                var MonthNames = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                  select transaction.Month).ToList();

                //var MonthNames = (from months in dbEntities.GraphDataFilters
                //                    select months.Month).ToList();

                string MonthNameList = string.Join(",", MonthNames);


                string transactioncountlist = string.Join(",", TransactionCount);

                ViewBag.MonthGraphList = MonthNameList.Trim();
                ViewBag.TransactionGraphList = transactioncountlist.Trim();


                //if (optradio == 2)
                //{
                //    //Method1
                //    return new ActionAsPdf("MonthStatements", new { UserId = UserId, ddlTerminalKey = ddlTerminalKey, ddlmonthlist = ddlmonthlist, ddlyearlist = ddlyearlist, optradio = optradio })
                //    {
                //        FileName = "MonthlyTransactionReportPreview" + ".pdf"

                //    };


                //}
                if (optradio == 2)
                {
                    //Method1
                    //return new ActionAsPdf("MonthStatements", new { UserId = UserId, ddlTerminalKey = ddlTerminalKey, ddlmonthlist = ddlmonthlist, ddlyearlist = ddlyearlist, optradio = optradio })
                    //{
                    //    FileName = "MonthlyTransactionReportPreview" + ".pdf"

                    //};

                    return RedirectToAction("MonthlyTransactionReportPdf", "Report", new
                    {
                        UserId = UserId,
                        ddlBankList = ddlBankList,
                        ddlSettlementType = ddlSettlementType,
                        ddlTerminalKey = ddlTerminalKey,
                        ddlmonthlist = ddlmonthlist,
                        ddlyearlist = ddlyearlist,
                        optradio = optradio
                    });


                }

                //string UserId,int? ddlBankList = 0, int? ddlSettlementType = 0,   string ddlTerminalKey = "", int? ddlmonthlist = 0, int? ddlyearlist = 0, int? optradio = 1

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

            }

            return View();
        }



        public JsonResult ReloadTerminaList(int UserId, int BankId)
        {
            try
            {
                if (BankId > 0)
                {
                    var TerminalList = GetAtmList(UserId, BankId);
                    var Terminaldata = TerminalList.Select(m => new SelectListItem()
                    {
                        Text = m.TerminalName.ToString(),
                        Value = m.TerminalKey.ToString(),
                    });

                    return Json(Terminaldata, JsonRequestBehavior.AllowGet);
                }
                else if (BankId == 0)
                {
                    var TerminalList = GetAllAtmList(UserId);
                    var Terminaldata = TerminalList.Select(m => new SelectListItem()
                    {
                        Text = m.TerminalName.ToString(),
                        Value = m.TerminalKey.ToString(),
                    });

                    return Json(Terminaldata, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);

            }



        }

        private IList<Sp_Load_Bank_Atm_Result> GetAtmList(int userid, int BankId)
        {
            return DbEntity.Sp_Load_Bank_Atm(userid, BankId).ToList();
        }

        private IList<Sp_Get_User_Assigned_Terminal_List_Result> GetAllAtmList(int userid)
        {
            return DbEntity.Sp_Get_User_Assigned_Terminal_List(userid).ToList();
        }


        [HttpPost]
        public JsonResult SaveImage(string base64image, string UserId)
        {
            var absoultepath = "";


            if (string.IsNullOrEmpty(base64image))
            {
                return Json(absoultepath, JsonRequestBehavior.AllowGet);

            }

            var t = base64image.Substring(22);  // remove data:image/png;base64,

            byte[] bytes = Convert.FromBase64String(t);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            //var randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".png";
            var randomFileName = UserId + ".png";

            absoultepath = "/Barchart/" + randomFileName;
            var fullPath = Path.Combine(Server.MapPath("~/Barchart/"), randomFileName);


            image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
            return Json(absoultepath, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult MonthStatements(string UserId, string ddlTerminalKey = "", int? ddlBankList = 0, int? ddlmonthlist = 0, int? ddlSettlementType = 0, int? ddlyearlist = 0, int? optradio = 1, int? ajax = 1)


        {

            ViewBag.NewsImage = DbEntity.tblNewletters.ToList();

            List<tblBankAccountDetail> BDA = new List<tblBankAccountDetail>();
            try
            {

                int monthid = Convert.ToInt32(ddlmonthlist);

                string monthName = "";


                monthName = new DateTime(2010, monthid, 1).ToString("MMM", CultureInfo.InvariantCulture);

                ViewBag.TerminalLocation = "";
                ViewBag.TerminalAddress = "";
                ViewBag.Region = "";
                ViewBag.PostalCode = "";
                if (ddlTerminalKey != null && ddlTerminalKey == "")
                {
                    ViewBag.TerminalLocation = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                                select terminal.LocationName
                                                ).Single();

                    ViewBag.TerminalAddress = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                               select terminal.Address
                            ).SingleOrDefault();

                    ViewBag.Region = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                      select terminal.Region
                          ).SingleOrDefault();

                    ViewBag.PostalCode = (from terminal in DbEntity.tblTerminals.Where(a => a.TerminalKey == ddlTerminalKey)
                                          select terminal.PostalCode
                          ).SingleOrDefault();


                    ViewBag.Commission = (from terminal in DbEntity.tblTermsDetails.Where(a => a.TerminalKey == ddlTerminalKey)
                                          select terminal.Totalterminalsurchageamount
                           ).SingleOrDefault();

                    //BDA =
                    var BankDetail = (from termsdetails in DbEntity.tblTermsDetails
                                      join bankaccountdetails in DbEntity.tblBankAccountDetails
                                   on termsdetails.VaultCashID equals bankaccountdetails.BankAccountId
                                      where termsdetails.TerminalKey == ddlTerminalKey
                                      select new
                                      {
                                          Entityname = bankaccountdetails.Entityname,
                                          MailingAddress = bankaccountdetails.MailingAddress,
                                          Entityregion = bankaccountdetails.Entityregion,
                                          Entitypostalcode = bankaccountdetails.Entitypostalcode
                                      }).ToList();


                    foreach (var item in BankDetail)

                    {
                        BDA.Add(new tblBankAccountDetail()
                        {
                            Entityname = item.Entityname,
                            MailingAddress = item.MailingAddress,
                            Entityregion = item.Entityregion,
                            Entitypostalcode = item.Entitypostalcode
                        });
                    }
                    ViewBag.MerchantDetails = BDA;

                    ViewBag.GrossCommission = DbEntity.Sp_Get_Gross_Commision(ddlTerminalKey).FirstOrDefault();
                }
                ViewBag.MerchantDetails = BDA;


                ViewBag.MonthName = monthName;
                ViewBag.MonthYear = monthName + " " + ddlyearlist;




                ViewBag.UserId = UserId;

                ViewBag.ddlTerminlKey = ddlTerminalKey;
                ViewBag.ddlMonthKey = ddlmonthlist;
                ViewBag.ddlYearlKey = ddlyearlist;

                ViewBag.optradio = optradio;


                ViewBag.UserBarChart = "/Barchart/" + UserId + ".png";
                ViewBag.Ajax = ajax;
                //int  Month = Convert.ToInt32( DateTime.Now.ToString("MM"));
                int year = Convert.ToInt32(DateTime.Now.Year.ToString());


                string month = DateTime.Now.ToString("MMMM");
                string monthshortname = DateTime.Now.ToString("MMM");
                string CurrentMonthYear = month + " " + year;

                ViewBag.MonthShortName = monthshortname;
                ViewBag.CurrentMonthYear = CurrentMonthYear;
                var WebMoonUser = (from users in DbEntity.tblWebmoonUsers
                                   select users.WebmoonId).Single();
                ViewBag.WebMoonUser = WebMoonUser;

                var TransactionCount = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                        select transaction.TransactionCount).ToList();
                var MonthNames = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                  select transaction.Month).ToList();


                var monthtranscount = 0;
                foreach (var coun in TransactionCount)
                {
                    if (coun != 0)
                    {
                        monthtranscount = coun;
                    }
                }
                ViewBag.monthtranscount = monthtranscount;

                //var MonthNames = (from months in dbEntities.GraphDataFilters
                //                    select months.Month).ToList();

                string MonthNameList = string.Join(",", MonthNames);


                string transactioncountlist = string.Join(",", TransactionCount);

                ViewBag.MonthGraphList = MonthNameList.Trim();
                ViewBag.TransactionGraphList = transactioncountlist.Trim();




                int yearfilter = ddlyearlist ?? 0;
                int monthfilter = ddlmonthlist ?? 0;

                DateTime now = DateTime.Now;

                var LastyearDate = new DateTime(yearfilter - 1, monthfilter, 1);
                var startDate = new DateTime(yearfilter, monthfilter, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                string cWhere = "";
                //string cWhere = "  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                //    "and TransactionResponseCode = 0 ";

                if (ddlBankList > 0 && (ddlTerminalKey == "" || ddlTerminalKey == "0") && ddlSettlementType == 0 )
                {
                    //cWhere += "and TD.vaultcashid = " + ddlBankList + "";
                     cWhere = " tblBankAccountDetails AS BAD,tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                    "and TransactionResponseCode = 0 ";
                    cWhere += "and (td.SurchargesplitId1 =" + ddlBankList + " or td.SurchargesplitId2 =" + ddlBankList + " or td.SurchargesplitId3 =" + ddlBankList + " or td.SurchargesplitId4 =" + ddlBankList + " " +
                            "or td.SurchargesplitId5 = " + ddlBankList + ") and BAD.BankAccountId="+ddlBankList+" ";
                }

                if (ddlTerminalKey != "" && ddlTerminalKey != "0" && ddlBankList ==0 && ddlSettlementType == 0)
                {
                    cWhere = " tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblBankAccountDetails BAD ON BAD.BankAccountId = TD.SurchargesplitId1 "+
                        " or BAD.BankAccountId = TD.SurchargesplitId2 or BAD.BankAccountId = TD.SurchargesplitId3 or BAD.BankAccountId = TD.SurchargesplitId4 or BAD.BankAccountId = TD.SurchargesplitId5 Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                 "and TransactionResponseCode = 0 ";
                    cWhere += " and R.AtmTerminalKey = '" + ddlTerminalKey + "'";
                }
                if (ddlTerminalKey != "" && ddlTerminalKey != "0" && ddlBankList > 0 && ddlSettlementType == 0)
                {
                    cWhere = " tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblBankAccountDetails BAD ON BAD.BankAccountId = TD.SurchargesplitId1 " +
                        " or BAD.BankAccountId = TD.SurchargesplitId2 or BAD.BankAccountId = TD.SurchargesplitId3 or BAD.BankAccountId = TD.SurchargesplitId4 or BAD.BankAccountId = TD.SurchargesplitId5 Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                 "and TransactionResponseCode = 0 ";
                    cWhere += " and R.AtmTerminalKey = '" + ddlTerminalKey + "' and BAD.BankAccountId=" + ddlBankList + " ";
                }
                if (ddlTerminalKey != "" && ddlTerminalKey != "0" && ddlBankList > 0 && ddlSettlementType != 0)
                {
                    cWhere = " tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblBankAccountDetails BAD ON BAD.BankAccountId = TD.SurchargesplitId1 " +
                        " or BAD.BankAccountId = TD.SurchargesplitId2 or BAD.BankAccountId = TD.SurchargesplitId3 or BAD.BankAccountId = TD.SurchargesplitId4 or BAD.BankAccountId = TD.SurchargesplitId5 Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                 "and TransactionResponseCode = 0 ";
                    cWhere += " and R.AtmTerminalKey = '" + ddlTerminalKey + "' and BAD.BankAccountId=" + ddlBankList + " ";
                    cWhere += " and (Surchargesettlementsource1 = " + ddlSettlementType + " or Surchargesettlementsource2 =" + ddlSettlementType + " OR Surchargesettlementsource3 =" + ddlSettlementType + " or Surchargesettlementsource4 = " + ddlSettlementType + " or Surchargesettlementsource5 =  " + ddlSettlementType + ")";

                }
                //if (ddlyearlist != 0)
                //{
                //    cWhere += " and year(HostDate) = " + ddlyearlist + "";
                //}


                //if (ddlmonthlist != 0)
                //{
                //    cWhere += " and Month(Hostdate) = " + ddlmonthlist + "";
                //}
                if (ddlSettlementType != 0 && (ddlTerminalKey == "" || ddlTerminalKey == "0") && ddlBankList > 0)
                {
                    cWhere = " tblBankAccountDetails AS BAD,tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                   "and TransactionResponseCode = 0 ";
                    cWhere += "and (td.SurchargesplitId1 =" + ddlBankList + " or td.SurchargesplitId2 =" + ddlBankList + " or td.SurchargesplitId3 =" + ddlBankList + " or td.SurchargesplitId4 =" + ddlBankList + " " +
                            "or td.SurchargesplitId5 = " + ddlBankList + ") and BAD.BankAccountId=" + ddlBankList + " ";

                    cWhere += " and (Surchargesettlementsource1 = " + ddlSettlementType + " or Surchargesettlementsource2 =" + ddlSettlementType + " OR Surchargesettlementsource3 =" + ddlSettlementType + " or Surchargesettlementsource4 = " + ddlSettlementType + " or Surchargesettlementsource5 =  " + ddlSettlementType + ")";
                }
                if (ddlSettlementType != 0 && (ddlTerminalKey != "" || ddlTerminalKey != "0") && ddlBankList == 0)
                {
                    cWhere = " tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblBankAccountDetails BAD ON BAD.BankAccountId = TD.SurchargesplitId1 " +
                        " or BAD.BankAccountId = TD.SurchargesplitId2 or BAD.BankAccountId = TD.SurchargesplitId3 or BAD.BankAccountId = TD.SurchargesplitId4 or BAD.BankAccountId = TD.SurchargesplitId5 Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                 "and TransactionResponseCode = 0 ";
                    cWhere += " and R.AtmTerminalKey = '" + ddlTerminalKey + "'";
                    cWhere += " and (Surchargesettlementsource1 = " + ddlSettlementType + " or Surchargesettlementsource2 =" + ddlSettlementType + " OR Surchargesettlementsource3 =" + ddlSettlementType + " or Surchargesettlementsource4 = " + ddlSettlementType + " or Surchargesettlementsource5 =  " + ddlSettlementType + ")";
                }
                if (ddlSettlementType != 0 && (ddlTerminalKey == "" || ddlTerminalKey == "0") && ddlBankList ==0)
                {
                    cWhere = " tblReportData R Left Outer Join tblTermsDetails TD ON TD.TerminalKey = R.AtmTerminalKey Left Outer join tblBankAccountDetails BAD ON BAD.BankAccountId = TD.SurchargesplitId1 " +
                        " or BAD.BankAccountId = TD.SurchargesplitId2 or BAD.BankAccountId = TD.SurchargesplitId3 or BAD.BankAccountId = TD.SurchargesplitId4 or BAD.BankAccountId = TD.SurchargesplitId5 Left Outer join tblTerminal TA ON TA.TerminalKey = R.AtmTerminalKey  Where  R.ReportDate between (cast('" + LastyearDate + "'  as date)) and (cast('" + endDate + "'  as date))" +
                 "and TransactionResponseCode = 0 ";
                    cWhere += " and (Surchargesettlementsource1 = " + ddlSettlementType + " or Surchargesettlementsource2 =" + ddlSettlementType + " OR Surchargesettlementsource3 =" + ddlSettlementType + " or Surchargesettlementsource4 = " + ddlSettlementType + " or Surchargesettlementsource5 =  " + ddlSettlementType + ")";
                }





                List<Sp_Get_User_Graph_Filter_Data__Result> GDR = new List<Sp_Get_User_Graph_Filter_Data__Result>();
                GDR = DbEntity.Sp_Get_User_Graph_Filter_Data_(cWhere).ToList();

                List<MeraModel> GDR1 = new List<MeraModel>();
                List<DataPoint> MonthList = new List<DataPoint>();
                for (int i = ddlmonthlist ?? 0; i <= 12; i++)
                {
                    DateTime date = new DateTime(ddlyearlist ?? 0, i, 1);

                    string monthabb = date.ToString("MMM");
                    MonthList.Add(new DataPoint(
                        month = monthabb,
                        year = ddlyearlist - 1 ?? 0,
                        monthid = i
                    ));
                }
                for (int i = 1; i <= ddlmonthlist; i++)
                {
                    DateTime date = new DateTime(ddlyearlist ?? 0, i, 1);

                    string monthabb = date.ToString("MMM");
                    MonthList.Add(new DataPoint(
                              month = monthabb,
                        year = ddlyearlist ?? 0,
                        monthid = i
                    ));
                }
                int count = 0;
                int Bankid = 0;
                string ATMKEY = "";
                List<string> dataPoints;
                List<string> Color;
                List<int> dataMonth;
                var MN = MonthList.Select(x => x.Month).ToList();
                dataPoints = new List<string>();
                for (int j = 0; j < MN.Count; j++)
                {
                    dataPoints.Add((MN[j]));
                }

                if (ddlBankList == 0 && ddlTerminalKey != "0")
                {
                    List<Sp_Get_User_Graph_Filter_Data__Result> GDRNew = new List<Sp_Get_User_Graph_Filter_Data__Result>();
                    GDRNew= GDR.OrderByDescending(x => x.BankAccountId).ToList();
                    for (int i = 0; i < GDRNew.Count; i++)
                    {
                        if (Bankid != Convert.ToInt32(GDRNew[i].BankAccountId))
                        {

                            dataMonth = new List<int>();
                            Color = new List<string>();
                            Bankid = Convert.ToInt32(GDRNew[i].BankAccountId);
                            if (Bankid != 0)
                            {


                                //var MN = GDR.Where(x => x.AtmTerminalKey == ATMKEY).Select(x => x.Month).ToList();
                                var TC = GDRNew.Where(x => x.BankAccountId == Bankid).Select((a => new MeraModel
                                {
                                    TransactionCount = a.TransactionCount,
                                    ID = a.ID,
                                    DateYear = a.DateYear
                                })).ToList();
                                var Col = GDRNew.Where(x => x.BankAccountId == Bankid).Select((a => new MeraModel
                                {
                                    ID = a.ID,
                                    DateYear = a.DateYear
                                })).ToList();

                                var MonthlyTransaction = GDRNew.Where(x => x.BankAccountId == Bankid && x.ID == ddlmonthlist && x.DateYear == ddlyearlist).Select(x => x.TransactionCount).FirstOrDefault();
                                int increment = 0;
                                for (int k = 0; k < MonthList.Count; k++)
                                {
                                    if (TC.Count > increment)
                                    {
                                        if (MonthList[k].MonthId == TC[increment].ID && MonthList[k].Year == TC[increment].DateYear)
                                        {
                                            dataMonth.Add((Convert.ToInt32(TC[increment].TransactionCount)));
                                            increment = increment + 1;
                                        }
                                        else
                                        {
                                            dataMonth.Add((Convert.ToInt32(0)));

                                        }
                                    }
                                    else
                                    {
                                        dataMonth.Add((Convert.ToInt32(0)));
                                    }
                                }
                                for (int j = 0; j < 13; j++)
                                {

                                    if (j == 12)
                                    {
                                        Color.Add(@"#7dc576");
                                    }
                                    else
                                    {
                                        Color.Add(@"#d5d5d5");
                                    }
                                }
                                //int surchargecommission = GDRNew.Where(x => x.BankAccountId == Bankid).Select(x => x.Surchargesplitamount1).FirstOrDefault();
                                count = count + 1;

                                decimal surchargecommission = 0;

                               
                                if (GDRNew[i].Surchargesplitamount1 > 0 && GDRNew[i].SurchargesplitId1 == Bankid)
                                {
                                    surchargecommission =Convert.ToDecimal(GDRNew[i].Surchargesplitamount1);
                                }
                                if (GDRNew[i].Surchargesplitamount2 > 0 && GDRNew[i].SurchargesplitId2 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDRNew[i].Surchargesplitamount2);
                                }
                                if (GDRNew[i].Surchargesplitamount3 > 0 && GDRNew[i].SurchargesplitId3 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDRNew[i].Surchargesplitamount3);
                                }
                                if (GDRNew[i].Surchargesplitamount4 > 0 && GDRNew[i].SurchargesplitId4 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDRNew[i].Surchargesplitamount4);
                                }
                                if (GDRNew[i].Surchargesplitamount5 > 0 && GDRNew[i].SurchargesplitId5 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDRNew[i].Surchargesplitamount5);
                                }



                                int settlementsource = 0;
                                if (GDRNew[i].Surchargesettlementsource1 > 0 && GDRNew[i].SurchargesplitId1 == Bankid)
                                {
                                    settlementsource = GDRNew[i].Surchargesettlementsource1;
                                }
                                if (GDRNew[i].Surchargesettlementsource2 > 0 && GDRNew[i].SurchargesplitId2 == Bankid)
                                {
                                    settlementsource = GDRNew[i].Surchargesettlementsource2;
                                }
                                if (GDRNew[i].Surchargesettlementsource3 > 0 && GDRNew[i].SurchargesplitId3 == Bankid)
                                {
                                    settlementsource = GDRNew[i].Surchargesettlementsource3;
                                }
                                if (GDRNew[i].Surchargesettlementsource4 > 0 && GDRNew[i].SurchargesplitId4 == Bankid)
                                {
                                    settlementsource = GDRNew[i].Surchargesettlementsource4;
                                }
                                if (GDRNew[i].Surchargesettlementsource5 > 0 && GDRNew[i].SurchargesplitId5 == Bankid)
                                {
                                    settlementsource = GDRNew[i].Surchargesettlementsource5;
                                }


                                GDR1.Add(new MeraModel
                                {
                                    ID = count,
                                    AtmTerminalKey = GDRNew[i].AtmTerminalKey.ToString(),
                                    MonthNames = dataPoints.ToArray(),
                                    Month = dataMonth.ToArray(),
                                    Color = Color.ToArray(),
                                    Entityname = GDRNew[i].Entityname.ToString(),
                                    MailingAddress = GDRNew[i].MailingAddress.ToString(),
                                    Entityregion = GDRNew[i].Entityregion.ToString(),
                                    Entitypostalcode = GDRNew[i].Entitypostalcode.ToString(),
                                    LocationName = GDRNew[i].LocationName.ToString(),
                                    Address = GDRNew[i].Address.ToString(),
                                    PostalCode = GDRNew[i].PostalCode.ToString(),
                                    Region = GDRNew[i].Region.ToString(),
                                    Totalterminalsurchageamount = GDRNew[i].Totalterminalsurchageamount,
                                    SurchargesplitId1 = GDRNew[i].SurchargesplitId1,
                                    SurchargesplitId2 = GDRNew[i].SurchargesplitId2,
                                    SurchargesplitId3 = GDRNew[i].SurchargesplitId3,
                                    SurchargesplitId4 = GDRNew[i].SurchargesplitId4,
                                    SurchargesplitId5 = GDRNew[i].SurchargesplitId5,

                                    Surchargesettlementsource1 = GDRNew[i].Surchargesettlementsource1,
                                    Surchargesettlementsource2 = GDRNew[i].Surchargesettlementsource2,
                                    Surchargesettlementsource3 = GDRNew[i].Surchargesettlementsource3,
                                    Surchargesettlementsource4 = GDRNew[i].Surchargesettlementsource4,
                                    Surchargesettlementsource5 = GDRNew[i].Surchargesettlementsource5,



                                    Surchargesplitamount1 = surchargecommission,
                                    Surchargesettlementsource = settlementsource,

                                    TransactionCount = Convert.ToInt32(MonthlyTransaction),
                                }); ;
                            }

                            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                            Bankid = Convert.ToInt32(GDRNew[i].BankAccountId);

                            ViewBag.MonthId = string.Join(",", MonthNames).Trim();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < GDR.Count; i++)
                {


          

                        if (ATMKEY != GDR[i].AtmTerminalKey.ToString())
                        {
                            Bankid = Convert.ToInt32(GDR[i].BankAccountId);
                            dataMonth = new List<int>();
                            Color = new List<string>();
                            ATMKEY = GDR[i].AtmTerminalKey.ToString();
                            if (ATMKEY != "")
                            {


                                //var MN = GDR.Where(x => x.AtmTerminalKey == ATMKEY).Select(x => x.Month).ToList();
                                var TC = GDR.Where(x => x.AtmTerminalKey == ATMKEY && x.BankAccountId== Bankid).Select((a => new MeraModel
                                {
                                    TransactionCount = a.TransactionCount,
                                    ID = a.ID,
                                    DateYear = a.DateYear
                                })).Distinct().ToList();

                                var Col = GDR.Where(x => x.AtmTerminalKey == ATMKEY && x.BankAccountId == Bankid).Select((a => new MeraModel
                                {
                                    ID = a.ID,
                                    DateYear = a.DateYear
                                })).ToList();

                                var MonthlyTransaction = GDR.Where(x => x.AtmTerminalKey == ATMKEY && x.ID == ddlmonthlist && x.DateYear == ddlyearlist && x.BankAccountId == Bankid).Select(x => x.TransactionCount).FirstOrDefault();
                                int increment = 0;
                                for (int k = 0; k < MonthList.Count; k++)
                                {
                                    if (TC.Count > increment)
                                    {
                                        if (MonthList[k].MonthId == TC[increment].ID && MonthList[k].Year == TC[increment].DateYear)
                                        {
                                            dataMonth.Add((Convert.ToInt32(TC[increment].TransactionCount)));
                                            increment = increment + 1;
                                        }
                                        else
                                        {
                                            dataMonth.Add((Convert.ToInt32(0)));

                                        }
                                    }
                                    else
                                    {
                                        dataMonth.Add((Convert.ToInt32(0)));
                                    }
                                }
                                for (int j = 0; j < 13; j++)
                                {

                                    if (j == 12)
                                    {
                                        Color.Add(@"#7dc576");
                                    }
                                    else
                                    {
                                        Color.Add(@"#d5d5d5");
                                    }
                                }



                                decimal surchargecommission = 0;
                                if (GDR[i].Surchargesplitamount1 > 0 && GDR[i].SurchargesplitId1== Bankid )
                                {
                                    surchargecommission =Convert.ToDecimal( GDR[i].Surchargesplitamount1);
                                }
                                if (GDR[i].Surchargesplitamount2 > 0 && GDR[i].SurchargesplitId2 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDR[i].Surchargesplitamount2);
                                }
                                if (GDR[i].Surchargesplitamount3 > 0 && GDR[i].SurchargesplitId3 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDR[i].Surchargesplitamount3);
                                }
                                if (GDR[i].Surchargesplitamount4 > 0 && GDR[i].SurchargesplitId4 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDR[i].Surchargesplitamount4);
                                }
                                if (GDR[i].Surchargesplitamount5 > 0 && GDR[i].SurchargesplitId5 == Bankid)
                                {
                                    surchargecommission = Convert.ToDecimal(GDR[i].Surchargesplitamount5);
                                }


                                int settlementsource = 0;
                                if (GDR[i].Surchargesettlementsource1 > 0 && GDR[i].SurchargesplitId1 == Bankid)
                                {
                                    settlementsource = GDR[i].Surchargesettlementsource1;
                                }
                                if (GDR[i].Surchargesettlementsource2 > 0 && GDR[i].SurchargesplitId2 == Bankid)
                                {
                                    settlementsource = GDR[i].Surchargesettlementsource2;
                                }
                                if (GDR[i].Surchargesettlementsource3 > 0 && GDR[i].SurchargesplitId3 == Bankid)
                                {
                                    settlementsource = GDR[i].Surchargesettlementsource3;
                                }
                                if (GDR[i].Surchargesettlementsource4 > 0 && GDR[i].SurchargesplitId4 == Bankid)
                                {
                                    settlementsource = GDR[i].Surchargesettlementsource4;
                                }
                                if (GDR[i].Surchargesettlementsource5 > 0 && GDR[i].SurchargesplitId5 == Bankid)
                                {
                                    settlementsource = GDR[i].Surchargesettlementsource5;
                                }


                                count = count + 1;
                                GDR1.Add(new MeraModel
                                {

                                    ID = count,
                                    AtmTerminalKey = GDR[i].AtmTerminalKey.ToString(),
                                    MonthNames = dataPoints.ToArray(),
                                    Month = dataMonth.ToArray(),
                                    Color = Color.ToArray(),
                                    Entityname = GDR[i].Entityname.ToString(),
                                    MailingAddress = GDR[i].MailingAddress.ToString(),
                                    Entityregion = GDR[i].Entityregion.ToString(),
                                    Entitypostalcode = GDR[i].Entitypostalcode.ToString(),
                                    LocationName = GDR[i].LocationName.ToString(),
                                    Address = GDR[i].Address.ToString(),
                                    PostalCode = GDR[i].PostalCode.ToString(),
                                    Region = GDR[i].Region.ToString(),
                                    Totalterminalsurchageamount = GDR[i].Totalterminalsurchageamount,
                                    SurchargesplitId1 = GDR[i].SurchargesplitId1,
                                    SurchargesplitId2 = GDR[i].SurchargesplitId2,
                                    SurchargesplitId3 = GDR[i].SurchargesplitId3,
                                    SurchargesplitId4 = GDR[i].SurchargesplitId4,
                                    SurchargesplitId5 = GDR[i].SurchargesplitId5,

                                    Surchargesettlementsource1 = GDR[i].Surchargesettlementsource1,
                                    Surchargesettlementsource2 = GDR[i].Surchargesettlementsource2,
                                    Surchargesettlementsource3 = GDR[i].Surchargesettlementsource3,
                                    Surchargesettlementsource4 = GDR[i].Surchargesettlementsource4,
                                    Surchargesettlementsource5 = GDR[i].Surchargesettlementsource5,
                                    TransactionCount = Convert.ToInt32(MonthlyTransaction),

                                    Surchargesettlementsource = settlementsource,
                                    Surchargesplitamount1 = surchargecommission,
                                }); ;
                            }

                            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                            ATMKEY = GDR[i].AtmTerminalKey.ToString();

                            ViewBag.MonthId = string.Join(",", MonthNames).Trim();
                        }
                    }





                }
                ViewBag.TransactionList = GDR1.ToList();

                List<GraphColor> GCC = new List<GraphColor>();
                int Years = 0;

                //int currentyear = DateTime.Now.Year;

                //if(ddlyearlist == currentyear)
                //{
                //    int Previousmonth = 0;
                //    Previousmonth = 13 - ddlmonthlist??0;
                //    ddlmonthlist=  ddlmonthlist + Previousmonth;
                //}
                //for (int i = 1; i <= 13; i++)
                //{

                //    if(i == 13)
                //    {
                //        GCC.Add(new GraphColor()
                //        {

                //            Color = @"'#7dc576'"
                //        });
                //    }
                //   else 
                //    {
                //        GCC.Add(new GraphColor()
                //        {

                //            Color = @"'#d5d5d5'"
                //        });
                //    }

                //}
                //string ColorList = string.Join(",", GCC.Select(a => a.Color));
                //ViewBag.ColorList = ColorList;

                transactioncountlist = string.Join(",", TransactionCount);

                ViewBag.MonthlyStatementReport = GDR1;
                return View(GDR1);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }


        public ActionResult ReportPreview(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0)
        {

            try
            {
                //int  Month = Convert.ToInt32( DateTime.Now.ToString("MM"));
                int year = Convert.ToInt32(DateTime.Now.Year.ToString());


                string month = DateTime.Now.ToString("MMMM");
                string monthshortname = DateTime.Now.ToString("MMM");
                string CurrentMonthYear = month + " " + year;

                ViewBag.MonthShortName = monthshortname;
                ViewBag.CurrentMonthYear = CurrentMonthYear;

                //string UserPrimaryID = (from users in DbEntity.tblUsers
                //                     where users.UserID == UserId
                //                        select users.User_ID).Single().ToString();




                var WebMoonUser = (from users in DbEntity.tblWebmoonUsers
                                   select users.WebmoonId).Single();
                ViewBag.WebMoonUser = WebMoonUser;


                var TransactionCount = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
                                        select transaction.TransactionCount).ToList();
                var MonthNames = (from transaction in DbEntity.Sp_Get_User_Graph_Data(UserId, ddlTerminalKey, ddlmonthlist, ddlyearlist)
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


        [FilterConfig.AuthorizeActionFilter]
        [FilterConfig.NoDirectAccess]
        [HttpGet]
        public JsonResult SyncReportData(string UserID, DateTime ReportDate)

        {
            string ResponseStaus = "";
            int ReportLength = 0;
            try
            {

                ViewBag.ReportDate = ReportDate;

                //string date = "20210422";

                //var result = DateTime.ParseExact(date,
                //  "yyyyMd",
                //  CultureInfo.InvariantCulture,
                //  DateTimeStyles.AssumeLocal);




                string Reportdate = ReportDate.ToString("yyyy-MM-dd");

                //var Userpassword = (from users in DbEntity.tblUsers
                //                    where users.UserID == UserID
                //                    select users.Password).Single();

                //Get WebMon User 
                var WebmoonUser = DbEntity.tblWebmoonUsers.ToList().FirstOrDefault();

                string UserIDPassword = WebmoonUser.WebmoonId + WebmoonUser.Password;
                string ZipFileName = UserID + "_" + Reportdate + ".zip";



                //Soap Service to get report data
                byte[] baPassword;
                var objSHA256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                baPassword = objSHA256.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(UserIDPassword.Trim()));
                //baPassword = objSHA256.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes("RocketReportsAsadawan88"));
                var objBatchReport = new BatchReport.BatchReportServiceSoapClient();

                ReportLength = objBatchReport.GetReportLength(Reportdate, WebmoonUser.WebmoonId, baPassword);

                if (ReportLength > 0)
                {
                    String ReportBinaryString = Convert.ToBase64String(objBatchReport.GetReportData(Reportdate, WebmoonUser.WebmoonId, baPassword));
                    //String a = Convert.ToBase64String(objBatchReport.GetReportData("2021-04-22", "RocketReports", baPassword));
                    objBatchReport.Close();
                    objBatchReport = null;



                    if (ReportBinaryString != null && ReportBinaryString.Length > 0)
                    {
                        ResponseStaus = "1";
                        byte[] bytes = Convert.FromBase64String(ReportBinaryString);


                        //***Creating Directory Reports-->Users-->UserNameFolder
                        string MainRoot = "/Reports/Users/" + UserID;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));
                        //Creating Directory Reports-->Users-->UserNameFolder ***//

                        string ZipFilePath = MainRoot + "/" + ZipFileName;

                        System.IO.File.WriteAllBytes(Server.MapPath(ZipFilePath), bytes);


                        string ZipFileFolder = "/Reports/Users/" + UserID + "/" + (ZipFileName.Replace(".zip", ""));

                        exists = System.IO.Directory.Exists(Server.MapPath(ZipFileFolder));

                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(ZipFileFolder));


                        System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath(ZipFileFolder));

                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }


                        ZipFile.ExtractToDirectory(Server.MapPath(ZipFilePath), Server.MapPath(ZipFileFolder));

                        //Read Report data from text file 
                        string ReportDateFile = Reportdate.Replace("-", "");

                        string DataReportTextFileName = ZipFileFolder + "/Daily_Detail_ASCII_MintATM_" + ReportDateFile + ".txt";

                        string ReportDatafilepath = Server.MapPath(DataReportTextFileName);

                        bool FileExists = System.IO.File.Exists(ReportDatafilepath);


                        //Check with Rocketreports
                        if (FileExists == false)
                        {
                            DataReportTextFileName = ZipFileFolder + "/Daily_Detail_ASCII_RocketReports_" + ReportDateFile + ".txt";

                            ReportDatafilepath = Server.MapPath(DataReportTextFileName);

                            FileExists = System.IO.File.Exists(ReportDatafilepath);
                        }

                        //Check with Mymint
                        if (FileExists == false)
                        {
                            DataReportTextFileName = ZipFileFolder + "/Daily_Detail_ASCII_Mymint_" + ReportDateFile + ".txt";

                            ReportDatafilepath = Server.MapPath(DataReportTextFileName);

                            FileExists = System.IO.File.Exists(ReportDatafilepath);
                        }


                        if (FileExists == true)
                        {
                            //string text = System.IO.File.ReadAllText(@"C:\Users\HP\source\repos\ATMReportService\ATMReportService\bin\Debug\\pdfFileNameExtract\Daily_Detail_ASCII_RocketReports_20210416.txt");


                            string[] lines = System.IO.File.ReadAllLines(ReportDatafilepath);

                            if (lines.Length > 0)
                            {
                                var UserTerminal = DbEntity.tblReportDatas.Where(a => a.ReportDate == ReportDate);
                                DbEntity.tblReportDatas.RemoveRange(UserTerminal);
                                DbEntity.SaveChanges();


                            }

                            tblReportData RD = new tblReportData();
                            // Display the file contents by using a foreach loop.
                            foreach (string line in lines)
                            {
                                string[] Reportdata = line.Split(',');

                                RD.CompanyName = Reportdata[0].Replace("\"", "").ToString();
                                //RD.HostDate = Convert.ToDateTime(Reportdata[1].ToString());
                                string date = Reportdata[1].Replace("\"", "").ToString();
                                RD.HostDate = DateTime.ParseExact(date, "yyyyMd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);


                                RD.RequestCashWithDrawlAmount = Convert.ToDecimal(Reportdata[2]);
                                RD.DispensedCashAmount = Convert.ToDecimal(Reportdata[3]);
                                RD.MaskedCardHolderCardNumber = Reportdata[4].Replace("\"", "");
                                RD.TransactionSequenceNumber = Reportdata[5].Replace("\"", "");
                                RD.TransactionResponseCode = Reportdata[6].Replace("\"", "");
                                RD.AtmTerminalKey = Reportdata[7].Replace("\"", "");
                                date = Reportdata[8].Replace("\"", "").ToString();
                                RD.TransactionDate = DateTime.ParseExact(date, "yyyyMd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                                RD.TransactionTime = Reportdata[9].Replace("\"", "").ToString();
                                RD.SurchargeFee = Convert.ToDecimal(Reportdata[10]);
                                RD.TransactionNetwork = Reportdata[11].Replace("\"", "");
                                RD.InterchangeFee = Convert.ToDecimal(Reportdata[12]);
                                RD.TransactionType = Reportdata[13].Replace("\"", "");
                                RD.TransactionBankAccountType = Reportdata[14].Replace("\"", "");
                                RD.NotAcceptable = Reportdata[14].Replace("\"", "");
                                RD.CreatedDate = DateTime.Now;
                                RD.CreatedBy = UserID;
                                RD.ReportDate = ReportDate;
                                DbEntity.tblReportDatas.Add(RD);
                                DbEntity.SaveChanges();

                            }
                        }


                        ///Read Terminal Manager File
                        ///
                         //ReportDateFile = Reportdate.Replace("-", "");
                        string DataTerminalTextFileName = ZipFileFolder + "/Daily_Deposit_ASCII_MintATM_" + ReportDateFile + ".txt";



                        string TerminalDatafilepath = Server.MapPath(DataTerminalTextFileName);

                        bool TerminalFileExists = System.IO.File.Exists(TerminalDatafilepath);


                        //Check with Rocketreports
                        if (TerminalFileExists == false)
                        {
                            DataTerminalTextFileName = ZipFileFolder + "/Daily_Deposit_ASCII_RocketReports_" + ReportDateFile + ".txt";

                            TerminalDatafilepath = Server.MapPath(DataTerminalTextFileName);

                            TerminalFileExists = System.IO.File.Exists(TerminalDatafilepath);
                        }

                        //Check with Mymint
                        if (TerminalFileExists == false)
                        {
                            DataTerminalTextFileName = ZipFileFolder + "/Daily_Deposit_ASCII_Mymint_" + ReportDateFile + ".txt";

                            TerminalDatafilepath = Server.MapPath(DataTerminalTextFileName);

                            TerminalFileExists = System.IO.File.Exists(TerminalDatafilepath);
                        }



                        if (TerminalFileExists == true)
                        {
                            //string text = System.IO.File.ReadAllText(@"C:\Users\HP\source\repos\ATMReportService\ATMReportService\bin\Debug\\pdfFileNameExtract\Daily_Detail_ASCII_RocketReports_20210416.txt");


                            string[] lines = System.IO.File.ReadAllLines(TerminalDatafilepath);

                            if (lines.Length > 0)
                            {
                                var TerminalManager = DbEntity.tblTerminalManagers.Where(a => a.ReportDate == ReportDate);
                                DbEntity.tblTerminalManagers.RemoveRange(TerminalManager);
                                DbEntity.SaveChanges();


                            }

                            tblTerminalManager TM = new tblTerminalManager();
                            // Display the file contents by using a foreach loop.
                            foreach (string line in lines)
                            {
                                string[] Reportdata = line.Split(',');

                                TM.Name = Reportdata[0].Replace("\"", "").ToString();
                                TM.Type = Reportdata[1].Replace("\"", "").ToString();
                                TM.TerminalId = Reportdata[2].Replace("\"", "").ToString();
                                TM.FundingType = Reportdata[3].Replace("\"", "").ToString();
                                TM.AccountName = Reportdata[4].Replace("\"", "").ToString();
                                TM.RoutingNumber = Reportdata[5].Replace("\"", "").ToString();
                                TM.TransitNumber = Reportdata[6].Replace("\"", "").ToString();
                                TM.AccountNumber = Reportdata[7].Replace("\"", "").ToString();
                                TM.DepositAmount = Convert.ToDecimal(Reportdata[8]);
                                TM.TransactionDate = Convert.ToDateTime(Reportdata[9].Replace("\"", ""));
                                TM.SettlementDate = Convert.ToDateTime(Reportdata[9].Replace("\"", ""));
                                TM.CreatedDate = DateTime.Now;
                                TM.CreatedBy = UserID;
                                TM.ReportDate = ReportDate;
                                DbEntity.tblTerminalManagers.Add(TM);
                                DbEntity.SaveChanges();

                            }
                        }






                    }
                    else
                    {
                        ResponseStaus = "0";
                    }

                    //return Json(ResponseStaus, JsonRequestBehavior.AllowGet);



                }
                else if (ReportLength < 0)
                {

                    ResponseStaus = Convert.ToString(ReportLength);
                    //return Json(new { ResponseStaus = ReportLength, url = Url.Action("Index", "Report", new { UserID = UserID, ReportDate = ReportDate }) }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { ResponseStaus = ResponseStaus, url = Url.Action("Index", "Report", new { UserID = UserID, ReportDate = ReportDate }) }, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json(new { ResponseStaus = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        //[HttpPost]
        //public ActionResult ReportAllRecord(string UserID, DateTime ReportDate)
        //{

        //    return Json(new { url = Url.Action("Index", "Report", new { UserID = UserID, ReportDate = ReportDate, IsAllRecord = 1 }) });

        //    //string UserID, DateTime ReportDate, string[] ddlTerminalKey
        //}

        public ActionResult DownloadPdf(string UserId, string ddlTerminalKey, int? ddlmonthlist = 0, int? ddlyearlist = 0)
        {

            //Method1
            return new ActionAsPdf("ReportPreview", new { UserId = UserId, ddlTerminalKey = ddlTerminalKey, ddlmonthlist = ddlmonthlist, ddlyearlist = ddlyearlist })
            {
                FileName = "DailyTerminalReportss" + ".pdf"

            };
        }

        public ActionResult DailyTransactionReportPdf()
        {
            //Method1
            return new ActionAsPdf("DailyTransactionReportPreview")
            {
                FileName = "DailyTransactionReportPreview" + ".pdf"

            };

        }

        public ActionResult DailyTransactionReportPreview(string UserID, DateTime ReportDate, string ddlTerminalKey)
        {

            try
            {

                var User_ID = (from users in DbEntity.tblUsers
                               where users.UserID == UserID
                               select users.User_ID).Single();

                ViewBag.ReportDate = ReportDate.ToString("MMM dd, yyyy");

                ViewBag.TerminalKey = ddlTerminalKey;

                if (ddlTerminalKey == null || ddlTerminalKey == "0")
                {
                    ddlTerminalKey = "";
                    var ReportData =

                          //DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == UserID && DbFunctions.TruncateTime(rd.ReportDate) == DbFunctions.TruncateTime(ReportDate)).ToList();

                          ViewBag.ReportData = DbEntity.Sp_Get_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).OrderByDescending(a => a.TransactionSequenceNumber).ToList();
                    //ViewBag.ReportData = ReportData;

                }

                else if (ddlTerminalKey != null)
                {
                    // var ReportData =
                    //                       DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == UserID && DbFunctions.TruncateTime(rd.ReportDate)== DbFunctions.TruncateTime(ReportDate)
                    //&& ddlTerminalKey.Contains(rd.AtmTerminalKey)).ToList();


                    // ViewBag.ReportData = ReportData;
                    ViewBag.ReportData = DbEntity.Sp_Get_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).OrderByDescending(a => a.TransactionSequenceNumber).ToList();
                }

                double TotalDispensed = 0.00;
                double TotalSurcharge = 0.00;
                double TotalSum = 0.00;
                int TotalStatus = 0;
                foreach (var item in ViewBag.ReportData)
                {
                    TotalDispensed = TotalDispensed + Convert.ToDouble(item.DispensedCashAmount);
                    TotalSurcharge = TotalSurcharge + Convert.ToDouble(item.SurchargeFee);
                    TotalSum = TotalSum + Convert.ToDouble(item.DispensedCashAmount) + Convert.ToDouble(item.SurchargeFee);

                    if (item.TransactionStatus == "Approved")
                    {
                        TotalStatus = TotalStatus + 1;
                    }

                }


                ViewBag.TotalDispensed = TotalDispensed;
                ViewBag.TotalSurcharge = TotalSurcharge;
                ViewBag.TotalSum = TotalSum;
                ViewBag.TotalStatus = TotalStatus;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();

        }

        [HttpGet]
        //      return RedirectToAction("MonthlyTransactionReportPdf", "Report", new
        //                {
        //                    UserId = UserId,
        //                    ddlBankList = ddlBankList,
        //                    ddlSettlementType = ddlSettlementType,
        //                    ddlTerminalKey = ddlTerminalKey,
        //                    ddlmonthlist = ddlmonthlist,
        //                    ddlyearlist = ddlyearlist,
        //                    optradio = optradio
        //});
        public ActionResult MonthlyTransactionReportPdf(string UserId, string ddlTerminalKey = "", int? ddlBankList = 0, int? ddlSettlementType = 0, int? ddlmonthlist = 0, int? ddlyearlist = 0, int? optradio = 1, int? ajax = 1)
        {


            //Method1
            return new ActionAsPdf("MonthStatements", new { UserId = UserId, ddlBankList = ddlBankList, ddlSettlementType = ddlSettlementType, ddlTerminalKey = ddlTerminalKey, ddlmonthlist = ddlmonthlist, ddlyearlist = ddlyearlist, optradio = optradio, ajax = ajax })
            {
                FileName = "MonthlyTransactionReportPreview" + ".pdf"

            };

        }


        [HttpGet]
        [FilterConfig.AuthorizeActionFilter]
        [FilterConfig.NoDirectAccess]
        public ActionResult DailyDeposit(string UserID, DateTime ReportDate, string ddlTerminalKey, int? IsAllRecord = 0, int? optradio = 1)
        {

            try
            {


                var User_ID = (from users in DbEntity.tblUsers
                               where users.UserID == UserID
                               select users.User_ID).Single();

                ViewBag.ReportDate = ReportDate;
                ViewBag.UserID = UserID;
                ViewBag.TerminalList = DbEntity.Sp_Get_User_Assigned_Terminal_List(User_ID).ToList();


                ViewBag.ddlTerminlKey = ddlTerminalKey;



                var ReportData = DbEntity.tblReportDatas.Where(rd => rd.CreatedBy == "").ToList();

                ViewBag.ReportData = ReportData;


                if (IsAllRecord != 1)
                {
                    if (ddlTerminalKey == null || ddlTerminalKey == "0")
                    {
                        ddlTerminalKey = "";

                        ViewBag.ReportData = DbEntity.Sp_Get_Daily_Deposit_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).ToList();



                    }

                    else if (ddlTerminalKey != null)
                    {

                        ViewBag.ReportData = DbEntity.Sp_Get_Daily_Deposit_Report_Data_List(ReportDate, ddlTerminalKey, User_ID).ToList();

                    }

                }






                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while getting Terminals list : " + ex.Message;
            }
            return View();
        }

    }
}