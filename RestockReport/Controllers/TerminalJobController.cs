using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    public class TerminalJobController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();
        // GET: Terminal

        // GET: TerminalJob
        public ActionResult Index()
        {
            try
            {

                //Soap Service to get Terminal List 


                var WebmoonUser = DbEntity.tblWebmoonUsers.ToList().FirstOrDefault();



                RestockReport.TerminalsList.DataQuerySoapClient soap = new RestockReport.TerminalsList.DataQuerySoapClient();
                var Reslult = soap.DailyTerminalConfig(WebmoonUser.WebmoonId, WebmoonUser.Password);

                tblTerminal TT = new tblTerminal();
                tblTerminalSurchargeAccount TTSC = new tblTerminalSurchargeAccount();

                var TerminalKey = "";

                if (Reslult.Length > 0)
                {
                    DbEntity.Database.ExecuteSqlCommand("TRUNCATE TABLE [tblTerminal]");
                    DbEntity.Database.ExecuteSqlCommand("TRUNCATE TABLE [tblTerminalSurchargeAccount]");


                    foreach (var items in Reslult)
                    {
                        TerminalKey = items.TerminalID;
                        TT.TerminalKey = items.TerminalID;
                        TT.LocationName = items.LocationName;
                        TT.LocationType = items.LocationType;
                        TT.ContactName = items.LocationContactName;
                        TT.ContactPhone = items.LocationContactPhone;
                        TT.Address = items.Address;
                        TT.City = items.City;
                        TT.Region = items.Province;
                        TT.PostalCode = items.PostalCode;
                        TT.MakeAndModel = items.Make + " - " + items.Model;
                        TT.SerialNumber = items.SerialNo;
                        TT.IsActive = 1;
                        //TT.EscalationContactName = items.EscalationContactName;
                        TT.IsDeleted = 0;
                        TT.CreatedDate = DateTime.Now;
                        TT.CreatedBy = "Hasan";
                        TT.Comment = "";
                        TT.RelationshipId = 0;
                        TT.ProgramaTypeId = 0;
                        TT.CommunicationTypeId = 0;
                        TT.EmailAddress = "";
                        TT.Surcharge = 0;
                        TT.CommunicationSerialNumber = "";
                        TT.EmailAddress = "";
                        DbEntity.tblTerminals.Add(TT);
                        DbEntity.SaveChanges();

                        foreach (var details in items.SurchargeAccounts)
                        {
                            TTSC.TerminalKey = TerminalKey;
                            TTSC.AccountHolder = details.AccountHolder;
                            TTSC.AccountRTA = details.AccountRTA;
                            TTSC.SplitType = details.SplitType;
                            TTSC.SplitAmount = Convert.ToDouble(details.SplitAmount);
                            TTSC.PaymentSchedule = details.PaymentSchedule;
                            TTSC.IsSurchargeAccount = 1;
                            TTSC.IsInterchangeAccount = 0;

                            var SurchargeAccountHolder = details.AccountHolder;

                            DbEntity.tblTerminalSurchargeAccounts.Add(TTSC);
                            DbEntity.SaveChanges();
                        }

                        foreach (var details in items.InterchangeAccounts)
                        {
                            TTSC.TerminalKey = TerminalKey;
                            TTSC.AccountHolder = details.AccountHolder;
                            TTSC.AccountRTA = details.AccountRTA;
                            TTSC.SplitType = details.SplitType;
                            TTSC.SplitAmount = Convert.ToDouble(details.SplitAmount);
                            TTSC.PaymentSchedule = details.PaymentSchedule;
                            TTSC.IsSurchargeAccount = 0;
                            TTSC.IsInterchangeAccount = 1;
                            DbEntity.tblTerminalSurchargeAccounts.Add(TTSC);
                            DbEntity.SaveChanges();
                        }


                    }

                }

                ViewBag.Message = "Terminal Update Successfully";


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
    }
}