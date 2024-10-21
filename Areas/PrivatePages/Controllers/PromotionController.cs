using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models; // Thư viện cho models của bạn

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class PromotionController : Controller
    {
        // GET: PrivatePages/Promotion
        public ActionResult Index()
        {
            using (var db = new BanBanhOnline()) {
                // Gọi phương thức để cập nhật giá trị giảm giá
                UpdateDiscounts();
                // Lấy danh sách khuyến mãi còn hiệu lực (ngày kết thúc lớn hơn ngày hiện tại)
                var danhSachKhuyenMai = db.KhuyenMais
                    .Where(km => km.trangThai == true && km.ngayKetThuc > DateTime.Now)
                    .ToList(); // Lấy danh sách khuyến mãi
                
                return View(danhSachKhuyenMai); // Trả về view danh sách khuyến mãi
            }
        }
        

    [HttpGet]
        public ActionResult Create()
        {
            taiKhoanTV tk = Session["ttDangNhap"] as taiKhoanTV;
            if (tk == null) {
                // Nếu không có tài khoản trong Session, redirect đến trang đăng nhập
                return RedirectToAction("Login", "Loginadmin"); // Chỉnh sửa thành tên controller và action đúng của bạn
            }

            KhuyenMai khuyenMai = new KhuyenMai {
                ngayBatDau = DateTime.Now, // Ngày bắt đầu mặc định
                taiKhoan = tk.taiKhoan // Lấy tài khoản đăng nhập từ Session
            };

            ViewBag.SanPhamList = GetSanPhamList(); // Lấy danh sách sản phẩm từ database
            return View(khuyenMai);
        }

        // Hàm lấy danh sách sản phẩm và chuyển đổi mã sản phẩm thành tên sản phẩm
        private List<SelectListItem> GetSanPhamList()
        {
            using (var db = new BanBanhOnline()) {
                return db.sanPhams.Select(sp => new SelectListItem {
                    Value = sp.maSP.ToString(),
                    Text = sp.tenSP // Giả sử bạn có thuộc tính tên sản phẩm
                }).ToList();
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(KhuyenMai khuyenMai)
        {
            // Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(khuyenMai.maSP)) {
                ModelState.AddModelError("maSP", "Vui lòng chọn mã sản phẩm.");
            }
            if (string.IsNullOrWhiteSpace(khuyenMai.noiDung)) {
                ModelState.AddModelError("noiDung", "Vui lòng nhập nội dung khuyến mãi.");
            }
            if (khuyenMai.giamGia <= 0) {
                ModelState.AddModelError("giamGia", "Vui lòng nhập mức giảm giá hợp lệ.");
            }
            if (khuyenMai.ngayKetThuc < khuyenMai.ngayBatDau) {
                ModelState.AddModelError("ngayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // Kiểm tra tài khoản từ session
            taiKhoanTV tk = Session["ttDangNhap"] as taiKhoanTV;
            if (tk != null) {
                khuyenMai.taiKhoan = tk.taiKhoan; // Gán tài khoản vào khuyến mãi
            }
            else {
                ModelState.AddModelError("taiKhoan", "Bạn cần đăng nhập để thực hiện hành động này.");
            }

            // Nếu không có lỗi, tiến hành lưu khuyến mãi
            if (ModelState.IsValid) {
                // Tạo mã khuyến mãi
                khuyenMai.maKM = string.Format("{0:yyMMddhhmm}", DateTime.Now);
                khuyenMai.ngayBatDau = DateTime.Now; // Ngày bắt đầu
                khuyenMai.trangThai = true;
                using (var db = new BanBanhOnline()) {
                    // Thêm khuyến mãi vào database
                    db.KhuyenMais.Add(khuyenMai);
                    db.SaveChanges(); // Lưu thay đổi vào database

                    // Cập nhật sản phẩm với mức giảm giá
                    var sanPham = db.sanPhams.FirstOrDefault(sp => sp.maSP == khuyenMai.maSP);
                    if (sanPham != null) {
                        sanPham.giamGia = khuyenMai.giamGia; // Gán mức giảm giá cho sản phẩm
                        db.SaveChanges(); // Lưu thay đổi vào database
                    }
                }

                // Thông báo đăng khuyến mãi thành công bằng TempData
                TempData["SuccessMessage"] = "Đăng khuyến mãi thành công!";

                // Redirect về action Index để làm mới trang
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, cần lấy lại danh sách sản phẩm và hiển thị lại form
            ViewBag.SanPhamList = GetSanPhamList();
            return View(khuyenMai);
        }
        /// <summary>
        /// hàm sửa promotion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(string id)
        {
            using (var db = new BanBanhOnline()) {
                // Tìm khuyến mãi theo mã khuyến mãi
                var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.maKM == id);
                if (khuyenMai == null) {
                    return HttpNotFound(); // Nếu không tìm thấy khuyến mãi
                }

                // Lấy danh sách sản phẩm
                ViewBag.SanPhamList = GetSanPhamList();

                // Trả về view để chỉnh sửa khuyến mãi
                return View(khuyenMai);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(KhuyenMai khuyenMai)
        {
            // Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(khuyenMai.maSP)) {
                ModelState.AddModelError("maSP", "Vui lòng chọn mã sản phẩm.");
            }
            if (string.IsNullOrWhiteSpace(khuyenMai.noiDung)) {
                ModelState.AddModelError("noiDung", "Vui lòng nhập nội dung khuyến mãi.");
            }
            if (khuyenMai.giamGia <= 0) {
                ModelState.AddModelError("giamGia", "Vui lòng nhập mức giảm giá hợp lệ.");
            }
            if (khuyenMai.ngayKetThuc < khuyenMai.ngayBatDau) {
                ModelState.AddModelError("ngayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // Nếu không có lỗi, lưu thay đổi
            if (ModelState.IsValid) {
                using (var db = new BanBanhOnline()) {
                    var kmDb = db.KhuyenMais.FirstOrDefault(km => km.maKM == khuyenMai.maKM);
                    if (kmDb != null) {
                        // Cập nhật thông tin khuyến mãi
                        kmDb.noiDung = khuyenMai.noiDung;
                        kmDb.giamGia = khuyenMai.giamGia;
                        kmDb.ngayBatDau = khuyenMai.ngayBatDau;
                        kmDb.ngayKetThuc = khuyenMai.ngayKetThuc;
                        kmDb.maSP = khuyenMai.maSP;

                        db.SaveChanges(); // Lưu thay đổi

                        // Cập nhật giảm giá cho sản phẩm nếu cần
                        var sanPham = db.sanPhams.FirstOrDefault(sp => sp.maSP == khuyenMai.maSP);
                        if (sanPham != null) {
                            sanPham.giamGia = khuyenMai.giamGia;
                            db.SaveChanges();
                        }
                    }

                    // Thông báo chỉnh sửa thành công
                    TempData["SuccessMessage"] = "Chỉnh sửa khuyến mãi thành công!";
                    return RedirectToAction("Index");
                }
            }

            // Nếu có lỗi, lấy lại danh sách sản phẩm
            ViewBag.SanPhamList = GetSanPhamList();
            return View(khuyenMai);
        }

        /// <summary>
        /// hàm xử lý ẩn khuyến mãi  
        /// </summary>
        public ActionResult Hide(string id)
        {
            using (var db = new BanBanhOnline()) {
                // Tìm khuyến mãi theo mã khuyến mãi
                var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.maKM == id);
                if (khuyenMai != null) {
                    khuyenMai.trangThai = false; // Đánh dấu là ẩn
                    db.SaveChanges(); // Lưu thay đổi

                    // Cập nhật sản phẩm: Đặt giảm giá về 0
                    var sanPham = db.sanPhams.FirstOrDefault(sp => sp.maSP == khuyenMai.maSP);
                    if (sanPham != null) {
                        sanPham.giamGia = 0;
                        db.SaveChanges();
                    }

                    // Thông báo ẩn thành công
                    TempData["SuccessMessage"] = "Ẩn khuyến mãi thành công!";
                }
                return RedirectToAction("Index");
            }
        }

        // Hàm hiển thị khuyến mãi đang hoạt động
        public ActionResult ActivePromotions()
        {
            using (var db = new BanBanhOnline()) {
                // Lấy danh sách khuyến mãi đang hoạt động (trangThai = true)
                var danhSachKhuyenMaiHoatDong = db.KhuyenMais
                    .Where(km => km.trangThai == true && km.ngayKetThuc >= DateTime.Now)
                    .ToList(); // Lấy danh sách khuyến mãi

                return View(danhSachKhuyenMaiHoatDong); // Trả về view danh sách khuyến mãi hoạt động
            }
        }

        // Hàm hiển thị khuyến mãi đã ẩn
        public ActionResult HiddenPromotions()
        {
            using (var db = new BanBanhOnline()) {
                // Lấy danh sách khuyến mãi đã ẩn (trangThai = false)
                var danhSachKhuyenMaiDaAn = db.KhuyenMais
                    .Where(km => km.trangThai == false)
                    .ToList(); // Lấy danh sách khuyến mãi

                return View(danhSachKhuyenMaiDaAn); // Trả về view danh sách khuyến mãi đã ẩn
            }
        }

        /// <summary>
        /// Hàm tìm kiếm theo mã khuyến mãi theo trạng thái false
        /// </summary>
        public ActionResult TimKiemKM(string searchMaKM)
        {
            using (var db = new BanBanhOnline()) {
                var khuyenMaiList = db.KhuyenMais.Where(k => k.maKM.Contains(searchMaKM) && k.trangThai == false).ToList();
                // sử dụng "HiddenPromotions" để truyền trực tiếp vào view
                return View("HiddenPromotions", khuyenMaiList);
               
            }
        }

        // Hàm cập nhật giá trị giảm giá về 0 cho khuyến mãi đã hết hạn
        private void UpdateDiscounts()
        {
            using (var db = new BanBanhOnline()) {
                // Lấy danh sách khuyến mãi đã kết thúc
                var khuyenMaisDaKetThuc = db.KhuyenMais
                    .Where(km => km.ngayKetThuc <= DateTime.Now)
                    .ToList();

                foreach (var km in khuyenMaisDaKetThuc) {
                    // Tìm sản phẩm liên quan tới khuyến mãi đã hết hạn
                    var sanPham = db.sanPhams.FirstOrDefault(sp => sp.maSP == km.maSP);

                    if (sanPham != null) {
                        // Đặt lại mức giảm giá về 0
                        sanPham.giamGia = 0;
                        // Lưu thay đổi vào database
                        db.Entry(sanPham).State = System.Data.Entity.EntityState.Modified;
                    }
                    km.trangThai = false;
                    // Xóa hoặc đánh dấu khuyến mãi đã hết hạn nếu cần
                    km.giamGia = 0; // Cập nhật khuyến mãi cũng về 0 nếu muốn
                    db.Entry(km).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges(); // Lưu tất cả thay đổi vào database
            }
        }

    }
}
