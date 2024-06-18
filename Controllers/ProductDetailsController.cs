using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
namespace ShopTechNoLoGy.Controllers
{
    public class ProductDetailsController : Controller
    {
        // GET: Produc
        public ActionResult Index(string maSP)
        {
            BanBanhOnline db = new BanBanhOnline();
            sanPham x = db.sanPhams.Where(z => z.maSP == maSP).First<sanPham>();
            ViewData["spcanxem"] = x;
            return View();

        }
    }
}