using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class ArticlesController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        // GET: PrivatePages/Articles
        public ActionResult Index()
        {
            CapNhatDuLieuChoGiaoDien();
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string maBaiViet)
        {
            baiViet x = db.baiViets.Find(maBaiViet);
            db.baiViets.Remove(x);
            db.SaveChanges();
            CapNhatDuLieuChoGiaoDien();
            return View("Index");
        }
        public ActionResult Active(string maBaiViet)
        {
            baiViet x = db.baiViets.Find(maBaiViet);
            x.daDuyet = false;

            db.SaveChanges();
            CapNhatDuLieuChoGiaoDien();
            return View("Index");
        }
        private void CapNhatDuLieuChoGiaoDien()
        {
            List<baiViet> l = db.baiViets.Where(x => x.daDuyet == true).ToList<baiViet>();
            ViewData["DanhSachBV"] = l;
        }
        public ActionResult editArticle(string mabaiviet)
        {
            baiViet bv = db.baiViets.Find(mabaiviet);
            if (bv == null) {
                return HttpNotFound();
            }

            // Tạo ViewBag cho danh sách mã loại


            return View(bv);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editArticle(baiViet model, HttpPostedFileBase hinhdaidien)
        {
            if (ModelState.IsValid) {
                // Tìm thực thể gốc từ cơ sở dữ liệu
                var originalEntity = db.baiViets.Find(model.maBV);
                if (originalEntity == null) {
                    return HttpNotFound();
                }
                if (hinhdaidien != null && hinhdaidien.ContentLength > 0) {
                    var fileName = System.IO.Path.GetFileName(hinhdaidien.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    hinhdaidien.SaveAs(path);

                    originalEntity.hinhDD = "~/Images/" + fileName;
                }
                originalEntity.tenBV = model.tenBV;
                originalEntity.ndTomTat = model.ndTomTat;
                originalEntity.ngayDang = model.ngayDang;
                originalEntity.taiKhoan = model.taiKhoan;
              
                originalEntity.noiDung = model.noiDung;
                TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
                db.Entry(originalEntity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Articles");
            }
            return View(model);
        }

    }
}