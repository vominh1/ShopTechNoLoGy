using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class ProductsController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        // GET: PrivatePages/Products
        public ActionResult Index()
        {
            HienThiSanPhamchoGiaodien();
            ViewBag.MaLoaiList = GetMaLoaiList();
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string maSanPham)
        {
            sanPham x = db.sanPhams.Find(maSanPham);
            db.sanPhams.Remove(x);
            db.SaveChanges();
            HienThiSanPhamchoGiaodien();
            return View("Index");
        }
        public ActionResult Active(string maSanPham)
        {
            sanPham x = db.sanPhams.Find(maSanPham);
            x.daDuyet = false;

            db.SaveChanges();
            HienThiSanPhamchoGiaodien();
            return View("Index");
        }

        private void HienThiSanPhamchoGiaodien()
        {
            List<sanPham> l = db.sanPhams.Where(x => x.daDuyet == true).ToList<sanPham>();
            ViewData["DanhSachSP"] = l;
            
        }
        [HttpGet]
        private List<SelectListItem> GetMaLoaiList()
        {
            using (var db = new BanBanhOnline()) {
                return db.loaiSPs.Select(l => new SelectListItem {
                    Value = l.maLoai.ToString(),
                    Text = l.loaiSP1
                }).ToList();
            }
        }
        public ActionResult Edit(string maSanPham)
        {
            sanPham sp = db.sanPhams.Find(maSanPham);
            if (sp == null) {
                return HttpNotFound();
            }

            // Tạo ViewBag cho danh sách mã loại
            ViewBag.MaLoaiList = GetMaLoaiList();

            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(sanPham model, HttpPostedFileBase hinhdaidien)
        {
            if (ModelState.IsValid) {
                // Tìm thực thể gốc từ cơ sở dữ liệu
                var originalEntity = db.sanPhams.Find(model.maSP);
                if (originalEntity == null) {
                    return HttpNotFound();
                }

                // Cập nhật các thuộc tính của thực thể gốc với các giá trị mới từ model
                if (hinhdaidien != null && hinhdaidien.ContentLength > 0) {
                    var fileName = System.IO.Path.GetFileName(hinhdaidien.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    hinhdaidien.SaveAs(path);

                    originalEntity.hinhDD = "~/Images/" + fileName;
                }

                // Cập nhật các thuộc tính khác (trừ maSP) từ model
                originalEntity.tenSP = model.tenSP;
                originalEntity.ndTomTat = model.ndTomTat;
                originalEntity.giaBan = model.giaBan;
                originalEntity.giamGia = model.giamGia;
                originalEntity.nhaSanXuat = model.nhaSanXuat;
                originalEntity.dvt = model.dvt;
                originalEntity.SoLuongTonKho = model.SoLuongTonKho;
                originalEntity.ngayDang = model.ngayDang;
                originalEntity.taiKhoan = model.taiKhoan;
                originalEntity.maLoai = model.maLoai;
                originalEntity.noiDung = model.noiDung;
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
              
                // Đánh dấu thực thể gốc là đã sửa đổi
                db.Entry(originalEntity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, tạo lại ViewBag cho danh sách mã loại
            ViewBag.MaLoaiList = new SelectList(db.loaiSPs, "maLoai", "maLoai", model.maLoai);

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Search(string searchString)
        {
            var products = db.sanPhams
                .Where(sp => sp.tenSP.Contains(searchString) || sp.maSP.Contains(searchString))
                .Where(sp => sp.daDuyet == true) // Assuming you only want to search approved products
                .ToList();

            ViewData["DanhSachSP"] = products;
            ViewBag.MaLoaiList = GetMaLoaiList();

            return View("Index");
        }

    }
}