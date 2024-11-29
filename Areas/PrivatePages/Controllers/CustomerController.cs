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
           
            return View();
        }


        // GET: KhachHang/ThanThiet

        public ActionResult KhachHangThanThiet()
        {   
            // Tiêu chí cho khách hàng thân thiết: Đã mua trên 5 đơn hàng và tổng giá trị đơn hàng trên 5 triệu VND
            var loyalCustomers = (from ctdh in db.ctDonHangs
                                  join dh in db.donHangs on ctdh.soDH equals dh.soDH
                                  join kh in db.khachHangs on dh.maKH equals kh.maKH
                                  join tk in db.taiKhoanTVs on dh.taiKhoan equals tk.taiKhoan
                                  group ctdh by new { kh.maKH, kh.tenKH, tk.taiKhoan } into customerGroup
                                  where customerGroup.Count() > 4 // Số đơn hàng
                                  select new {
                                      customerGroup.Key.maKH,
                                      customerGroup.Key.tenKH,
                                      customerGroup.Key.taiKhoan, // Lấy tài khoản từ nhóm
                                      ThanhTien = customerGroup.Sum(x => x.ThanhTien), // Tính tổng tiền từ chi tiết đơn hàng
                                      soluong = customerGroup.Sum(x => x.soLuong) // Tính tổng số lượng sản phẩm đã mua
                                  }).ToList();

            // Chuyển dữ liệu thành danh sách đối tượng để gửi đến View
            var loyalCustomerList = loyalCustomers.Select(x => new khachHangThanThietModel {
                MaKH = x.maKH,               // Gán mã khách hàng
                TenKH = x.tenKH,             // Gán tên khách hàng
                TaiKhoan = x.taiKhoan,       // Gán tài khoản
                ThanhTien = (decimal)x.ThanhTien, // Gán tổng tiền đã tính cho ThanhTien
                soluong = (int)x.soluong   // Gán tổng số lượng sản phẩm đã mua
            }).ToList();

            return View(loyalCustomerList);
        }

        public ActionResult KhachHangMuaNhieuNhat()
        {
            // Tiêu chí cho tài khoản mua nhiều nhất: Lọc theo số lượng sản phẩm đã mua hoặc tổng tiền
            var topAccounts = (from ctdh in db.ctDonHangs
                               join dh in db.donHangs on ctdh.soDH equals dh.soDH
                               join tk in db.taiKhoanTVs on dh.taiKhoan equals tk.taiKhoan
                               group ctdh by new { tk.taiKhoan, tk.tenTV } into accountGroup
                               select new {
                                   accountGroup.Key.taiKhoan,
                                   accountGroup.Key.tenTV,
                                   TotalAmountSpent = accountGroup.Sum(x => x.ThanhTien), // Tổng tiền đã chi
                                   TotalQuantity = accountGroup.Sum(x => x.soLuong) // Tổng số lượng sản phẩm đã mua
                               })
                               .OrderByDescending(x => x.TotalQuantity) // Sắp xếp theo số lượng sản phẩm mua
                               .ToList();

            // Chuyển dữ liệu thành danh sách đối tượng để gửi đến View
            var topAccountList = topAccounts.Select(x => new khachHangThanThietModel {
                MaKH = x.taiKhoan,             // Mã tài khoản
                TenKH = x.tenTV,               // Tên tài khoản
                ThanhTien = (decimal)x.TotalAmountSpent, // Tổng tiền đã chi
                soluong = (int)x.TotalQuantity   // Tổng số lượng sản phẩm đã mua
            }).ToList();

            return View(topAccountList);  // Trả về danh sách tài khoản mua nhiều nhất
        }

        public ActionResult ChiTietMuaHang(string taiKhoan)
        {
            // Truy vấn các sản phẩm mà tài khoản đã mua
            var purchasedProducts = (from ctdh in db.ctDonHangs
                                     join dh in db.donHangs on ctdh.soDH equals dh.soDH
                                     join sp in db.sanPhams on ctdh.maSP equals sp.maSP
                                     where dh.taiKhoan == taiKhoan  // Lọc theo mã tài khoản
                                     select new {
                                         sp.tenSP,  // Tên sản phẩm
                                         ctdh.soLuong,  // Số lượng sản phẩm đã mua
                                         ctdh.ThanhTien, // Tổng giá trị sản phẩm
                                         sp.giaBan // Giá của sản phẩm
                                     }).ToList();

            // Chuyển đổi dữ liệu thành danh sách các đối tượng để gửi tới view
            var productList = purchasedProducts.Select(x => new chitietmua {
                TenSP = x.tenSP,
                SoLuong = (int)x.soLuong,
                ThanhTien = (decimal)x.ThanhTien,
                DonGia = (decimal)x.giaBan
            }).ToList();

            return View(productList);  // Trả về danh sách các sản phẩm đã mua
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
        /////  Chi tiết sản phẩm theo khách hàng đã đặt hàng
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //// Hàm kết hợp đa bảng
        //// Cần tạo model để lấy dữ liệu lên 
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
                            ThanhTien = (decimal)ct.ThanhTien
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
