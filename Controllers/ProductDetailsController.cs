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
        // GET: ProductDetails
        public ActionResult Index(string maSP)
        {
            BanBanhOnline db = new BanBanhOnline();
            sanPham x = db.sanPhams.Include("binhLuanSPs").FirstOrDefault(z => z.maSP == maSP);

            if (x == null) {
                return HttpNotFound(); // Kiểm tra nếu sản phẩm không tồn tại
            }

            ViewData["spcanxem"] = x;
             ViewData["username"] = User.Identity.Name;
            return View();
        }

        // Action để thêm bình luận
        [HttpPost]
        public ActionResult ThemBinhLuan(string maSP, string noiDung)
        {
            // khơi tạo đối tượng tài khoản thành viên bằng  thành sesstion đã lưu trong đăng nhập
            taiKhoanTV tk = Session["ttDangNhap"] as taiKhoanTV;

            if (tk == null) {
                // Trả về trang đăng nhập hoặc thông báo lỗi nếu người dùng chưa đăng nhập
                return RedirectToAction("Index", "Login");
            }

            using (var db = new BanBanhOnline()) {
                var binhLuan = new binhLuanSP {
                    MaSP = maSP,
                    TaiKhoan = tk.taiKhoan, // Sử dụng tên tài khoản từ session
                    NoiDung = noiDung,
                    NgayBL = DateTime.Now
                };
                db.binhLuanSPs.Add(binhLuan);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "ProductDetails", new { maSP = maSP });
        }

    }
}
