using System;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Rotativa;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class CheckOutSuccessController : Controller
    {
        public ActionResult Index(string soDH)
        {
            try {
                CartShop1 cart = Session["GioHang"] as CartShop1;
                if (cart == null) {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["Cart"] = cart;
                Console.WriteLine("Cart Retrieved: " + cart.MaKH + ", " + cart.TaiKhoan);

                Session["GioHang"] = new CartShop1();
                ViewData["SoDH"] = soDH;
                return View();
            }
            catch (Exception ex) {
                Console.WriteLine("Exception: " + ex.Message);
                return View("Error");
            }
        }
        

    }
}
