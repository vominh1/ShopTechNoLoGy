using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using ShopTechNoLoGy.Areas.PrivatePages.Models;
using System.IO;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class NewProductController : Controller
    {
        private BanBanhOnline db = new BanBanhOnline();

        // GET: PrivatePages/NewProduct
        [HttpGet]
        public ActionResult Index()
        {
            try {
                sanPham z = new sanPham();
                z.ngayDang = DateTime.Now;
                z.taiKhoan = ThuongDung.getTentaiKhoan();
                ViewBag.ddhinh = "/Dulieu/Images/image_upload_1.jpg";
                ViewBag.MaLoaiList = GetMaLoaiList();
                return View(z);
            }
            catch (Exception ex) {
                // Log lỗi và hiển thị thông báo
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: PrivatePages/NewProduct
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(sanPham z, HttpPostedFileBase hinhdaidien)
        {
            try {
                // Thêm lại danh sách loại sản phẩm để tránh mất khi form không hợp lệ
                ViewBag.MaLoaiList = GetMaLoaiList();

                // Validate dữ liệu đầu vào
                ValidateProductData(z, hinhdaidien);

                if (ModelState.IsValid) {
                    // Khởi tạo thông tin cơ bản
                    z.maSP = string.Format("{0:yyMMddhhmm}", DateTime.Now);
                    z.daDuyet = true;
                    z.ngayDang = DateTime.Now;
                    z.taiKhoan = ThuongDung.getTentaiKhoan();

                    // Xử lý hình ảnh nếu có upload mới
                    if (hinhdaidien != null) {
                        string fileName = SaveProductImage(hinhdaidien, z.maSP);
                        z.hinhDD = "/images/image_product/" + fileName;
                    }

                    // Lưu sản phẩm vào database
                    SaveProduct(z);

                    // Thông báo thành công
                    TempData["SuccessMessage"] = "Đăng sản phẩm thành công!";
                    return RedirectToAction("Index");
                }

                // Nếu có lỗi, hiển thị lại form với dữ liệu đã nhập
                ViewBag.ddhinh = z.hinhDD ?? "/Dulieu/Images/image_upload_1.jpg";
                return View(z);
            }
            catch (Exception ex) {
                // Log lỗi
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm: " + ex.Message);
                ViewBag.ddhinh = z.hinhDD ?? "/Dulieu/Images/image_upload_1.jpg";
                return View(z);
            }
        }

        // Lấy danh sách loại sản phẩm
        private List<SelectListItem> GetMaLoaiList()
        {
            return db.loaiSPs.Select(l => new SelectListItem {
                Value = l.maLoai.ToString(),
                Text = l.loaiSP1
            }).ToList();
        }

        // Validate dữ liệu sản phẩm
        private void ValidateProductData(sanPham z, HttpPostedFileBase hinhdaidien)
        {
            if (string.IsNullOrWhiteSpace(z.tenSP)) {
                ModelState.AddModelError("tenSP", "Vui lòng nhập tên sản phẩm");
            }

            if (string.IsNullOrWhiteSpace(z.ndTomTat)) {
                ModelState.AddModelError("ndTomTat", "Vui lòng nhập nội dung tóm tắt");
            }

            if (string.IsNullOrWhiteSpace(z.noiDung)) {
                ModelState.AddModelError("noiDung", "Vui lòng nhập nội dung chi tiết");
            }

            if (z.giaBan <= 0) {
                ModelState.AddModelError("giaBan", "Giá bán phải lớn hơn 0");
            }

            if (z.maLoai == null) {
                ModelState.AddModelError("maLoai", "Vui lòng chọn loại sản phẩm");
            }

            // Chỉ validate hình ảnh khi chưa có hình và không có file upload mới
            if (hinhdaidien == null && string.IsNullOrEmpty(z.hinhDD)) {
                ModelState.AddModelError("hinhDD", "Vui lòng chọn hình ảnh đại diện");
            }
        }

        // Lưu hình ảnh sản phẩm
        private string SaveProductImage(HttpPostedFileBase imageFile, string productId)
        {
            if (imageFile != null && imageFile.ContentLength > 0) {
                string fileName = "HDD" + productId + Path.GetExtension(imageFile.FileName);
                string physicalPath = Server.MapPath("~/images/image_product");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(physicalPath)) {
                    Directory.CreateDirectory(physicalPath);
                }

                string filePath = Path.Combine(physicalPath, fileName);
                imageFile.SaveAs(filePath);

                return fileName;
            }
            return null;
        }

        // Lưu thông tin sản phẩm vào database
        private void SaveProduct(sanPham product)
        {
            try {
                db.sanPhams.Add(product);
                db.SaveChanges();
            }
            catch (Exception ex) {
                throw new Exception("Lỗi khi lưu sản phẩm vào database: " + ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}