using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
