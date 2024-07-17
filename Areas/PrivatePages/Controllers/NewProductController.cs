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

        // GET: PrivatePages/NewProduct
        [HttpGet]
        public ActionResult Index()
        {
            sanPham z = new sanPham();
            z.ngayDang = DateTime.Now;
            z.taiKhoan = ThuongDung.getTentaiKhoan();

            ViewBag.ddhinh = "/Dulieu/Images/image_upload_1.jpg";
            ViewBag.MaLoaiList = GetMaLoaiList(); // Lấy danh sách mã loại từ database

            return View(z);
        }
        // hàm chuyển đỏi mã loại thành tên mã loại 
        private List<SelectListItem> GetMaLoaiList()
        {
            using (var db = new BanBanhOnline()) {
                return db.loaiSPs.Select(l => new SelectListItem {
                    Value = l.maLoai.ToString(),
                    Text = l.loaiSP1
                }).ToList();
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Index(sanPham z, HttpPostedFileBase hinhdaidien)
        {
            // Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(z.tenSP)) {
                ModelState.AddModelError("tenSP", "Vui lòng nhập tên sản phẩm.");
            }
            if (string.IsNullOrWhiteSpace(z.ndTomTat)) {
                ModelState.AddModelError("ndTomTat", "Vui lòng nhập nội dung tóm tắt.");
            }
            if (string.IsNullOrWhiteSpace(z.noiDung)) {
                ModelState.AddModelError("noiDung", "Vui lòng nhập nội dung chính.");
            }
            if (z.giaBan == null) {
                ModelState.AddModelError("giaBan", "Vui lòng nhập giá bán.");
            }
            if (z.hinhDD == null) {
                ModelState.AddModelError("hinhDD", "Vui lòng chọn hình ảnh đại diện.");
            }
            if (z.maLoai == null) {
                ModelState.AddModelError("maLoai", "Vui lòng chọn loại sản phẩm.");
            }
                
            // Nếu không có lỗi, tiến hành lưu sản phẩm
            if (ModelState.IsValid) {
                // Xử lí thông tin sản phẩm
                z.maSP = string.Format("{0:yyMMddhhmm}", DateTime.Now);
                z.daDuyet = true;
                z.ngayDang = DateTime.Now;
                z.taiKhoan = ThuongDung.getTentaiKhoan();

                // Lưu hình vào thư mục chứa sản phẩm
                string virpath = "/images/image_product";
                string phypath = Server.MapPath("~/" + virpath);
                string ext = Path.GetExtension(hinhdaidien.FileName);
                string fileName = "HDD" + z.maSP + ext;
                hinhdaidien.SaveAs(Path.Combine(phypath, fileName));
                z.hinhDD = virpath + fileName;

                // Lưu sản phẩm vào database
                using (BanBanhOnline dbd = new BanBanhOnline()) {
                    dbd.sanPhams.Add(z);
                    dbd.SaveChanges();
                }

                // Thông báo đăng ảnh thành công bằng TempData
                TempData["SuccessMessage"] = "Đăng sản phẩm thành công!";

                // Redirect về action Index để làm mới trang
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, cần lấy lại danh sách mã loại và hiển thị lại form
            ViewBag.MaLoaiList = GetMaLoaiList();
            ViewBag.ddhinh = z.hinhDD ?? "/Dulieu/Images/image_upload_1.jpg"; // Giữ lại hình đã chọn trước đó

            return View(z);
        }

    }

}
