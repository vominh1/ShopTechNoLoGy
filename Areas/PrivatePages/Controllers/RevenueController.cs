using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class RevenueController : Controller
    {
        private readonly BanBanhOnline db = new BanBanhOnline();
  

        // GET: PrivatePages/Revenue
        public ActionResult Index()
        {
            var doanhThuNgay = db.Database.SqlQuery<DoanhThuNgayModel>(
                "SELECT CAST(dh.ngayDat AS DATE) AS Ngay, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL) AS DoanhThu " +
                "FROM donHang dh JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "GROUP BY CAST(dh.ngayDat AS DATE) " +
                "ORDER BY Ngay"
            ).ToList();

            return View(doanhThuNgay);
        }
        public ActionResult Doanhthuthang()
        {
            // Sử dụng CAST
            var doanhThuThang = db.Database.SqlQuery<DoanhThuThangModel>(
                "SELECT MONTH(dh.ngayDat) AS Thang, YEAR(dh.ngayDat) AS Nam, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM donHang dh JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "GROUP BY MONTH(dh.ngayDat), YEAR(dh.ngayDat) " +
                "ORDER BY Nam, Thang"
            ).ToList();


            return View(doanhThuThang);
        }
        // khai báo phương thức action method
        public ActionResult ChiTietDoanhThuNgaytrongthang(int thang, int nam, int? page)
        {
            int pageNumber = page ?? 1; // Nếu không có giá trị page, mặc định là 1
            int pageSize = 10; // Số lượng mục trên mỗi trang

            var chiTietNgay = db.Database.SqlQuery<ChiTietNgayModel>(
                "SELECT dh.ngayDat AS Ngay, " +
                "CAST(SUM(CAST(ct.giaBan * ct.soLuong - ct.giamGia AS DECIMAL(18, 2))) AS DECIMAL(18, 2)) AS DoanhThuNgay " +
                "FROM donHang dh " +
                "JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "WHERE MONTH(dh.ngayDat) = @p0 AND YEAR(dh.ngayDat) = @p1 " +
                "GROUP BY dh.ngayDat",
                thang, nam
            ).ToList();

            var modelPagedList = chiTietNgay.ToPagedList(pageNumber, pageSize);

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            return View(modelPagedList);
        }


        public ActionResult DoanhThuSanPham()
        {
            var doanhThuSanPham = db.Database.SqlQuery<DoanhThuSanPhamModel>(
                "SELECT sp.tenSP AS TenSanPham, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM sanPham sp JOIN ctDonHang ct ON sp.maSP = ct.maSP " +
                "GROUP BY sp.tenSP " +
                "ORDER BY DoanhThu DESC"
            ).ToList();

            return View(doanhThuSanPham);
        }

    }
}
