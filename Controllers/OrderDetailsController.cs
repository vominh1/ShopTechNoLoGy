using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class OrderDetailsController : Controller
    {
        private BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        public ActionResult Index()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (Session["ttDangNhap"] == null) {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("Index", "OrderDetails") });
            }

            HienThiDonHangdadat();
            
            // Trả về view hiển thị danh sách đơn hàng
            return View();
        }
        [HttpGet]
        public ActionResult ChuaXuLy()
        {
            if (Session["ttDangNhap"] == null) {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("ChuaXuLy", "OrderDetails") });
            }
            HienThiDonHangChuaXuLy();
            return View();
        }
        [HttpPost]
        public ActionResult ChiTietDonHang(string soDH)
        {
            var chiTiet = db.ctDonHangs.Where(x => x.soDH == soDH).ToList();
            ViewData["ChiTietDonHang"] = chiTiet;
            return View("ChiTietDonHang");
        }
        [HttpPost]
        public ActionResult ChiTietDonHangChuaXuLy(string soDH)
        {
            var chiTiet = db.ctDonHangs.Where(x => x.soDH == soDH).ToList();
            ViewData["ChiTietDonHangChuaXuLy"] = chiTiet;
            return View("ChiTietDonHangChuaXuLy");
        }

        private ActionResult HienThiDonHangdadat()
        {
            // Lấy thông tin tài khoản từ session hoặc cơ chế xác thực
            taiKhoanTV currentUser = Session["ttDangNhap"] as taiKhoanTV;

            if (currentUser != null) {
                // Lọc đơn hàng theo tài khoản đã đăng nhập
                List<donHang> l = db.donHangs
                    .Where(x => x.daKichHoat == true && x.taiKhoan == currentUser.taiKhoan)
                    .ToList();

                ViewData["DanhSachDonHangdadat"] = l;
            }
            else {
                // Xử lý trường hợp không có tài khoản trong session
                ViewData["DanhSachDonHangdadat"] = new List<donHang>();
            }

            return View();
        }
        private void HienThiDonHangChuaXuLy()
        {
            taiKhoanTV currentUser = Session["ttDangNhap"] as taiKhoanTV;
            
            if (currentUser != null) {
                List<donHang> l = db.donHangs
                    .Where(x => x.daKichHoat == false && x.taiKhoan == currentUser.taiKhoan)
                    .ToList();
                ViewData["DanhSachDonHangChuaXuLy"] = l;
            }
            else {
                ViewData["DanhSachDonHangChuaXuLy"] = new List<donHang>();
            }
        }
    }
}