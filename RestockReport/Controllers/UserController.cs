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
    public class UserController : Controller
    {
        // GET: User
        private RestockReportsEntities dbEntities = new RestockReportsEntities();
        public ActionResult Index()
        {
            ViewBag.Access_Level = dbEntities.tblAccess_Level.ToList();
            ViewBag.user = dbEntities.tblUsers.Where(u=> u.IsDeleted==0).ToList();
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Access_Level = dbEntities.tblAccess_Level.ToList();
            ViewBag.RegionList = new ModelHelper().ToSelectUserRegionList(dbEntities.tblRegions).Distinct().ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "UserID, Password,Email_Adress,IsActive,FirstName,LastName,Title,Company,Address,City,Province,Postalcode,Phonecell,Phoneother,RollAssigned")] tblUser user)
        {
            user.Create_Date = DateTime.Now;
            dbEntities.tblUsers.Add(user);
            dbEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? UserId, string ddlAccessLevel)
        {
            //ViewBag.TabStatus = "";

            //TempData["TabStatus"] = "";



            ViewBag.TerminalList = dbEntities.Sp_Get_User_Terminal_List(UserId).ToList();

            ViewBag.TerminalSegmentedList = dbEntities.Sp_Get_User_Segmented_Terminal_List(UserId).ToList();
            //ViewBag.UserMenuList = dbEntities.Sp_Get_User_Menu_List(User_ID).ToList();


            ViewBag.UserId = UserId;

            ViewBag.Access_Level = dbEntities.tblAccess_Level.ToList();

            //ViewBag.AvailableTerminalList = dbEntities.Sp_Get_User_Terminal_List("0", User_ID).ToList();
            //ViewBag.AissnedTerminalList = dbEntities.Sp_Get_User_Terminal_List("1", User_ID).ToList();


            var Access_Level_Id = (from users in dbEntities.tblUsers
                                   where users.User_ID == UserId
                                   select users.Access_Level_ID).Single();

            ViewBag.UserAccessLevelId = Access_Level_Id;
            if (ddlAccessLevel != null)
            {
                ViewBag.UserMenuList = dbEntities.Sp_Get_User_Menu_List(Convert.ToInt32(ddlAccessLevel)).ToList();
                ViewBag.Access_Level_Id = Convert.ToInt32(ddlAccessLevel);
                TempData["TabStatus"] = "3";
            }
            else
            {
                ViewBag.Access_Level_Id = Access_Level_Id;
                ViewBag.UserMenuList = dbEntities.Sp_Get_User_Menu_List(Convert.ToInt32(Access_Level_Id)).ToList();
            }





            var province = (from users in dbEntities.tblUsers
                            where users.User_ID == UserId
                            select users.Province).Single();
            ViewBag.RegionID = province;


            var User_Login_ID = (from users in dbEntities.tblUsers
                                 where users.User_ID == UserId
                                 select users.UserID).Single();
            ViewBag.User_Login_ID = User_Login_ID;
            ViewBag.RegionList = new ModelHelper().ToSelectUserRegionList(dbEntities.tblRegions).Distinct().ToList();
            tblUser user = dbEntities.tblUsers.Find(UserId);
            return View(user);
        }

        [HttpPost]
        public ActionResult ReloadUserPermissions(int User_ID, string ddlAccessLevel)
        {

            return Json(new { url = Url.Action("Edit", "User", new { UserId = User_ID, ddlAccessLevel = ddlAccessLevel }) });
        }


        [HttpPost]
        public ActionResult Edit([Bind(Include = "User_ID,UserID, Password,Email_Adress,IsActive,FirstName,LastName,Title,Company,Address,City,Province,Postalcode,Phonecell,Phoneother,RollAssigned")] tblUser user)
        {
            user.Create_Date = DateTime.Now;
            dbEntities.Entry(user).State = EntityState.Modified;
            dbEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int User_ID)
        {
            tblUser user = dbEntities.tblUsers.Find(User_ID);
            if (user != null)
            {
                dbEntities.tblUsers.Remove(user);
                dbEntities.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult SaveUser(tblUser UserInformation)
        {
            try
            {

                UserInformation.Create_Date = DateTime.Now;
                dbEntities.tblUsers.Add(UserInformation);
                dbEntities.SaveChanges();

                string Company = UserInformation.Company;
                if (Company != null || Company != "")
                {
                    bool exists = (from users in dbEntities.tblCompanies
                                   where users.Companyname == Company
                                   select users).Any();

                    if (exists == true)
                    {
                        
                    }
                    else if (exists == false)
                    {
                        tblCompany tc = new tblCompany();
                        tc.Companyname = Company;
                        dbEntities.tblCompanies.Add(tc);
                        
                        dbEntities.SaveChanges();
                    }
                }

               



                int NewUserId = dbEntities.tblUsers.Max(u => u.User_ID);

                //Add Report menu to user rights when new user is created 
                //tblUser_Menu_Right UMR = new tblUser_Menu_Right();
                //UMR.User_ID = NewUserId;
                //UMR.Page_ID = 4;
                //dbEntities.tblUser_Menu_Right.Add(UMR);
                //dbEntities.SaveChanges();


                return Json(new { status = 1, userid = UserInformation.UserID, url = Url.Action("Index", "User") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 0, data = ex.Message });
            }

            return Json(new { url = Url.Action("Index", "User") });
        }

        [HttpPost]
        public JsonResult SaveAccessLevel(string txtAccessLevel)

        {
            try
            {

                // Query for a specific customer.
                tblAccess_Level AL = new tblAccess_Level();
                AL.Access_Level_Type = txtAccessLevel;

                // Ask the DataContext to save all the changes.
                dbEntities.tblAccess_Level.Add(AL);
                dbEntities.SaveChanges();

                TempData["TabStatus"] = "3";


                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult UpdateUser(tblUser UserInformation)
        {
            try
            {


                tblUser user = (from c in dbEntities.tblUsers
                                where c.User_ID == UserInformation.User_ID
                                select c).FirstOrDefault();

                user.Password = UserInformation.Password;
                user.Email_Adress = UserInformation.Email_Adress;
                user.IsActive = UserInformation.IsActive;
                user.FirstName = UserInformation.FirstName;
                user.LastName = UserInformation.LastName;
                user.Title = UserInformation.Title;
                user.Company = UserInformation.Company;
                user.Address = UserInformation.Address;
                user.City = UserInformation.City;
                user.Province = UserInformation.Province;
                user.Postalcode = UserInformation.Postalcode;
                user.Phonecell = UserInformation.Phonecell;
                user.Phoneother = UserInformation.Phoneother;
                user.RollAssigned = UserInformation.RollAssigned;
                user.Create_Date = user.Create_Date;
                user.Access_Level_ID = UserInformation.Access_Level_ID;
                dbEntities.SaveChanges();



                string Company = UserInformation.Company;
                if (Company != null || Company != "")
                {
                    bool exists = (from users in dbEntities.tblCompanies
                                   where users.Companyname == Company
                                   select users).Any();

                    if (exists == true)
                    {

                    }
                    else if (exists == false)
                    {
                        tblCompany tc = new tblCompany();
                        tc.Companyname = Company;
                        dbEntities.tblCompanies.Add(tc);

                        dbEntities.SaveChanges();
                    }
                }


                return Json(new { status = 1, userid = UserInformation.FirstName, url = Url.Action("Index", "User") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 0, data = ex.Message });
            }

            return Json(new { url = Url.Action("Index", "User") });
        }




        [HttpGet]
        public JsonResult VerifyUserId(string UserId)

        {
            try
            {

                bool exists = (from users in dbEntities.tblUsers
                               where users.UserID == UserId
                               select users).Any();

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
        public ActionResult UpdateUserTerminalRights(int UserId, int status, string TerminalKey, int ActionType)
        {
            try
            {

                if (ActionType == 0)
                {
                    tblUserterminal TU = new tblUserterminal();
                    TU.TerminalKey = TerminalKey;
                    TU.UserID = UserId;

                    if (status == 1)
                    {
                        //status 1 means single terminal rights given
                        dbEntities.tblUserterminals.Add(TU);
                        dbEntities.SaveChanges();

                    }
                    else if (status == 0)
                    {
                        //status 0 means single terminal rights removed
                        var UserTerminal = dbEntities.tblUserterminals.Where(a => a.UserID == UserId && a.TerminalKey == TerminalKey);
                        dbEntities.tblUserterminals.RemoveRange(UserTerminal);
                        dbEntities.SaveChanges();
                    }

                    TempData["ActionStatus"] = status;

                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else if (ActionType == 2)
                {
                    //status 3 means all rights removed 
                    status = 3;
                    var RemoveAllUserTerminal = dbEntities.tblUserterminals.Where(a => a.UserID == UserId);
                    dbEntities.tblUserterminals.RemoveRange(RemoveAllUserTerminal);
                    dbEntities.SaveChanges();
                    TempData["TabStatus"] = "1";
                    TempData["ActionStatus"] = status;
                    return Json(status, JsonRequestBehavior.AllowGet);

                    //ViewBag.TabStatus = "1";



                }
                else if (ActionType == 1)
                {
                    //status 4  means all rights Assigned
                    status = 4;

                    var UserTerminal = dbEntities.tblUserterminals.Where(a => a.UserID == UserId);
                    dbEntities.tblUserterminals.RemoveRange(UserTerminal);
                    dbEntities.SaveChanges();


                    var GetAllTerminals = dbEntities.tblTerminals.Where(t => t.IsActive == 1).ToList();

                    tblUserterminal TU = new tblUserterminal();

                    foreach (var item in GetAllTerminals)
                    {
                        TU.TerminalKey = item.TerminalKey;
                        TU.UserID = UserId;
                        dbEntities.tblUserterminals.Add(TU);
                        dbEntities.SaveChanges();
                    }
                    TempData["TabStatus"] = "1";

                    TempData["ActionStatus"] = status;
                    return Json(status, JsonRequestBehavior.AllowGet);
                    //ViewBag.TabStatus = "1";

                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult UpdateSegmentedUserTerminalRights(int UserId, int status, string TerminalKey, int ActionType)
        {
            try
            {

                if (ActionType == 0)
                {
                    tblterminalsegmented TU = new tblterminalsegmented();
                    TU.TerminalKey = TerminalKey;
                    TU.UserID = UserId;

                    if (status == 1)
                    {
                        //status 1 means single terminal rights given
                        dbEntities.tblterminalsegmenteds.Add(TU);
                        dbEntities.SaveChanges();

                    }
                    else if (status == 0)
                    {
                        //status 0 means single terminal rights removed
                        var UserTerminal = dbEntities.tblterminalsegmenteds.Where(a => a.UserID == UserId && a.TerminalKey == TerminalKey);
                        dbEntities.tblterminalsegmenteds.RemoveRange(UserTerminal);
                        dbEntities.SaveChanges();
                    }

                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else if (ActionType == 2)
                {
                    //status 3 means all rights removed 
                    status = 3;
                    var RemoveAllUserTerminal = dbEntities.tblterminalsegmenteds.Where(a => a.UserID == UserId);
                    dbEntities.tblterminalsegmenteds.RemoveRange(RemoveAllUserTerminal);
                    dbEntities.SaveChanges();
                    TempData["TabStatus"] = "2";
                    TempData["ActionStatus"] = 5;
                    return Json(status, JsonRequestBehavior.AllowGet);

                    //ViewBag.TabStatus = "1";



                }
                else if (ActionType == 1)
                {
                    //status 4  means all rights Assigned
                    status = 4;


                    var UserSegmentedTerminal = dbEntities.tblterminalsegmenteds.Where(a => a.UserID == UserId);
                    dbEntities.tblterminalsegmenteds.RemoveRange(UserSegmentedTerminal);
                    dbEntities.SaveChanges();


                    var GetAllTerminals = dbEntities.tblUserterminals.Where(u => u.UserID == UserId).ToList();

                    tblterminalsegmented TU = new tblterminalsegmented();

                    foreach (var item in GetAllTerminals)
                    {
                        TU.TerminalKey = item.TerminalKey;
                        TU.UserID = UserId;
                        dbEntities.tblterminalsegmenteds.Add(TU);
                        dbEntities.SaveChanges();
                    }
                    TempData["TabStatus"] = "2";
                    TempData["ActionStatus"] = 6;

                    return Json(status, JsonRequestBehavior.AllowGet);
                    //ViewBag.TabStatus = "1";

                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public ActionResult UpdateMenuRights(int AccessId, int status, int Page_ID, int ParentId, int Counts)
        {
            try
            {
                tblAccess_Level_Menu ALM = new tblAccess_Level_Menu();

                ALM.Access_ID = AccessId;
                ALM.Page_ID = Page_ID;




                if (status == 1)
                {
                    dbEntities.tblAccess_Level_Menu.Add(ALM);
                    dbEntities.SaveChanges();


                    //Automatically  add parent menu in rights
                    if (ParentId > 0)
                    {
                        int count = dbEntities.tblAccess_Level_Menu.Count((a => a.Access_ID == AccessId && a.Page_ID == ParentId));

                        if (count == 0)
                        {
                            ALM.Access_ID = AccessId;
                            ALM.Page_ID = ParentId;
                            dbEntities.tblAccess_Level_Menu.Add(ALM);
                            dbEntities.SaveChanges();

                        }
                    }

                }
                else if (status == 0)
                {
                    var AccessLevelMenu = dbEntities.tblAccess_Level_Menu.Where(a => a.Access_ID == AccessId && a.Page_ID == Page_ID);
                    dbEntities.tblAccess_Level_Menu.RemoveRange(AccessLevelMenu);
                    dbEntities.SaveChanges();


                    if (Counts > 0)
                    {


                        int[] ChildMenuPageIds = dbEntities.tblMenus.Where(a => a.ParentId == Page_ID).Select(x => x.Page_ID).ToArray();

                        foreach (var ids in ChildMenuPageIds)
                        {
                            AccessLevelMenu = dbEntities.tblAccess_Level_Menu.Where(a => a.Access_ID == AccessId && a.Page_ID == ids);


                            dbEntities.tblAccess_Level_Menu.RemoveRange(AccessLevelMenu);
                            dbEntities.SaveChanges();
                        }



                    }


                }




                //tblUser_Menu_Right UMR = new tblUser_Menu_Right();

                //UMR.User_ID = UserId;
                //UMR.Page_ID = Page_ID;


                //if (status == 1)
                //{
                //    dbEntities.tblUser_Menu_Right.Add(UMR);
                //    dbEntities.SaveChanges();

                //}
                //else if (status == 0)
                //{
                //    var UserMenuRights = dbEntities.tblUser_Menu_Right.Where(a => a.User_ID == UserId && a.Page_ID == Page_ID);
                //    dbEntities.tblUser_Menu_Right.RemoveRange(UserMenuRights);
                //    dbEntities.SaveChanges();
                //}



                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult UpdateUserStatus(int? User_ID)
        {
            try
            {


                tblUser user = (from c in dbEntities.tblUsers
                                where c.User_ID == User_ID
                                select c).FirstOrDefault();

                user.IsDeleted = 1;
                dbEntities.SaveChanges();



                return Json(new { status = 1, url = Url.Action("Index", "User") });

            }
            catch (Exception ex)
            {
                return Json(new { status = 1,  url = Url.Action("Index", "User") });
            }

            return Json(new { status = 1, url = Url.Action("Index", "User") });
        }



        [HttpPost]
        public ActionResult UpdateUserTerminalStartDate(int? UserId, string TerminalKey,DateTime StartDate)
        {
            try
            {

                tblUserterminal userterminal = (from c in dbEntities.tblUserterminals
                                                where c.UserID == UserId && c.TerminalKey == TerminalKey
                                                select c).FirstOrDefault();

                userterminal.StartDate = StartDate;
                dbEntities.SaveChanges();



                return Json(1);

            }
            catch (Exception ex)
            {
                return Json(0);
            }

            return Json(1);
        }

    }
}