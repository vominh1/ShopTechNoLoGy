using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class CartShopController : Controller
    {
        // GET: CartShop
        [HttpGet]
        public ActionResult Index()
        {
            CartShop1 gh = Session["GioHang"] as CartShop1;
            //-- truyền ra cho view 
            ViewData["Cart"] = gh;
            return View();
        }             
        public ActionResult increase(string maSP)
        {
            CartShop1 gh = Session["GioHang"] as CartShop1;
            gh.addItem(maSP);
            Session["GioHang"] = gh;

            return RedirectToAction("Index");
        }
        public ActionResult decrease(string maSP)
        {

            CartShop1 gh = Session["GioHang"] as CartShop1;
            gh.decrease(maSP);
            Session["GioHang"] = gh;
            return RedirectToAction("Index");
        }
        public ActionResult RemoveItem(string maSP)
        {
            CartShop1 gh = Session["GioHang"] as CartShop1;
            gh.deleteItem(maSP);
            Session["GioHang"] = gh;
            return RedirectToAction("Index");
        }
    }
}