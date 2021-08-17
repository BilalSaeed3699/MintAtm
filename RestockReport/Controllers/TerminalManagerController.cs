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

    public class TerminalManagerController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();
        // GET: TerminalManager
        public ActionResult Index()
        {
            try
            {
                ViewBag.TermManagerList = DbEntity.Sp_Term_Manager_List().ToList();
                return View();
            }
            catch (Exception ex)
            {

            }

            
            return View();

        }



        public ActionResult Termsdetails(string TerminalId, string LocationName)
        {
            try
            {
               ViewBag.TerminalId = TerminalId;
                ViewBag.LocationName = LocationName;
                ViewBag.TermManagerList = DbEntity.Sp_Term_Manager_List().ToList();
                ViewBag.BankNameList = DbEntity.Sp_Get_BankNickName_List().ToList();
                ViewBag.TermsDetailBankNameList = DbEntity.Sp_Get_TermsDetails_BankNickName_List("SR").ToList();
                ViewBag.TermsDetailSegmentedBankNameList = DbEntity.Sp_Get_TermsDetails_BankNickName_List("SG").ToList();
                return View();
            }
            catch (Exception ex)
            {

            }


            return View();

        }



        public ActionResult EditTermsdetails(string TerminalId, string LocationName,int? TermsDetailId=0)
        {
            try
            {
                ViewBag.TermsDetails = DbEntity.tblTermsDetails.Where(t => t.TermsDetailId == TermsDetailId).ToList();

                ViewBag.TerminalId = TerminalId;
                ViewBag.LocationName = LocationName;
                ViewBag.TermsDetailId = TermsDetailId;
                ViewBag.TermManagerList = DbEntity.Sp_Term_Manager_List().ToList();
                ViewBag.BankNameList = DbEntity.Sp_Get_BankNickName_List().ToList();
                ViewBag.TermsDetailBankNameList = DbEntity.Sp_Get_TermsDetails_BankNickName_List("SR").ToList();
                ViewBag.TermsDetailSegmentedBankNameList = DbEntity.Sp_Get_TermsDetails_BankNickName_List("SG").ToList();
                return View();
            }
            catch (Exception ex)
            {

            }


            return View();

        }


        [HttpPost]
        public ActionResult SaveTermsDetails(tblTermsDetail TermsDetailInfo)
        {
            try
            {
                if(TermsDetailInfo.InterchangeSplitRemainder1==null)
                {
                    TermsDetailInfo.InterchangeSplitRemainder1 = "";
                }
                TermsDetailInfo.CreatedDate = DateTime.Now;
                TermsDetailInfo.CreateBy = Session["UserID"].ToString();
                DbEntity.tblTermsDetails.Add(TermsDetailInfo);
                DbEntity.SaveChanges();



                return Json(new { status = 1, terminal = TermsDetailInfo.TerminalKey, url = Url.Action("Index", "TerminalManager") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 0, data = ex.Message });
            }

            return Json(new { url = Url.Action("Index", "TerminalManager") });
        }




        [HttpPost]
        public JsonResult UpdateTermsDetails(tblTermsDetail TermsDetailInfo)

        {
            try
            {


                // Query for a specific customer.
                var TermDetail =
                    (from c in DbEntity.tblTermsDetails
                     where c.TermsDetailId == TermsDetailInfo.TermsDetailId
                     select c).First();

                if (TermsDetailInfo.InterchangeSplitRemainder1 == null)
                {
                    TermsDetailInfo.InterchangeSplitRemainder1 = "";
                }

                TermDetail.TerminalKey = TermsDetailInfo.TerminalKey;
                TermDetail.VaultCashID = TermsDetailInfo.VaultCashID;
                TermDetail.SurchargeAccountCount = TermsDetailInfo.SurchargeAccountCount;
                TermDetail.InterchangeAccountCount = TermsDetailInfo.InterchangeAccountCount;
                TermDetail.Totalterminalsurchageamount = TermsDetailInfo.Totalterminalsurchageamount;
                TermDetail.SurchargesplitId1 = TermsDetailInfo.SurchargesplitId1;
                TermDetail.Surchargesplitamount1 = TermsDetailInfo.Surchargesplitamount1;
                TermDetail.Surchargesettlementsource1 = TermsDetailInfo.Surchargesettlementsource1;
                TermDetail.IsSurchargesegmented1 = TermsDetailInfo.IsSurchargesegmented1;
                TermDetail.SurchargesegmentedID1 = TermsDetailInfo.SurchargesegmentedID1;
                TermDetail.SurchargesplitId2 = TermsDetailInfo.SurchargesplitId2;
                TermDetail.Surchargesplitamount2 = TermsDetailInfo.Surchargesplitamount2;
                TermDetail.Surchargesettlementsource2 = TermsDetailInfo.Surchargesettlementsource2;
                TermDetail.IsSurchargesegmented2 = TermsDetailInfo.IsSurchargesegmented2;
                TermDetail.SurchargesegmentedID2 = TermsDetailInfo.SurchargesegmentedID2;
                TermDetail.SurchargesplitId3 = TermsDetailInfo.SurchargesplitId3;
                TermDetail.Surchargesplitamount3 = TermsDetailInfo.Surchargesplitamount3;
                TermDetail.Surchargesettlementsource3 = TermsDetailInfo.Surchargesettlementsource3;
                TermDetail.IsSurchargesegmented3 = TermsDetailInfo.IsSurchargesegmented3;
                TermDetail.SurchargesegmentedID3 = TermsDetailInfo.SurchargesegmentedID3;
                TermDetail.SurchargesplitId4 = TermsDetailInfo.SurchargesplitId4;
                TermDetail.Surchargesplitamount4 = TermsDetailInfo.Surchargesplitamount4;
                TermDetail.Surchargesettlementsource4 = TermsDetailInfo.Surchargesettlementsource3;
                TermDetail.IsSurchargesegmented4 = TermsDetailInfo.IsSurchargesegmented4;
                TermDetail.SurchargesegmentedID4 = TermsDetailInfo.SurchargesegmentedID4;
                TermDetail.SurchargesplitId5 = TermsDetailInfo.SurchargesplitId5;
                TermDetail.Surchargesplitamount5 = TermsDetailInfo.Surchargesplitamount5;
                TermDetail.Surchargesettlementsource5 = TermsDetailInfo.Surchargesettlementsource5;
                TermDetail.IsSurchargesegmented5 = TermsDetailInfo.IsSurchargesegmented5;
                TermDetail.SurchargesegmentedID5 = TermsDetailInfo.SurchargesegmentedID5;
                TermDetail.Totalsurchargeassigned = TermsDetailInfo.Totalsurchargeassigned;
                TermDetail.TotalInterchangeAmount = TermsDetailInfo.TotalInterchangeAmount;
                TermDetail.InterchangeSplitRemainder1 = TermsDetailInfo.InterchangeSplitRemainder1;
                TermDetail.InterchangeSplit1 = TermsDetailInfo.InterchangeSplit1;
                TermDetail.InterchangeSplitAmount1 = TermsDetailInfo.InterchangeSplitAmount1;
                TermDetail.InterchangeSplit2 = TermsDetailInfo.InterchangeSplit2;
                TermDetail.InterchangeSplitAmount2 = TermsDetailInfo.InterchangeSplitAmount2;
                TermDetail.Interchangesettlementsource1 = TermsDetailInfo.Interchangesettlementsource1;
                TermDetail.InterchangeSplit3 = TermsDetailInfo.InterchangeSplit3;
                TermDetail.InterchangeSplitAmount3 = TermsDetailInfo.InterchangeSplitAmount3;
                TermDetail.Interchangesettlementsource2 = TermsDetailInfo.Interchangesettlementsource2;
                TermDetail.InterchangeSplit4 = TermsDetailInfo.InterchangeSplit4;
                TermDetail.InterchangeSplitAmount4 = TermsDetailInfo.InterchangeSplitAmount4;
                TermDetail.Interchangesettlementsource3 = TermsDetailInfo.Interchangesettlementsource3;
                TermDetail.InterchangeSplit5 = TermsDetailInfo.InterchangeSplit5;
                TermDetail.InterchangeSplitAmount5 = TermsDetailInfo.InterchangeSplitAmount5;
                TermDetail.Interchangesettlementsource4 = TermsDetailInfo.Interchangesettlementsource4;
                TermDetail.TotalInterchangeassigned = TermsDetailInfo.TotalInterchangeassigned;
                DbEntity.SaveChanges();

                return Json(new { status = 1, terminal = TermsDetailInfo.TerminalKey, url = Url.Action("Index", "TerminalManager") });
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
    }
}