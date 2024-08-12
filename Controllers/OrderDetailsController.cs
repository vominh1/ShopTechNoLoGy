using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;

namespace ShopTechNoLoGy.Controllers
{
    public class OrderDetailsController : Controller
    {
        private BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        public ActionResult Index()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (Session["ttDangNhap"] == null) {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("Index", "OrderDetails") });
            }

            HienThiDonHangdadat();
            
            // Trả về view hiển thị danh sách đơn hàng
            return View();
        }
        [HttpGet]
        public ActionResult ChuaXuLy()
        {
            if (Session["ttDangNhap"] == null) {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("ChuaXuLy", "OrderDetails") });
            }
            HienThiDonHangChuaXuLy();
            return View();
        }
        [HttpPost]
        public ActionResult ChiTietDonHang(string soDH)
        {
            // Lấy chi tiết đơn hàng
            var chiTiet = db.ctDonHangs
                .Where(x => x.soDH == soDH)
                .ToList();

            // Lấy thông tin đơn hàng
            var donHang = db.donHangs
                .FirstOrDefault(x => x.soDH == soDH);

            if (donHang == null) {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return HttpNotFound();
            }

            // Lấy thông tin khách hàng từ đơn hàng
            var khachHang = db.khachHangs
                .FirstOrDefault(x => x.maKH == donHang.maKH);

            // Truyền dữ liệu vào ViewData sau đó truyền vào view 
            ViewData["ChiTietDonHang"] = chiTiet;
            ViewData["KhachHang"] = khachHang;
            ViewData["Donhang"] = donHang;

            return View("ChiTietDonHang");
        }

        [HttpPost]
        public ActionResult ChiTietDonHangChuaXuLy(string soDH)
        {
            //lọc chi tiết đơn hàng theo số đơn hàng và truyền vào viewdata 

            // Lấy chi tiết đơn hàng
            var chiTiet = db.ctDonHangs
                .Where(x => x.soDH == soDH)
                .ToList();

            // Lấy thông tin đơn hàng
            var donHang = db.donHangs
                .FirstOrDefault(x => x.soDH == soDH);

            if (donHang == null) {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return HttpNotFound();
            }

            // Lấy thông tin khách hàng từ đơn hàng
            var khachHang = db.khachHangs
                .FirstOrDefault(x => x.maKH == donHang.maKH);
            ViewData["ChiTietDonHangChuaXuLy"] = chiTiet;
            ViewData["KhachHang"] = khachHang;
            ViewData["Donhang"] = donHang;
            return View("ChiTietDonHangChuaXuLy");
        }

        private ActionResult HienThiDonHangdadat()
        {
            // Lấy thông tin tài khoản từ session hoặc cơ chế xác thực
            taiKhoanTV currentUser = Session["ttDangNhap"] as taiKhoanTV;

            if (currentUser != null) {
                // Lọc đơn hàng theo tài khoản đã đăng nhập
                List<donHang> l = db.donHangs
                    .Where(x => x.daKichHoat == true && x.taiKhoan == currentUser.taiKhoan)
                    .ToList();
                //Lưu đơn hàng đã lọc vào trong viewdata
                ViewData["DanhSachDonHangdadat"] = l;
            }
            else {
                // Xử lý trường hợp không có tài khoản trong session
                ViewData["DanhSachDonHangdadat"] = new List<donHang>();
            }

            return View();
        }
        private void HienThiDonHangChuaXuLy()
        {
            // gán currentusse vào "ttDangNhap"
            taiKhoanTV currentUser = Session["ttDangNhap"] as taiKhoanTV;
            // kiểm tra điều kiện
            if (currentUser != null) {
                List<donHang> l = db.donHangs
                    .Where(x => x.daKichHoat == false && x.taiKhoan == currentUser.taiKhoan)
                    .ToList();
                ViewData["DanhSachDonHangChuaXuLy"] = l;
            }
            else {
                ViewData["DanhSachDonHangChuaXuLy"] = new List<donHang>();
            }
        }
        public ActionResult ExportPdf(string soDH)
        {
            // Lấy thông tin đơn hàng
            var chiTiet = db.ctDonHangs.Where(x => x.soDH == soDH).ToList();
            var donHang = db.donHangs.FirstOrDefault(x => x.soDH == soDH);
            var khachHang = db.khachHangs.FirstOrDefault(x => x.maKH == donHang.maKH);

            // Khởi tạo document PDF
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;
            document.Open();

            // Sử dụng font Unicode hỗ trợ tiếng Việt
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(baseFont, 12, Font.NORMAL);

            // Thêm nội dung vào PDF sử dụng font Unicode
            document.Add(new Paragraph("Chi Tiết Đơn Hàng", font));
            document.Add(new Paragraph($"Mã Đơn Hàng: {donHang.soDH}", font));
            document.Add(new Paragraph($"Tên Khách Hàng: {khachHang.tenKH}", font));
            document.Add(new Paragraph($"Ngày Đặt: {donHang.ngayDat}", font));
            document.Add(new Paragraph(" ", font));

            // Tạo bảng và thêm cột tiêu đề
            PdfPTable table = new PdfPTable(7);
            table.AddCell(new PdfPCell(new Phrase("Hình ảnh", font)));
            table.AddCell(new PdfPCell(new Phrase("Số đơn hàng", font)));
            table.AddCell(new PdfPCell(new Phrase("Mã sản phẩm", font)));
            table.AddCell(new PdfPCell(new Phrase("Số lượng", font)));
            table.AddCell(new PdfPCell(new Phrase("Giá bán", font)));
            table.AddCell(new PdfPCell(new Phrase("Giảm giá", font)));
            table.AddCell(new PdfPCell(new Phrase("Thành tiền", font)));

            decimal tongTien = 0;

            foreach (var item in chiTiet) {
                table.AddCell(new PdfPCell(new Phrase(item.maSP.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(item.soDH.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(item.maSP.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(item.soLuong.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(string.Format("{0:#,##0 VNĐ}", item.giaBan), font)));
                table.AddCell(new PdfPCell(new Phrase(string.Format("{0:#,##0 VNĐ}", item.giamGia), font)));
                table.AddCell(new PdfPCell(new Phrase(string.Format("{0:#,##0 VNĐ}", item.ThanhTien), font)));

                tongTien += item.ThanhTien ?? 0;
            }

            // Thêm dòng trống trước tổng tiền
            PdfPCell emptyCell = new PdfPCell(new Phrase("", font));
            emptyCell.Colspan = 6;
            emptyCell.Border = 0;
            table.AddCell(emptyCell);

            // Thêm dòng tổng tiền vào bảng
            PdfPCell totalCell = new PdfPCell(new Phrase(string.Format("Tổng tiền: {0:#,##0 VNĐ}", tongTien), font));
            totalCell.Colspan = 1;
            totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(totalCell);

            document.Add(table);
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            // Trả về file PDF
            return File(workStream, "application/pdf", "ChiTietDonHang.pdf");
        }


    }
}