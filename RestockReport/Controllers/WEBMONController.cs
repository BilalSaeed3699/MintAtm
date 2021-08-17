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
    public class WEBMONController : Controller
    {

        private RestockReportsEntities dbEntities = new RestockReportsEntities();
        // GET: WEBMON
        public ActionResult Index()
        {
            var WebmoonUser = dbEntities.tblWebmoonUsers.ToList().FirstOrDefault();

            ViewBag.WebMoonId = WebmoonUser.WebmoonId;
            ViewBag.Password = WebmoonUser.Password;

            return View();
        }

        [HttpPost]
        public ActionResult Changepassword(string WebmoonId, string Password)
        {
           
          // Query for a specific user.
            var Webmoonuser =
                (from c in dbEntities.tblWebmoonUsers
                 //where c.WebmoonId == WebmoonId
                 select c).FirstOrDefault();

            // Change the name of the contact.
            Webmoonuser.WebmoonId = WebmoonId;
            Webmoonuser.Password = Password;

            // Ask the DataContext to save all the changes.
            dbEntities.SaveChanges();


            ViewBag.IsSuccess = "Password change successfully";
            return RedirectToAction("Index", "WEBMON");
        }
    }
}