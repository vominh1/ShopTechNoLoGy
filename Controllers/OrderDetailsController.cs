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

            // Thêm nội dung vào PDF
            document.Add(new Paragraph("Chi Tiết Đơn Hàng"));
            document.Add(new Paragraph($"Mã Đơn Hàng: {donHang.soDH}"));
            document.Add(new Paragraph($"Tên Khách Hàng: {khachHang.tenKH}"));
            document.Add(new Paragraph($"Ngày Đặt: {donHang.ngayDat}"));
            document.Add(new Paragraph(" "));

            PdfPTable table = new PdfPTable(7);
            table.AddCell("Hình ảnh");
            table.AddCell("Số đơn hàng");
            table.AddCell("Mã sản phẩm");
            table.AddCell("Số lượng");
            table.AddCell("Giá bán");
            table.AddCell("Giảm giá");
            table.AddCell("Thành tiền");

            foreach (var item in chiTiet) {
                table.AddCell(item.maSP.ToString());
                table.AddCell(item.soDH.ToString());
                table.AddCell(item.maSP.ToString());
                table.AddCell(item.soLuong.ToString());
                table.AddCell(string.Format("{0:#,##0 VNĐ}", item.giaBan));
                table.AddCell(string.Format("{0:#,##0 VNĐ}", item.giamGia));
                table.AddCell(string.Format("{0:#,##0 VNĐ}", item.ThanhTien));
            }

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