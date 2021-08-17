using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{

    [FilterConfig.AuthorizeActionFilter]
    [FilterConfig.NoDirectAccess]

    public class BankAccountManagerController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();
        // GET: BankAccountManager
        public ActionResult Index()
        {
            try
            {
                ViewBag.BankDetailList = DbEntity.Sp_Get_Bank_Detail_List().ToList();
                return View();

            }
            catch (Exception ex)
            {

            }

            ViewBag.BankDetailList = DbEntity.Sp_Get_Bank_Detail_List().ToList();
            return View();
            
        }


        [HttpPost]
        public ActionResult UpdateBankStatus(int? BankAccountId)
        {
            try
            {


                tblBankAccountDetail user = (from c in DbEntity.tblBankAccountDetails
                                where c.BankAccountId == BankAccountId
                                             select c).FirstOrDefault();

                user.IsDeleted = 1;
                DbEntity.SaveChanges();



                return Json(new { status = 1, url = Url.Action("Index", "BankAccountManager") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 1, url = Url.Action("Index", "BankAccountManager") });
            }

            return Json(new { status = 1, url = Url.Action("Index", "BankAccountManager") });
        }


        public ActionResult Edit(int? UserId)
        {
            try
            {
                ViewBag.BankAccountUsage = DbEntity.Sp_Get_Bank_Account_UsageDetail(UserId).ToList();
                ViewBag.RegionList = new ModelHelper().ToSelectUserRegionList(DbEntity.tblRegions).Distinct().ToList();
                ViewBag.AccountType = DbEntity.tblAccountTypes.ToList();
                ViewBag.AccountStatus = DbEntity.tblAccountStatus.ToList();
                ViewBag.BankAccountDetails = DbEntity.tblBankAccountDetails.Where(a => a.BankAccountId == UserId).ToList();

               var AccountHoldername = DbEntity.tblBankAccountDetails.Where(a => a.BankAccountId == UserId).Select(x => x.Accountholdername).FirstOrDefault();
                ViewBag.AccountHoldername = AccountHoldername;
                ViewBag.StackHolder = DbEntity.tblStackHolders.ToList();
                ViewBag.BankAccountList = DbEntity.tblBankAccountLists.ToList();
                ViewBag.AccountHolderList = DbEntity.Sp_Get_AccountHolder_List(1, AccountHoldername).ToList();
            }
            catch (Exception ex)
            {

            }

            ViewBag.BankDetailList = DbEntity.Sp_Get_Bank_Detail_List().ToList();
            return View();

        }

        public ActionResult New()
        {
            try
            {
                ViewBag.RegionList = new ModelHelper().ToSelectUserRegionList(DbEntity.tblRegions).Distinct().ToList();
                ViewBag.AccountType = DbEntity.tblAccountTypes.ToList();
                ViewBag.AccountStatus = DbEntity.tblAccountStatus.ToList();
                ViewBag.StackHolder = DbEntity.tblStackHolders.ToList();
                ViewBag.BankAccountList = DbEntity.tblBankAccountLists.ToList();
                ViewBag.AccountHolderList = DbEntity.Sp_Get_AccountHolder_List(0,"").ToList();
                return View();
            }
            catch (Exception ex)
            {

            }

            return View();

        }

        [HttpGet]
        public JsonResult GetAccountHolderInformation(string AccountHolderName)

        {
            try
            {

           

                // Query for a specific customer.
                var Result =  DbEntity.Sp_Get_AccountHolder_Details(2, AccountHolderName).ToList();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult SaveBankInfo(tblBankAccountDetail BankInformation)
        {
            try
            {

                BankInformation.CreateDate = DateTime.Now;
                BankInformation.CreatedBy = Session["UserID"].ToString();
                DbEntity.tblBankAccountDetails.Add(BankInformation);
                DbEntity.SaveChanges();




                return Json(new { status = 1, BankName = BankInformation.Bankname, url = Url.Action("Index", "BankAccountManager") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 0, data = ex.Message });
            }

            return Json(new { url = Url.Action("Index", "BankAccountManager") });
        }



        [HttpGet]
        public JsonResult VerifyBankName(string BankName)

        
        {
            try
            {

                bool exists = (from bank in DbEntity.tblBankAccountDetails
                               where bank.Bankname == BankName
                               select bank).Any();

                if (exists == true)
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (exists == false)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult UpdateBankInfo(tblBankAccountDetail BankInformation)

        {
            try
            {
                

                // Query for a specific customer.
                var BankAccountDetail =
                    (from c in DbEntity.tblBankAccountDetails
                     where c.BankAccountId == BankInformation.BankAccountId
                     select c).First();
                
                BankAccountDetail.AccountTypeId = BankInformation.AccountTypeId;
                BankAccountDetail.Bankname = BankInformation.Bankname;
                BankAccountDetail.Routingnumber = BankInformation.Routingnumber;
                BankAccountDetail.Address = BankInformation.Address;
                BankAccountDetail.Transitnumber= BankInformation.Transitnumber;
                BankAccountDetail.City = BankInformation.City;
                BankAccountDetail.Accountnumber = BankInformation.Accountnumber;
                BankAccountDetail.RegionId = BankInformation.RegionId;
                BankAccountDetail.CurrentstatusId   = BankInformation.CurrentstatusId;
                BankAccountDetail.Postalcode= BankInformation.Postalcode;
                BankAccountDetail.Accountholdername = BankInformation.Accountholdername;
                BankAccountDetail.Nickname = BankInformation.Nickname;
                BankAccountDetail.Stackholdertypeid = BankInformation.Stackholdertypeid;
                BankAccountDetail.Bankaccountcontactname = BankInformation.Bankaccountcontactname;
                BankAccountDetail.Phonenumber= BankInformation.Phonenumber;
                BankAccountDetail.EmailAddress = BankInformation.EmailAddress;
                BankAccountDetail.Entityname = BankInformation.Entityname;
                BankAccountDetail.MailingAddress = BankInformation.MailingAddress;
                BankAccountDetail.Entitycity = BankInformation.Entitycity;
                BankAccountDetail.Entityregion = BankInformation.Entityregion;
                BankAccountDetail.Entitypostalcode = BankInformation.Entitypostalcode;
                BankAccountDetail.BankNameId = BankInformation.BankNameId;
                BankAccountDetail.Entityregionid = BankInformation.Entityregionid;
                DbEntity.SaveChanges();




                return Json(new { status = 1, BankName = BankInformation.Bankname, url = Url.Action("Index", "BankAccountManager") });
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }







        [HttpPost]
        public JsonResult AddNewBank(string NewBank)

        {
            try
            {

                var BankExists = DbEntity.tblBankAccountLists.Where(u => u.Name == NewBank);

                //Check if category already open
                if (BankExists.Count() > 0)
                {
                    ViewBag.BankList = DbEntity.tblBankAccountLists.Where(a => a.BankAccountId == 0).ToList();
                    return Json(ViewBag.BankList, JsonRequestBehavior.AllowGet);
                }

                // Query for a specific customer.
                tblBankAccountList BA = new tblBankAccountList();
                BA.Name = NewBank;

                // Ask the DataContext to save all the changes.
                DbEntity.tblBankAccountLists.Add(BA);
                DbEntity.SaveChanges();

                ViewBag.BankList = DbEntity.tblBankAccountLists.ToList();


                return Json(ViewBag.BankList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

    }
}