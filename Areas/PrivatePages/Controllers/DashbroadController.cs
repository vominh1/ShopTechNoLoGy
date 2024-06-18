using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Areas.PrivatePages.Models;
using ShopTechNoLoGy.Models;
namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class DashbroadController : Controller
    {
        // GET: PrivatePages/Dashbroad
     
        public ActionResult Index()
        {   
           
            return View();
        }
    }
}