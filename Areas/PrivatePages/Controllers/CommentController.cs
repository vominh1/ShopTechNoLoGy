using System;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class CommentController : Controller
    {
        private BanBanhOnline db = new BanBanhOnline();

        // GET: PrivatePages/Comment/Index
        public ActionResult Index()
        {
            // Chỉ lấy các bình luận chưa có phản hồi
            var binhLuans = db.binhLuanSPs.Include("sanPham").Include("taiKhoanTV")
                                          .Where(b => string.IsNullOrEmpty(b.PhanHoi)) // Lọc bình luận chưa có phản hồi
                                          .ToList();
            return View(binhLuans);
        }

        // GET: PrivatePages/Comment/Responded
        public ActionResult BinhLuanDaPhanHoi()
        {
            // Lấy tất cả bình luận đã có phản hồi từ cơ sở dữ liệu
            var binhLuans = db.binhLuanSPs.Include("sanPham")
                                          .Include("taiKhoanTV")
                                          .Where(b => !string.IsNullOrEmpty(b.PhanHoi)) // Lọc các bình luận đã có phản hồi
                                          .ToList();
            return View(binhLuans);
        }
        public ActionResult PhanHoiLai(int id)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }
            return View(binhLuan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhanHoiLai(int id, string phanHoi)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }

            // Cập nhật phản hồi và ngày phản hồi mới
            binhLuan.PhanHoi = phanHoi;
            binhLuan.NgayPhanHoi = DateTime.Now;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Chuyển hướng về danh sách bình luận đã phản hồi
            return RedirectToAction("BinhLuanDaPhanHoi");
        }

        // GET: PrivatePages/Comment/Reply/5
        public ActionResult Reply(int id)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }
            return View(binhLuan);
        }

        // POST: PrivatePages/Comment/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int id, string phanHoi)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }

            // Cập nhật phản hồi và ngày phản hồi
            binhLuan.PhanHoi = phanHoi;
            binhLuan.NgayPhanHoi = DateTime.Now;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Chuyển hướng về danh sách bình luận
            return RedirectToAction("Index");
        }

        // Cánh báo người dùng khi người dùng comment có hiện dấu hiệu bôi nhọ trang
        public ActionResult DanhDauKhongPhuHop(int id)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }

            // Đánh dấu bình luận là bị cảnh báo
            binhLuan.BiCanhBao = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Chuyển hướng về danh sách bình luận
            return RedirectToAction("Index");
        }
        /// <summary>
        ///  Hàm danh sách bình luận bị cảnh báo
        /// </summary>
      
        public ActionResult BinhLuanBiCanhBao()
        {
            // Lấy các bình luận bị cảnh báo
            var binhLuans = db.binhLuanSPs.Include("sanPham")
                                          .Include("taiKhoanTV")
                                          .Where(b => b.BiCanhBao == true)
                                          .ToList();
            return View(binhLuans);
        }
        // GET: PrivatePages/Comment/RemoveWarning/5
        public ActionResult GoBinhLuanBiCanhCao(int id)
        {
            var binhLuan = db.binhLuanSPs.FirstOrDefault(b => b.MaBL == id);
            if (binhLuan == null) {
                return HttpNotFound();
            }

            // Đặt BiCanhBao thành false (gỡ cảnh báo)
            binhLuan.BiCanhBao = false;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Chuyển hướng về danh sách bình luận bị cảnh báo
            return RedirectToAction("BinhLuanBiCanhBao");
        }



    }
}
