using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class CustomerController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        public ActionResult Index()
        {
            HienThiDonHangDaXuLy();
            return View();
        }

        public ActionResult CustomerList()
        {
            var customers = db.khachHangs
                .Select(kh => new CustomerModel {
                    MaKH = kh.maKH,
                    TenKH = kh.tenKH,
                    email = kh.email,
                    soDT = kh.soDT
                })
                .ToList();

            return View(customers);
        }

        [HttpPost]
        public ActionResult Search(string searchString)
        {
            var customers = db.khachHangs
                .Where(kh => kh.tenKH.Contains(searchString) || kh.maKH.Contains(searchString))
                .Select(kh => new CustomerModel {
                    MaKH = kh.maKH,
                    TenKH = kh.tenKH,
                    email = kh.email,
                    soDT = kh.soDT
                })
                .ToList();

            if (customers.Count == 0) {
                ViewBag.Message = "Không có khách hàng này.";
            }

            return View("CustomerList", customers);
        }
        /// <summary>
        ///  Chi tiết sản phẩm theo khách hàng đã đặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // Hàm kết hợp đa bảng
        // Cần tạo model để lấy dữ liệu lên 
        public ActionResult OrderDetails(string id) 
        {
            var orders = db.donHangs
                .Where(dh => dh.maKH == id)
                .Select(dh => new OrderDetailsModel {
                    soDH = dh.soDH,
                    NgayDat = (DateTime)dh.ngayDat,
                    OrderItems = db.ctDonHangs
                        .Where(ct => ct.soDH == dh.soDH)
                        .Select(ct => new OrderItemDetailsModel {
                            MaSP = ct.maSP,
                            TenSP = ct.sanPham.tenSP,
                            SoLuong = (int)ct.soLuong,
                            GiaBan = (int)ct.giaBan,
                            GiamGia = (decimal)ct.giamGia,
                            ThanhTien=(decimal)ct.ThanhTien
                        })
                        .ToList()
                })
                .ToList();

            return View(orders);
        }


        [HttpPost]
        public ActionResult Delete(string maDonHang)
        {
            var orderDetails = db.ctDonHangs.Where(d => d.soDH == maDonHang).ToList();

            foreach (var detail in orderDetails) {
                db.ctDonHangs.Remove(detail);
            }

            var order = db.donHangs.Find(maDonHang);
            if (order != null) {
                db.donHangs.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Active(string maDonhang)
        {
            var order = db.donHangs.Find(maDonhang);
            if (order != null) {
                order.daKichHoat = false;
                db.SaveChanges();
            }

            HienThiDonHangDaXuLy();
            return View("Index");
        }

        private void HienThiDonHangDaXuLy()
        {
            var processedOrders = db.donHangs.Where(x => x.daKichHoat == true).ToList();
            ViewData["DanhSachDonHang"] = processedOrders;
        }
    }
}
