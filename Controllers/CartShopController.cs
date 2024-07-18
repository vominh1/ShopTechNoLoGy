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
        private BanBanhOnline db = new BanBanhOnline();
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
            // Truy xuất thông tin sản phẩm từ cơ sở dữ liệu
            var product = db.sanPhams.SingleOrDefault(p => p.maSP == maSP);

            // Kiểm tra xem sản phẩm có tồn tại hay không
            if (product != null) {
                // Kiểm tra số lượng tồn kho
                if (product.SoLuongTonKho == 1) {
                    ViewBag.ProductLowStockMessage = "Sản phẩm này chỉ còn 1!";
                }
                else if (product.SoLuongTonKho > 1) {
                    // Lấy giỏ hàng từ session
                    CartShop1 gh = Session["GioHang"] as CartShop1;

                    // Thêm sản phẩm vào giỏ hàng
                    gh.addItem(maSP);

                    // Cập nhật giỏ hàng trong session
                    Session["GioHang"] = gh;
                }
            }
            else {
                // Thông báo hoặc xử lý khi không tìm thấy sản phẩm
                ViewBag.Message = "Không tìm thấy sản phẩm!";
            }

            // Load lại trang Index để hiển thị thông tin mới
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