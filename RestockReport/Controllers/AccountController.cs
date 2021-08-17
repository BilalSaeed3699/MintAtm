using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    public class AccountController : Controller
    {
        private RestockReportsEntities dbEntities = new RestockReportsEntities();
        public ActionResult Index()
        {


            //Soap Service to get report data 
            //byte[] baPassword;
            //var objSHA256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            //baPassword = objSHA256.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes("RocketReportsAsadawan88"));
            //var objBatchReport = new BatchReport.BatchReportServiceSoapClient();
            //String a = Convert.ToBase64String(objBatchReport.GetReportData("2021-04-22", "RocketReports", baPassword));
            //objBatchReport.Close();
            //objBatchReport = null;



            //Soap Service to get Terminal List 

            //RestockReport.TerminalsList.DataQuerySoapClient soap = new RestockReport.TerminalsList.DataQuerySoapClient();
            //var Reslult = soap.DailyTerminalConfig("RocketReports", "Asadawan88");

            //tblTerminal TT = new tblTerminal();
            //tblTerminalSurchargeAccount TTSC = new tblTerminalSurchargeAccount();

            //var TerminalKey = "";
            //foreach (var items in Reslult)
            //{
            //    TerminalKey = items.TerminalID;
            //    TT.TerminalKey = items.TerminalID;
            //    TT.LocationName = items.LocationName;
            //    TT.LocationType = items.LocationType;
            //    TT.Address = items.Address;
            //    TT.City = items.City;
            //    TT.Region = items.Province;
            //    TT.PostalCode = items.PostalCode;
            //    TT.MakeAndModel = items.Make + " - " + items.Model;
            //    TT.SerialNumber = items.SerialNo;
            //    TT.IsActive = 1;
            //    TT.IsDeleted = 0;
            //    TT.CreatedDate = DateTime.Now;
            //    TT.CreatedBy = "Hasan";
            //    TT.Comment = "";
            //    dbEntities.tblTerminals.Add(TT);
            //    dbEntities.SaveChanges();

            //    foreach (var details in items.SurchargeAccounts)
            //    {
            //        TTSC.TerminalKey = TerminalKey;
            //        TTSC.AccountHolder = details.AccountHolder;
            //        TTSC.AccountRTA = details.AccountRTA;
            //        TTSC.SplitType = details.SplitType;
            //        TTSC.SplitAmount = Convert.ToDouble(details.SplitAmount);
            //        TTSC.PaymentSchedule = details.PaymentSchedule;
            //        TTSC.IsSurchargeAccount = 1;
            //        TTSC.IsInterchangeAccount = 0;

            //        var SurchargeAccountHolder = details.AccountHolder;

            //        dbEntities.tblTerminalSurchargeAccounts.Add(TTSC);
            //        dbEntities.SaveChanges();
            //    }

            //    foreach (var details in items.InterchangeAccounts)
            //    {
            //        TTSC.TerminalKey = TerminalKey;
            //        TTSC.AccountHolder = details.AccountHolder;
            //        TTSC.AccountRTA = details.AccountRTA;
            //        TTSC.SplitType = details.SplitType;
            //        TTSC.SplitAmount = Convert.ToDouble(details.SplitAmount);
            //        TTSC.PaymentSchedule = details.PaymentSchedule;
            //        TTSC.IsSurchargeAccount = 0;
            //        TTSC.IsInterchangeAccount = 1;
            //        dbEntities.tblTerminalSurchargeAccounts.Add(TTSC);
            //        dbEntities.SaveChanges();
            //    }


            //}


            return View();
        }


        [HttpPost]
        public ActionResult Index(string userid, string password)
        {
            try
            {
                tblUser User = dbEntities.tblUsers.Where(x => x.UserID == userid && x.Password == password).FirstOrDefault();



                if (User != null)
                {
                      if (User.IsDeleted == 1)
                    {
                        ViewBag.IsError = "User is removed";
                        return View();
                    }
                    else if(User.IsActive == true)
                    {

                        var TitleName = "";
                        if (User.Title == 0)
                        {
                            TitleName = "";
                        }
                        else
                        {
                            TitleName = (from title in dbEntities.tblTitles
                                             where title.TitleID == User.Title
                                             select title.TitleName).Single();

                        }
                       
                        Session["UserName"] = User.FirstName;
                        Session["Title"] = TitleName;


                        var UserID = (from users in dbEntities.tblUsers
                                      where users.UserID == userid
                                      select users.UserID).Single();
                        Session["UserID"] = UserID;


                        var UserPrimaryID = (from users in dbEntities.tblUsers
                                             where users.UserID == userid
                                             select users.User_ID).Single();
                        Session["UserPrimaryID"] = UserPrimaryID;


                        var DailyReportMenu = dbEntities.Sp_Get_User_Dashboard_Menu_List(User.Access_Level_ID).Where(a => a.Page_ID == 1004).ToList();
                        var  MonthlyReportMenu = dbEntities.Sp_Get_User_Dashboard_Menu_List(User.Access_Level_ID).Where(a => a.Page_ID == 1003).ToList();

                        Session["DailyReportMenu"] = DailyReportMenu;
                        Session["MonthlyReportMenu"] = MonthlyReportMenu;

                        Session["Menu"] = dbEntities.Sp_Get_User_Dashboard_Menu_List(User.Access_Level_ID).ToList();
                        return RedirectToAction("Index", "Home", new { UserId = Convert.ToString(User.User_ID) });
                    }
                    else if (User.IsActive == false)
                    {
                        ViewBag.IsError = "Your Account is deactivated please contact to Admin";
                        return View();
                    }
                   
                }
                else
                {
                    ViewBag.IsError = "Your Credentials are incorrect";
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.IsError = ex.Message;
                return View();
            }


        }
        [FilterConfig.NoDirectAccess]
        public ActionResult Changepassword()
        {
            return View();
        }
        [FilterConfig.NoDirectAccess]
        [HttpPost]
        public ActionResult Changepassword(string oldPassword, string newPassword)
        {
            tblUser User = dbEntities.tblUsers.Where(x => x.Password == oldPassword).FirstOrDefault();
            if (User == null)
            {
                ViewBag.IsError = "Your Old Password is incorrect";
                return View();
            }
            else
            {
                User.Password = newPassword;
                dbEntities.Entry(User).State = EntityState.Modified;
                dbEntities.SaveChanges();
                ViewBag.IsSuccess = "Password change successfully";
                return View();
            }
        }
        public ActionResult Forgetpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgetpassword(string email)
        {
            tblUser User = dbEntities.tblUsers.Where(x => x.Email_Adress == email).FirstOrDefault();
            if (User != null)
            {
                if (User.IsActive == true)
                {
                    SendForgetPasswordEmail(User.User_ID, User.Email_Adress, User.FirstName);
                    ViewBag.IsSuccess = "Password change link is sent to your email address please check your email..!!";
                    return View();
                }
                else if (User.IsActive == false)
                {
                    ViewBag.IsError = "Your Account is deactivated please contact to Admin";
                    return View();
                }
            }

            else
            {
                ViewBag.IsError = "Your email is not registered";
                return View();
            }
            return View();
        }
        private void SendForgetPasswordEmail(int userId, string email, string name)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["RocketEntities"].ConnectionString;
                Guid activationCode = Guid.NewGuid();

                tblActivationCode ActivationCodedata = new tblActivationCode();
                ActivationCodedata.Code = activationCode;
                ActivationCodedata.UID = userId;

                dbEntities.tblActivationCodes.Add(ActivationCodedata);
                dbEntities.SaveChanges();




                tblSetting sa = dbEntities.tblSettings.Find(1);
                //using (MailMessage mm = new MailMessage(sa.Email, email))
                //{
                //    string link = Request.Url.ToString();
                //    link = link.Replace("Forgetpassword", "ChangePasswordnew");
                //    mm.Subject = "Password reset";
                //    string body = "Hello " + name + ",";
                //    body += "<br /><br />Please click the following link to reset your password";
                //    body += "<br /><a href = '" + link + "?ActivationCode=" + activationCode + "'>Click here to activate your account.</a>";
                //    body += "<br /><br />Thanks";
                //    mm.Body = body;
                //    mm.IsBodyHtml = true;
                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = sa.SMTP;
                //    smtp.EnableSsl = true;

                //    //smtp.EnableSsl = Convert.ToBoolean(sa.IsActive);
                //    NetworkCredential NetworkCred = new NetworkCredential(sa.Email, sa.Password);
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = NetworkCred;
                //    smtp.Port = Convert.ToInt32(sa.Port);
                //    smtp.Send(mm);
                //}


                using (MailMessage mm = new MailMessage(sa.Email, email))
                {
                    string link = Request.Url.ToString();
                    link = link.Replace("Forgetpassword", "ChangePasswordnew");
                    mm.Subject = "Password reset";
                    string body = "Hello " + name + ",";
                    body += "<br /><br />Please click the following link to reset your password";
                    body += "<br /><a href = '" + link + "?ActivationCode=" + activationCode + "'>Click here to activate your account.</a>";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = sa.SMTP;
                    smtp.EnableSsl = Convert.ToBoolean(sa.IsActive);
                    NetworkCredential NetworkCred = new NetworkCredential(sa.Email, sa.Password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(sa.Port);
                    smtp.Send(mm);
                }


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
        }




        public ActionResult ChangePasswordnew()
        {
            string activationCode = !string.IsNullOrEmpty(Request.QueryString["ActivationCode"]) ? Request.QueryString["ActivationCode"] : Guid.Empty.ToString();
            ViewBag.ActivationCode = activationCode;
            ViewBag.error = "Please enter your New Password..!!";
            return View();
        }

        [HttpPost]
        public ActionResult ChangePasswordnew(string newPassword, string ActivationCode)
        {

            //if (newPassword != confirm_password)
            //{
            //    ViewBag.error = "Password not match..!!";
            //    return View();
            //}


            //string UserId = Convert.ToString(Session["UserID"]);

            //// Query for a specific customer.
            //var users =
            //    (from u in dbEntities.tblUsers
            //     where u.UserID == UserId
            //     select u).First();

            //// Change the name of the contact.
            //users.Password = newPassword;

            //// Ask the DataContext to save all the changes.
            //dbEntities.SaveChanges();
            //ViewBag.error = "Password changed";



            string code = Request.QueryString["ActivationCode"];

            int UserID = 0;
            string constr = ConfigurationManager.ConnectionStrings["RocketEntities"].ConnectionString;
            string activationCode = ActivationCode;
            //string activationCode = !string.IsNullOrEmpty(Request.QueryString["ActivationCode"]) ? Request.QueryString["ActivationCode"] : Guid.Empty.ToString();
            if (activationCode != "00000000-0000-0000-0000-000000000000")
            {
                List<tblActivationCode> info = dbEntities.tblActivationCodes.SqlQuery("Select * from tblActivationCode where Code = '" + activationCode + "'").ToList();
                foreach (var x in info)
                {
                    UserID = Convert.ToInt32(x.UID);
                }
                if (UserID > 0)
                {
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM tblActivationCode WHERE Code = @Code; update tbluser set Password='" + newPassword + "' where User_ID=" + UserID + ";"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@Code", activationCode);
                                cmd.Connection = con;
                                con.Open();
                                int rowsAffected = cmd.ExecuteNonQuery();
                                con.Close();
                                if (rowsAffected == 2)
                                {
                                    return RedirectToAction("Index", "Account");
                                }
                                else
                                {
                                    return View();
                                }
                            }
                        }

                    }
                }
                else
                {
                    ViewBag.error = "Password already changed please make new request..!!";
                    return View();
                }
            }
            return RedirectToAction("Index", "Account");
        }
    }
}