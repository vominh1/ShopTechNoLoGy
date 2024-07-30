using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class LogoutAdminController : Controller
    {
        // GET: PrivatePages/LogoutAdmin
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogoutAdmin()
        {
            Session["ttDangNhap"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Dashbroad");
        }
    }
}