using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    public class ReportDataJobController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();
        // GET: ReportDataJob
        public ActionResult Index()
        {
            
            string ResponseStaus = "";
            int ReportLength = 0;
            int totalrecords = 0;
            DateTime ReportDate;
            try
            {
                var UserId = (from user in DbEntity.tblWebmoonUsers
                                        select user.WebmoonId).SingleOrDefault();

                 ReportDate = DateTime.Today.AddDays(-2) ;
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
                string ZipFileName = UserId + "_" + Reportdate + ".zip";



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
                        string MainRoot = "/Reports/Users/" + UserId;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));
                        //Creating Directory Reports-->Users-->UserNameFolder ***//

                        string ZipFilePath = MainRoot + "/" + ZipFileName;

                        System.IO.File.WriteAllBytes(Server.MapPath(ZipFilePath), bytes);


                        string ZipFileFolder = "/Reports/Users/" + UserId + "/" + (ZipFileName.Replace(".zip", ""));

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

                            totalrecords = lines.Count();
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
                                RD.CreatedBy = UserId;
                                RD.ReportDate = ReportDate;
                                DbEntity.tblReportDatas.Add(RD);
                                DbEntity.SaveChanges();

                            }
                        }

                        tblReportJobLog tbl = new tblReportJobLog();
                        tbl.Jobtime = DateTime.Now;
                        tbl.Jobfunctionname = "Index (2 days Old)";
                        tbl.Recordcount = totalrecords;
                        tbl.Status = "Success";
                        tbl.Soapserviceresponsetype = ReportLength;
                        tbl.ScheduleTime = "11AM Canadian";
                        tbl.ReportDate = ReportDate;
                        DbEntity.tblReportJobLogs.Add(tbl);
                        DbEntity.SaveChanges();
                    }
                    else
                    {
                        ResponseStaus = "0";
                        tblReportJobLog tbl = new tblReportJobLog();
                        tbl.Jobtime = DateTime.Now;
                        tbl.Jobfunctionname = "Index (2 days Old)";
                        tbl.Recordcount = totalrecords;
                        tbl.Status = "Fail ";
                        tbl.Soapserviceresponsetype =Convert.ToInt32(ResponseStaus);
                        tbl.ScheduleTime = "11AM Canadian";
                        tbl.ReportDate = ReportDate;
                        DbEntity.tblReportJobLogs.Add(tbl);
                        DbEntity.SaveChanges();


                    }

                    //return Json(ResponseStaus, JsonRequestBehavior.AllowGet);



                }
                else if (ReportLength < 0)
                {

                    tblReportJobLog tbl = new tblReportJobLog();
                    tbl.Jobtime = DateTime.Now;
                    tbl.Jobfunctionname = "Index (2 days Old)";
                    tbl.Recordcount = totalrecords;
                    tbl.Status = "Fail";
                    tbl.Soapserviceresponsetype = ReportLength;
                    tbl.ScheduleTime = "11AM Canadian";
                    tbl.ReportDate = ReportDate;
                    DbEntity.tblReportJobLogs.Add(tbl);
                    DbEntity.SaveChanges();

                    ResponseStaus = Convert.ToString(ReportLength);
                    //return Json(new { ResponseStaus = ReportLength, url = Url.Action("Index", "Report", new { UserID = UserID, ReportDate = ReportDate }) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ResponseStaus = "0";
                    ResponseStaus = "0";
                    tblReportJobLog tbl = new tblReportJobLog();
                    tbl.Jobtime = DateTime.Now;
                    tbl.Jobfunctionname = "Index (2 days Old)";
                    tbl.Recordcount = totalrecords;
                    tbl.Status = "Fail ";
                    tbl.Soapserviceresponsetype = Convert.ToInt32(ResponseStaus);
                    tbl.ScheduleTime = "11AM Canadian";
                    tbl.ReportDate = ReportDate;
                    DbEntity.tblReportJobLogs.Add(tbl);
                    DbEntity.SaveChanges();
                }

                //return Json(ResponseStaus, JsonRequestBehavior.AllowGet);




                ViewBag.Message = ResponseStaus;
                return View();
            }
            catch (Exception ex)
            {
                ReportDate = DateTime.Today.AddDays(-1);
                tblReportJobLog tbl = new tblReportJobLog();
                tbl.Jobtime = DateTime.Now;
                tbl.Jobfunctionname = "Index (2 days Old)";
                tbl.Recordcount = 0;
                tbl.Status = "Exception Occur" + ex.Message;
                tbl.Soapserviceresponsetype = ReportLength;
                tbl.ScheduleTime = "11AM Canadian";
                tbl.ReportDate = ReportDate;
                DbEntity.tblReportJobLogs.Add(tbl);
                DbEntity.SaveChanges();

                ViewBag.Message = ex.Message;
            }
            return View();
        }



        public ActionResult Daily()
        {

            string ResponseStaus = "";
            int ReportLength = 0;
            int totalrecords = 0;
            DateTime ReportDate;
            try
            {
                var UserId = (from user in DbEntity.tblWebmoonUsers
                              select user.WebmoonId).SingleOrDefault();

                 ReportDate = DateTime.Today.AddDays(-1);
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
                string ZipFileName = UserId + "_" + Reportdate + ".zip";



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
                        string MainRoot = "/Reports/Users/" + UserId;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(MainRoot));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(MainRoot));
                        //Creating Directory Reports-->Users-->UserNameFolder ***//

                        string ZipFilePath = MainRoot + "/" + ZipFileName;

                        System.IO.File.WriteAllBytes(Server.MapPath(ZipFilePath), bytes);


                        string ZipFileFolder = "/Reports/Users/" + UserId + "/" + (ZipFileName.Replace(".zip", ""));

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


                            totalrecords = lines.Count();
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
                                RD.CreatedBy = UserId;
                                RD.ReportDate = ReportDate;
                                DbEntity.tblReportDatas.Add(RD);
                                DbEntity.SaveChanges();

                            }




                        }

                        tblReportJobLog tbl = new tblReportJobLog();
                        tbl.Jobtime = DateTime.Now;
                        tbl.Jobfunctionname = "Daily (1 days Old)";
                        tbl.Recordcount = totalrecords;
                        tbl.Status = "Success";
                        tbl.Soapserviceresponsetype = ReportLength;
                        tbl.ScheduleTime = "7PM Canadian";
                        tbl.ReportDate = ReportDate;
                        DbEntity.tblReportJobLogs.Add(tbl);
                        DbEntity.SaveChanges();
                    }
                    else
                    {
                       

                        ResponseStaus = "0";

                        tblReportJobLog tbl = new tblReportJobLog();
                        tbl.Jobtime = DateTime.Now;
                        tbl.Jobfunctionname = "Daily (1 days Old)";
                        tbl.Recordcount = totalrecords;
                        tbl.Status = "No Record Found";
                        tbl.Soapserviceresponsetype = Convert.ToInt32(ResponseStaus);
                        tbl.ScheduleTime = "7PM Canadian";
                        tbl.ReportDate = ReportDate;
                        DbEntity.tblReportJobLogs.Add(tbl);
                        DbEntity.SaveChanges();
                    }

                    //return Json(ResponseStaus, JsonRequestBehavior.AllowGet);



                }
                else if (ReportLength < 0)
                {

                    ResponseStaus = Convert.ToString(ReportLength);
                    //return Json(new { ResponseStaus = ReportLength, url = Url.Action("Index", "Report", new { UserID = UserID, ReportDate = ReportDate }) }, JsonRequestBehavior.AllowGet);


                    tblReportJobLog tbl = new tblReportJobLog();
                    tbl.Jobtime = DateTime.Now;
                    tbl.Jobfunctionname = "Daily (1 days Old)";
                    tbl.Recordcount = totalrecords;
                    tbl.Status = "No Record Found";
                    tbl.Soapserviceresponsetype = ReportLength;
                    tbl.ScheduleTime = "7PM Canadian";
                    tbl.ReportDate = ReportDate;
                    DbEntity.tblReportJobLogs.Add(tbl);
                    DbEntity.SaveChanges();

                }
                else
                {
                    ResponseStaus = "0";


                    tblReportJobLog tbl = new tblReportJobLog();
                    tbl.Jobtime = DateTime.Now;
                    tbl.Jobfunctionname = "Daily (1 days Old)";
                    tbl.Recordcount = totalrecords;
                    tbl.Status = "No Record Found";
                    tbl.Soapserviceresponsetype = Convert.ToInt32(ResponseStaus);
                    tbl.ScheduleTime = "7PM Canadian";
                    tbl.ReportDate = ReportDate;
                    DbEntity.tblReportJobLogs.Add(tbl);
                    DbEntity.SaveChanges();
                }

                //return Json(ResponseStaus, JsonRequestBehavior.AllowGet);




                ViewBag.Message = ResponseStaus;
                return View();
            }
            catch (Exception ex)
            {

                ReportDate = DateTime.Today.AddDays(-1);

                tblReportJobLog tbl = new tblReportJobLog();
                tbl.Jobtime = DateTime.Now;
                tbl.Jobfunctionname = "Daily (1 days Old)";
                tbl.Recordcount = totalrecords;
                tbl.Status ="Exception Occur"  + ex.Message;
                tbl.Soapserviceresponsetype = ReportLength;
                tbl.ScheduleTime = "7PM Canadian";
                tbl.ReportDate = ReportDate;
                DbEntity.tblReportJobLogs.Add(tbl);
                DbEntity.SaveChanges();
                ViewBag.Message = ex.Message;
            }
            return View();
        }
    }
}