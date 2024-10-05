using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class WareHouseController : Controller
    {
        private readonly BanBanhOnline _context;

        public WareHouseController()
        {
            _context = new BanBanhOnline();
        }

        // GET: PrivatePages/WareHouse
        public ActionResult Index()
        {
            var nhapKhoList = _context.NhapKhoes.Include("sanPham").ToList();
            var xuatKhoList = _context.XuatKhoes.Include("sanPham").ToList(); // Lấy danh sách xuất kho
            ViewBag.XuatKhoList = xuatKhoList; // Đưa danh sách xuất kho vào ViewBag
            return View(nhapKhoList);
        }

        // GET: PrivatePages/WareHouse/Create
        public ActionResult Create()
        {
            ViewBag.SanPhamList = _context.sanPhams.ToList();
            return View(new NhapKho()); // Khởi tạo mô hình mới
        }

        // POST: PrivatePages/WareHouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhapKho nhapKho)
        {
            if (ModelState.IsValid) {
                // Lấy thông tin tài khoản từ session
                var taiKhoanHienTai = Session["ttDangNhap"] as taiKhoanTV; // Giả sử ttDangNhap lưu đối tượng tài khoản

                if (taiKhoanHienTai == null) {
                    ModelState.AddModelError("", "Bạn chưa đăng nhập.");
                    ViewBag.SanPhamList = _context.sanPhams.ToList();
                    return View(nhapKho);
                }

                nhapKho.NguoiNhap = taiKhoanHienTai.taiKhoan; // Lưu tài khoản hiện tại vào NguoiNhap
                nhapKho.taiKhoan = taiKhoanHienTai.taiKhoan; // Gán tài khoản vào trường taiKhoan

                var product = _context.sanPhams.Find(nhapKho.maSP);
                if (product != null) {
                    product.SoLuongTonKho += nhapKho.SoLuongNhap ?? 0; // Cập nhật số lượng tồn kho
                    nhapKho.NgayNhap = DateTime.Now; // Thiết lập ngày nhập
                    _context.NhapKhoes.Add(nhapKho); // Thêm vào bảng NhapKho
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Sản phẩm không tồn tại.");
            }

            ViewBag.SanPhamList = _context.sanPhams.ToList();
            return View(nhapKho);
        }
        // GET: PrivatePages/WareHouse
        public ActionResult dsExport()
        {
            var xuatKhoList = _context.XuatKhoes.Include("sanPham").ToList();
            return View(xuatKhoList);
        }

        public ActionResult Export()
        {
            ViewBag.SanPhamList = _context.sanPhams.ToList();
            return View(new XuatKho()); // Khởi tạo mô hình mới
        }

        // POST: PrivatePages/WareHouse/Export
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(XuatKho xuatKho)
        {
            if (ModelState.IsValid) {
                // Lấy thông tin tài khoản từ session
                var taiKhoanHienTai = Session["ttDangNhap"] as taiKhoanTV;

                if (taiKhoanHienTai == null) {
                    ModelState.AddModelError("", "Bạn chưa đăng nhập.");
                    ViewBag.SanPhamList = _context.sanPhams.ToList();
                    return View(xuatKho);
                }

                // Thiết lập thông tin cho xuatKho
                xuatKho.NguoiXuat = taiKhoanHienTai.taiKhoan;
                xuatKho.taiKhoan = taiKhoanHienTai.taiKhoan;
                xuatKho.NgayXuat = DateTime.Now; // Thiết lập ngày xuất

                // Tìm sản phẩm theo mã sản phẩm
                var product = _context.sanPhams.Find(xuatKho.maSP);
                if (product != null) {
                    // Kiểm tra số lượng xuất kho
                    if (xuatKho.SoLuongXuat > product.SoLuongTonKho) {
                        ModelState.AddModelError("", "Số lượng xuất không hợp lệ. Không đủ hàng trong kho.");
                    }
                    else {
                        // Cập nhật số lượng tồn kho
                        product.SoLuongTonKho -= xuatKho.SoLuongXuat ?? 0;
                        _context.XuatKhoes.Add(xuatKho); // Thêm vào bảng XuatKho
                        _context.SaveChanges(); // Lưu thay đổi
                        return RedirectToAction("Index"); // Quay lại trang danh sách
                    }
                }
                ModelState.AddModelError("", "Sản phẩm không tồn tại.");
            }

            // Nếu có lỗi, hiển thị lại danh sách sản phẩm
            ViewBag.SanPhamList = _context.sanPhams.ToList();
            return View(xuatKho);
        }

        // tồn kho 
        public ActionResult TonKho(string searchName, string categoryId, int? page)
        {
            var sanPhamList = _context.sanPhams.AsQueryable();

            // Tìm kiếm theo tên sản phẩm nếu có nhập
            if (!string.IsNullOrEmpty(searchName)) {
                sanPhamList = sanPhamList.Where(sp => sp.tenSP.Contains(searchName));
            }

            // Tìm kiếm theo mã loại sản phẩm nếu có nhập
            if (!string.IsNullOrEmpty(categoryId)) {
                sanPhamList = sanPhamList.Where(sp => sp.maSP == categoryId);
            }

            // Sắp xếp danh sách sản phẩm
            sanPhamList = sanPhamList.OrderBy(sp => sp.tenSP);

            // Thiết lập số lượng sản phẩm trên mỗi trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            // Trả về danh sách sản phẩm dưới dạng phân trang
            return View(sanPhamList.ToPagedList(pageNumber, pageSize));
        }



    }
}