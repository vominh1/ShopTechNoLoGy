using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Areas.PrivatePages.Models;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class OrdersController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        // GET: PrivatePages/Products
        public ActionResult Index()
        {
            HienThiDonHangChuaXuLy();
            return View();
        }

        public ActionResult dhDaXuly()
        {
            HienThiDonHangDaXuLy();
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string maDonHang)
        {
            var orderDetails = db.ctDonHangs.Where(d => d.soDH == maDonHang).ToList();

            // Xóa tất cả các bản ghi con trước khi xóa bản ghi cha
            foreach (var detail in orderDetails) {
                db.ctDonHangs.Remove(detail);
            }

            // Tiến hành xóa bản ghi cha sau khi đã xóa tất cả các bản ghi con
            var order = db.donHangs.Find(maDonHang);
            if (order != null) {
                db.donHangs.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        public ActionResult Active(string maDonhang)
        {
            donHang x = db.donHangs.Find(maDonhang);
            x.daKichHoat = true;

            db.SaveChanges();
            HienThiDonHangChuaXuLy();
           
            return View("Index");
        }
        public ActionResult OrderDetails(string maDonHang)
        {
            var order = db.donHangs.Find(maDonHang);
            if (order == null) {
                return HttpNotFound();
            }
            var orderDetails = db.ctDonHangs
                .Where(ct => ct.soDH == maDonHang)
                .Select(ct => new OrderItemDetailsModel {
                    MaSP = ct.maSP,
                    TenSP = ct.sanPham.tenSP,
                    SoLuong = (int)ct.soLuong,
                    GiaBan = (int)ct.giaBan,
                    GiamGia = (decimal)ct.giamGia,
                    ThanhTien = (decimal)ct.ThanhTien
                })
                .ToList();
            decimal tongThanhTien = orderDetails.Sum(item => item.ThanhTien);

            // Lấy thông tin khách hàng từ đơn hàng
            var khachHang = db.khachHangs.FirstOrDefault(kh => kh.maKH == order.maKH);
            var model = new OrderDetailsModel {
                soDH = order.soDH,
                NgayDat = (DateTime)order.ngayDat,
                OrderItems = orderDetails,
                TenKH = khachHang.tenKH,
                DiaChi = khachHang.diaChi,
                SoDt = khachHang.soDT,
                Email = khachHang.email,
                TongThanhTien = tongThanhTien // Thêm tổng thành tiền vào model
            };

            return View(model);
        }

        public ActionResult OrderDetailsT(string maDonHang)
        {
            // Tìm đơn hàng theo mã
            var order = db.donHangs.Find(maDonHang);
            if (order == null) {
                return HttpNotFound();
            }

            // Lấy chi tiết đơn hàng
            var orderDetails = db.ctDonHangs
                .Where(ct => ct.soDH == maDonHang)
                .Select(ct => new OrderItemDetailsModel {
                    MaSP = ct.maSP,
                    TenSP = ct.sanPham.tenSP,
                    SoLuong = (int)ct.soLuong,
                    GiaBan = (int)ct.giaBan,
                    GiamGia = (decimal)ct.giamGia,
                    ThanhTien = (decimal)ct.ThanhTien
                })
                .ToList();

            // Tính tổng thành tiền
            decimal tongThanhTien = orderDetails.Sum(item => item.ThanhTien);

            // Lấy thông tin khách hàng từ đơn hàng
            var khachHang = db.khachHangs.FirstOrDefault(kh => kh.maKH == order.maKH);

            // Tạo mô hình để truyền vào view
            var model = new OrderDetailsModel {
                soDH = order.soDH,
                NgayDat = (DateTime)order.ngayDat,
                OrderItems = orderDetails,
                TenKH = khachHang?.tenKH, // Nếu có thông tin khách hàng
                DiaChi = khachHang?.diaChi,
                SoDt = khachHang?.soDT,
                Email = khachHang?.email,
                TongThanhTien = tongThanhTien // Thêm tổng thành tiền vào model
            };

            return View(model);
        }

        private void HienThiDonHangChuaXuLy()
        {
            List<donHang> l = db.donHangs.Where(x => x.daKichHoat == false ).OrderBy(x => x.ngayDat).ToList<donHang>();
            ViewData["DanhSachDonHang1"] = l;
        }
        /// <summary>
        /// Hàm tìm kiếm đơn hàng thông qua số đơn hàng
        /// </summary>
        /// <param name="soDonHang"></param>
        /// <returns></returns>
        public ActionResult SearchOrder()
        {
            return View(); // Trả về view tìm kiếm
        }
        [HttpPost]
        public ActionResult SearchOrder(string soDonHang)
        {
            // Tìm kiếm đơn hàng dựa trên số đơn hàng
            var order = db.donHangs.FirstOrDefault(o => o.soDH == soDonHang);

            if (order == null) {
                ViewData["Message"] = "Không tìm thấy đơn hàng với số đơn hàng này.";
                return View(); // Trả về view tìm kiếm với thông báo lỗi
            }
           
            // Lấy chi tiết đơn hàng
            var orderDetails = db.ctDonHangs
                .Where(ct => ct.soDH == soDonHang)
                .Select(ct => new OrderItemDetailsModel {
                    MaSP = ct.maSP,
                    TenSP = ct.sanPham.tenSP,
                    SoLuong = (int)ct.soLuong,
                    GiaBan = (int)ct.giaBan,
                    GiamGia = (decimal)ct.giamGia,
                    ThanhTien = (decimal)ct.ThanhTien,
                   
                })
                .ToList();

            // Tính tổng thành tiền
            decimal tongThanhTien = orderDetails.Sum(item => item.ThanhTien); // Sử dụng Sum trên danh sách

            // Lấy thông tin khách hàng từ đơn hàng
            var khachHang = db.khachHangs.FirstOrDefault(kh => kh.maKH == order.maKH);
           
            var model = new OrderDetailsModel {
                soDH = order.soDH,
                NgayDat = (DateTime)order.ngayDat,
                OrderItems = orderDetails,
                // Thông tin khách hàng
                TenKH = khachHang.tenKH,
                DiaChi = khachHang.diaChi,
                SoDt = khachHang.soDT,
                Email = khachHang.email,
                TongThanhTien = tongThanhTien // Thêm tổng thành tiền vào model
            };

            return View(model); // Trả về view với model đã tìm thấy
        }

       
        [HttpPost]
        private void HienThiDonHangDaXuLy()
        {
            List<donHang> l = db.donHangs.Where(x => x.daKichHoat == true).OrderBy(x => x.ngayDat).ToList<donHang>();
            ViewData["DanhSachDonHang"] = l;
        }
    }
}