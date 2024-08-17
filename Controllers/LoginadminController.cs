using System;
using System.Linq;
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
            // Tìm kiếm tài khoản với tài khoản và mật khẩu đã cung cấp
            taiKhoanTV ttdn = context.taiKhoanTVs.FirstOrDefault(x => x.taiKhoan.Equals(Acc.ToLower().Trim()) && x.matKhau.Equals(Pass));

            // Kiểm tra tính xác thực và quyền admin
            bool isAuthentic = ttdn != null && ttdn.taiKhoan.Equals(Acc.ToLower().Trim()) && ttdn.matKhau.Equals(Pass) && ttdn.quyenadmin == true;

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
            // Thêm thông báo lỗi nếu tài khoản không có quyền admin
            ModelState.AddModelError("", ttdn == null ? "Tài khoản hoặc mật khẩu không đúng." : "Tài khoản của bạn không được phép đăng nhập vào trang quản trị.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogoutAdmin()
        {
            Session["ttDangNhap"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}
