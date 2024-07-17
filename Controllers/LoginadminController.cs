using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class LoginadminController : Controller
    {
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Acc, string Pass, string returnUrl)
        {
            var context = new BanBanhOnline();
            taiKhoanTV ttdn = context.taiKhoanTVs.FirstOrDefault(x => x.taiKhoan.Equals(Acc.ToLower().Trim()) && x.matKhau.Equals(Pass));
            bool isAuthentic = ttdn != null && ttdn.taiKhoan.Equals(Acc.ToLower().Trim()) && ttdn.matKhau.Equals(Pass);
            if (isAuthentic) {
                Session["ttDangNhap"] = ttdn;

                if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl)) {
                    return Redirect(returnUrl);
                }
                else {
                   return RedirectToAction("Index", "Dashbroad", new { area = "PrivatePages" });
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session["ttDangNhap"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}