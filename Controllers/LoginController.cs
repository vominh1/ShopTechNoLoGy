using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
namespace ShopTechNoLoGy.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Acc, string Pass)
        {
            // Không cần mã hóa mật khẩu ở đây
            //string mk = MaHoa.encryptSHA256(Pass);
            taiKhoanTV ttdn = new BanBanhOnline().taiKhoanTVs.FirstOrDefault(x => x.taiKhoan.Equals(Acc.ToLower().Trim()) && x.matKhau.Equals(Pass));
            bool isAuthentic = ttdn != null && ttdn.taiKhoan.Equals(Acc.ToLower().Trim()) && ttdn.matKhau.Equals(Pass);
            if (isAuthentic) {
                Session["ttDangNhap"] = ttdn;
                return RedirectToAction("Index", "Dashbroad", new { Area = "PrivatePages" });
            }

            return View();
        }


    }
}