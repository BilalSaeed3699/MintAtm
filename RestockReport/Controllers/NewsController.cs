using RestockReport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestockReport.Controllers
{
    public class NewsController : Controller
    {
        private RestockReportsEntities DbEntity = new RestockReportsEntities();

        [FilterConfig.AuthorizeActionFilter]
        [FilterConfig.NoDirectAccess]
        // GET: News
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Newsandnotices()
        {
            ViewBag.NewsImage = DbEntity.tblNewletters.ToList();
            return View();
        }

        [HttpPost]

        //Upload Files 
        public ActionResult UploadImages()
        {



            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {

                    {

                        tblNewletter newsimage = new tblNewletter();
                        // Get all files from Request object
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string fname;
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                                fname = file.FileName;
                            }
                            else
                            {
                                fname = file.FileName;

                            }

                            //latestSubcategoryRecord = dbEntities.tblTemporaryImages.OrderByDescending(a => a.ImageId).First();

                            // Get the complete folder path and store the file inside it.
                            var FolderPath = Server.MapPath("/assets/img/");

                            if (!Directory.Exists(FolderPath))
                            {
                                // Try to create the directory.
                                DirectoryInfo di = Directory.CreateDirectory(FolderPath);
                            }
                            FolderPath = "/assets/img/" + fname + "";

                            //qry = "Exec Sp_InsertUpdate_FilePath_Temp '" + LoaderNumbers + "' , '" + FolderPath + "' ,  '" + fname + "' ";
                            //string result = ut.InsertUpdate(qry);

                          
                            newsimage.Filename = fname;
                            newsimage.FilePath = FolderPath;

                            DbEntity.Database.ExecuteSqlCommand("TRUNCATE TABLE [tblNewletter]");
                            DbEntity.tblNewletters.Add(newsimage);
                            DbEntity.SaveChanges();

                            FolderPath = "/assets/img/" ;

                            fname = Path.Combine(Server.MapPath(FolderPath), fname);
                            file.SaveAs(fname);

                        }
                        
                    }

                    return Json("", JsonRequestBehavior.AllowGet);


                }
                catch (Exception ex)
                {
                    return Json("Error occurred.Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("1");
            }
        }

    }
}