using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
namespace ShopTechNoLoGy.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            string testMK = MaHoa.encryptSHA256("dayabc");
            List<sanPham> l = Common.getProductByLoaiSP(4);
            return View();
        }
        public ActionResult AddToCart(string maSP)
        {
            //lấy giỏ hàng từ sesion ra
            CartShop1 gh = Session["GioHang"] as CartShop1;
            gh.addItem(maSP);
            Session["GioHang"] = gh;
            //--- Cập nhật giỏ hàng vòa trong session
            return View("Index");
        }
    }
}