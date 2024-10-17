using System;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class ArticleContentController : Controller
    {
        // GET: ArticleContent/Index/{maBV}
        public ActionResult Index(string maBV)
        {
            using (BanBanhOnline db = new BanBanhOnline()) {
                // Eagerly load baiViet along with its related binhLuanBVs
                baiViet x = db.baiViets
                              .Include("binhLuanBVs")
                              .FirstOrDefault(z => z.maBV == maBV);

                if (x == null) {
                    return HttpNotFound();
                }

                // Tăng số lần đọc và lưu vào cơ sở dữ liệu
                x.solandoc++;
                db.SaveChanges();

                // Pass dữ liệu bài viết tới view
                ViewData["Baicanxem"] = x;
            }

            return View();
        }


        //bình luận bài viết
        [HttpPost]
        public ActionResult ThemBinhLuanBV(string maBV, string noiDung)
        {
            // khơi tạo đối tượng tài khoản thành viên bằng  thành sesstion đã lưu trong đăng nhập
            taiKhoanTV tk = Session["ttDangNhap"] as taiKhoanTV;

            if (tk == null) {
                // Trả về trang đăng nhập hoặc thông báo lỗi nếu người dùng chưa đăng nhập
                return RedirectToAction("Index", "Login");
            }

            using (var db = new BanBanhOnline()) {
                var binhLuanBV = new binhLuanBV {
                    MaBV = maBV,
                    TaiKhoan = tk.taiKhoan, // Sử dụng tên tài khoản từ session
                    NoiDung = noiDung,
                    NgayBL = DateTime.Now
                };
                db.binhLuanBVs.Add(binhLuanBV);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "ArticleContent", new { maBV = maBV });
        }

    }

}
