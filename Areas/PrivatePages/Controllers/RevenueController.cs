using System;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using ShopTechNoLoGy.Models;
using System.Data.SqlClient;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class RevenueController : Controller
    {
        private readonly BanBanhOnline db = new BanBanhOnline();
        // GET: PrivatePages/Revenue
        // hàm index dùm để hiển thị doanh thu ngày

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
        /// <summary>
        /// hàm so sanh doanh thu ngay với ngày
        /// </summary>
        /// <param name="ngayBatDau"></param>
        /// <param name="ngayKetThuc"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetChonNgaySoSanh(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            // Kiểm tra và điều chỉnh năm nếu quá dài
            string yearBatDau = ngayBatDau.Year.ToString();
            string yearKetThuc = ngayKetThuc.Year.ToString();

            if (yearBatDau.Length > 4) {
                yearBatDau = yearBatDau.Substring(0, 4); // Chỉ lấy 4 ký tự đầu tiên
            }

            if (yearKetThuc.Length > 4) {
                yearKetThuc = yearKetThuc.Substring(0, 4); // Chỉ lấy 4 ký tự đầu tiên
            }

            // Chuyển lại về DateTime với năm đã điều chỉnh
            ngayBatDau = new DateTime(int.Parse(yearBatDau), ngayBatDau.Month, ngayBatDau.Day);
            ngayKetThuc = new DateTime(int.Parse(yearKetThuc), ngayKetThuc.Month, ngayKetThuc.Day);

            // Kiểm tra xem có dữ liệu trong 2 ngày này không
            var checkData = db.Database.SqlQuery<int>(
                "SELECT COUNT(DISTINCT CAST(dh.ngayDat AS DATE)) " +
                "FROM donHang dh " +
                "WHERE CAST(dh.ngayDat AS DATE) BETWEEN @p0 and @p1", // IN (@p0, @p1) nếu muôn so sánh giữa ngày và 1 ngày
                ngayBatDau.Date, ngayKetThuc.Date
            ).FirstOrDefault();

            // Nếu không có dữ liệu, trả về thông báo
            if (checkData == 0) {
                return Json(new { message = "Không có dữ liệu trong các ngày đã chọn." }, JsonRequestBehavior.AllowGet);
            }

            // Thực hiện truy vấn lấy dữ liệu cho 2 ngày cụ thể
            var data = db.Database.SqlQuery<DoanhThuNgayModel>(
                 "SELECT CAST(dh.ngayDat AS DATE) AS Ngay, " +
                 "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL) AS DoanhThu " +
                 "FROM donHang dh " +
                 "JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                 "WHERE CAST(dh.ngayDat AS DATE) BETWEEN @p0 and @p1 " + // IN (@p0, @p1)
                 "GROUP BY CAST(dh.ngayDat AS DATE) " +
                 "ORDER BY Ngay",
                 ngayBatDau.Date, ngayKetThuc.Date
             ).ToList();

            var result = data.Select(d => new {
                Ngay = d.Ngay.ToString("dd-MM-yyyy"),
                DoanhThu = d.DoanhThu.ToString("C0")
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Doanhthuthang()
        {

            var doanhThuThang = db.Database.SqlQuery<DoanhThuThangModel>(
                "SELECT MONTH(dh.ngayDat) AS Thang, YEAR(dh.ngayDat) AS Nam, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM donHang dh JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "GROUP BY MONTH(dh.ngayDat), YEAR(dh.ngayDat) " +
                "ORDER BY Nam, Thang"
            ).ToList();


            return View(doanhThuThang);
        }

        [HttpGet]
        public JsonResult GetChonThangSoSanh(int thang1, int nam1, int thang2, int nam2)
        {
            // Kiểm tra dữ liệu trong 2 tháng này
            var checkData = db.Database.SqlQuery<int>(
                "SELECT COUNT(*) " +
                "FROM donHang dh " +
                "WHERE (MONTH(dh.ngayDat) = @p0 AND YEAR(dh.ngayDat) = @p1) " +
                "OR (MONTH(dh.ngayDat) = @p2 AND YEAR(dh.ngayDat) = @p3)",
                thang1, nam1, thang2, nam2
            ).FirstOrDefault();

            // Nếu không có dữ liệu, trả về thông báo
            if (checkData == 0) {
                return Json(new { message = "Không có dữ liệu trong các tháng đã chọn." }, JsonRequestBehavior.AllowGet);
            }

            // Thực hiện truy vấn lấy dữ liệu cho 2 tháng
            var data = db.Database.SqlQuery<DoanhThuThangModel>(
                "SELECT MONTH(dh.ngayDat) AS Thang, YEAR(dh.ngayDat) AS Nam, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM donHang dh " +
                "JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "WHERE (MONTH(dh.ngayDat) = @p0 AND YEAR(dh.ngayDat) = @p1) " +
                "OR (MONTH(dh.ngayDat) = @p2 AND YEAR(dh.ngayDat) = @p3) " +
                "GROUP BY MONTH(dh.ngayDat), YEAR(dh.ngayDat) " +
                "ORDER BY Nam, Thang",
                thang1, nam1, thang2, nam2
            ).ToList();

            var result = data.Select(d => new {
                ThangNam = $"Tháng {d.Thang}/{d.Nam}",
                DoanhThu = d.DoanhThu.ToString("C0")
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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
        public ActionResult ExportDaylyRevenueToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // Set license context here

            var doanhThuNgay = db.Database.SqlQuery<DoanhThuNgayModel>(
                "SELECT CAST(dh.ngayDat AS DATE) AS Ngay, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL) AS DoanhThu " +
                "FROM donHang dh JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "GROUP BY CAST(dh.ngayDat AS DATE) " +
                "ORDER BY Ngay"
            ).ToList();

            using (var package = new ExcelPackage()) {
                var worksheet = package.Workbook.Worksheets.Add("Doanh Thu Ngay");

                // Add headers
                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Doanh thu";

                // Add data
                for (int i = 0; i < doanhThuNgay.Count; i++) {
                    worksheet.Cells[i + 2, 1].Value = doanhThuNgay[i].Ngay.ToShortDateString();
                    worksheet.Cells[i + 2, 2].Value = doanhThuNgay[i].DoanhThu;
                }

                // Adjust column width
                worksheet.Cells.AutoFitColumns();

                // Convert the ExcelPackage to a byte array
                var stream = new MemoryStream();
                package.SaveAs(stream);

                // Set the stream position to the beginning
                stream.Position = 0;

                // Return the file result
                var fileName = "DoanhThuNgay.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public ActionResult ExportMonthlyRevenueToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // Set the license context

            // tạo câu truy vấn dữ liệu theo tháng
            var doanhThuThang = db.Database.SqlQuery<DoanhThuThangModel>(
                "SELECT MONTH(dh.ngayDat) AS Thang, YEAR(dh.ngayDat) AS Nam, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM donHang dh JOIN ctDonHang ct ON dh.soDH = ct.soDH " +
                "GROUP BY MONTH(dh.ngayDat), YEAR(dh.ngayDat) " +
                "ORDER BY Nam, Thang"
            ).ToList();

            using (var package = new ExcelPackage()) {
                var worksheet = package.Workbook.Worksheets.Add("Doanh Thu Thang");

                // Thêm cột đầu 
                worksheet.Cells[1, 1].Value = "Năm";
                worksheet.Cells[1, 2].Value = "Tháng";
                worksheet.Cells[1, 3].Value = "Doanh thu";

                // thêm dữ liệu
                for (int i = 0; i < doanhThuThang.Count; i++) {
                    worksheet.Cells[i + 2, 1].Value = doanhThuThang[i].Nam;
                    worksheet.Cells[i + 2, 2].Value = doanhThuThang[i].Thang;
                    worksheet.Cells[i + 2, 3].Value = doanhThuThang[i].DoanhThu;
                }

                // điều chỉnh chiều rộng
                worksheet.Cells.AutoFitColumns();


                var stream = new MemoryStream();
                package.SaveAs(stream);

                // Set the stream position to the beginning
                stream.Position = 0;

                // Return the file result
                var fileName = "DoanhThuThang.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public ActionResult ExportProductRevenueToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // Set the license context

            // Fetch the product revenue data
            var doanhThuSanPham = db.Database.SqlQuery<DoanhThuSanPhamModel>(
                "SELECT sp.tenSP AS TenSanPham, " +
                "CAST(SUM(ct.giaBan * ct.soLuong - ct.giamGia) AS DECIMAL(18, 2)) AS DoanhThu " +
                "FROM sanPham sp JOIN ctDonHang ct ON sp.maSP = ct.maSP " +
                "GROUP BY sp.tenSP " +
                "ORDER BY DoanhThu DESC"
            ).ToList();

            using (var package = new ExcelPackage()) {
                var worksheet = package.Workbook.Worksheets.Add("Doanh Thu San Pham");

                // Add headers
                worksheet.Cells[1, 1].Value = "Tên Sản Phẩm";
                worksheet.Cells[1, 2].Value = "Doanh thu";

                // Add data
                for (int i = 0; i < doanhThuSanPham.Count; i++) {
                    worksheet.Cells[i + 2, 1].Value = doanhThuSanPham[i].TenSanPham;
                    worksheet.Cells[i + 2, 2].Value = doanhThuSanPham[i].DoanhThu;
                }

                // Adjust column width
                worksheet.Cells.AutoFitColumns();

                // Convert the ExcelPackage to a byte array
                var stream = new MemoryStream();
                package.SaveAs(stream);

                // Set the stream position to the beginning
                stream.Position = 0;

                // Return the file result
                var fileName = "DoanhThuSanPham.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
