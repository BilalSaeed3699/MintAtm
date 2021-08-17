using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    [FilterConfig.AuthorizeActionFilter]
    [FilterConfig.NoDirectAccess]

    public class TerminalController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();
        // GET: Terminal




        [HttpPost]
        public JsonResult SaveTerminalComments(string SerialNumber,string Comments,int RelationId,int ProgramId,int CommunicationId,decimal Surcharge,string Email="", string TerminalKey="")

        {
            try
            {

                // Query for a specific customer.
                var terminals =
                    (from c in DbEntity.tblTerminals
                     where c.TerminalKey == TerminalKey
                     select c).First();

                // Change the name of the contact.
                terminals.Comment = Comments;
                terminals.RelationshipId = RelationId;
                terminals.ProgramaTypeId = ProgramId;
                terminals.CommunicationTypeId = CommunicationId;

                if (Surcharge == null)
                {
                    Surcharge = 0;
                }
                
                terminals.Surcharge = Surcharge;
                
                terminals.EmailAddress = Email;
                if(SerialNumber==null)
                {
                    SerialNumber = "";
                }
                terminals.CommunicationSerialNumber = SerialNumber;

                // Ask the DataContext to save all the changes.
                DbEntity.SaveChanges();



                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }



        [HttpGet]
        public JsonResult GetTerminalInformation(string TerminalKey)

        {
            try
            {

                List<tblTerminal> TM = new List<tblTerminal>();

                // Query for a specific customer.
                TM = DbEntity.tblTerminals.ToList().Where(t => t.TerminalKey == TerminalKey).ToList();
                return Json(TM, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

        }




        [HttpGet]
        public JsonResult SyncTerminalData()

        {
            try
            {

                //Soap Service to get Terminal List 


                var WebmoonUser = DbEntity.tblWebmoonUsers.ToList().FirstOrDefault();
                string  status ="0";
               

                RestockReport.TerminalsList.DataQuerySoapClient soap = new RestockReport.TerminalsList.DataQuerySoapClient();
                var Reslult = soap.DailyTerminalConfig(WebmoonUser.WebmoonId, WebmoonUser.Password);

                tblTerminal TT = new tblTerminal();
                tblTerminalSurchargeAccount TTSC = new tblTerminalSurchargeAccount();

                var TerminalKey = "";

                if (Reslult.Length > 1)
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
                    status = "1";
                }
                else if (Reslult.Length <= 1)
                {
                    status = "0";


                }




                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult Index()
        {
            try
            {

                //Soap Service to get Terminal List 

                //RestockReport.TerminalsList.DataQuerySoapClient soap = new RestockReport.TerminalsList.DataQuerySoapClient();
                //var Reslult = soap.DailyTerminalConfig("RocketReports", "Asadawan88");

                //if (Reslult.Length>0)
                //{
                //    int Lne = Reslult.Length;

                //}

                ViewBag.TerminalsList = DbEntity.tblTerminals.Where(t=> t.IsActive == 1).ToList();




                ViewBag.RelationShipDropdownList = new ModelHelper().RelationshipOwner(DbEntity.tblCompanies).Distinct().ToList();
                ViewBag.ProgramTypeDropdownList = new ModelHelper().ProgramType(DbEntity.tblProgramTypes).Distinct().ToList();
                ViewBag.CommunicationTypeDropdownList = new ModelHelper().CommunicationType(DbEntity.tblCommunicationTypes).Distinct().ToList();
                ViewBag.TerminalDropdownList = new ModelHelper().ToSelectTerminal(DbEntity.tblTerminals).Distinct().ToList();
                ViewBag.LocationType = new ModelHelper().ToSelectLocationTypeList(DbEntity.tblTerminals).Distinct().ToList();
                ViewBag.RegionList = new ModelHelper().ToSelectRegionList(DbEntity.tblTerminals).Distinct().ToList();
                ViewBag.MakeAndModel = new ModelHelper().ToSelectMakeAndModelList(DbEntity.tblTerminals).Distinct().ToList();


               
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception occur while getting Terminals list : " + ex.Message;
            }
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}