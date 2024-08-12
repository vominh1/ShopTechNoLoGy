using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class CheckOutController : Controller
    {
        // GET: CheckOut
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["ttDangNhap"] == null) {
                // Redirect to Login if not authenticated
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("Index", "CheckOut") });
            }

            // Create customer object with information for the view
            khachHang x = new khachHang();

            // Get purchase information from session and pass to view via ViewData
            CartShop1 gh = Session["GioHang"] as CartShop1;
            ViewData["Cart"] = gh;
            return View(x);
        }


        [HttpPost]
        public ActionResult SaveToDataDase(khachHang x)
        {
            if (Session["ttDangNhap"] == null) {
                // Redirect to Login if not authenticated
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action("Index", "CheckOut") });
            }

            // Validate input fields
            if (string.IsNullOrWhiteSpace(x.tenKH)) {
                ModelState.AddModelError("tenKH", "Vui lòng nhập họ tên khách hàng.");
            }
            if (string.IsNullOrWhiteSpace(x.soDT)) {
                ModelState.AddModelError("soDT", "Vui lòng nhập số điện thoại.");
            }
            if (string.IsNullOrWhiteSpace(x.email)) {
                ModelState.AddModelError("email", "Vui lòng nhập địa chỉ email.");
            }
            if (string.IsNullOrWhiteSpace(x.diaChi)) {
                ModelState.AddModelError("diaChi", "Vui lòng nhập địa chỉ giao hàng.");
            }

            // Check ModelState for any validation errors
            if (!ModelState.IsValid) {
                // Populate ViewData again and return the view with validation errors
                CartShop1 gh = Session["GioHang"] as CartShop1;
                ViewData["Cart"] = gh;
                return View("Index", x); // Return to the checkout page with validation errors
            }

            using (var context = new BanBanhOnline()) {
                using (DbContextTransaction trans = context.Database.BeginTransaction()) {
                    try {
                        // Tạo mã khách hàng ngẫu nhiên
                        x.maKH = GenerateRandomCustomerCode(context);
                        context.khachHangs.Add(x);
                        context.SaveChanges();

                        taiKhoanTV currentUser = Session["ttDangNhap"] as taiKhoanTV;

                        donHang d = new donHang();
                        d.soDH = String.Format("{0:yyMMddhhmm}", DateTime.Now);
                        d.maKH = x.maKH;
                        d.daKichHoat = false;
                        d.ngayDat = DateTime.Now;
                        d.ngayGH = DateTime.Now.AddDays(2);
                        d.taiKhoan = currentUser?.taiKhoan ?? "guest"; // Use the logged-in user's account or "guest" if null
                        d.diaChiGH = x.diaChi;
                        d.ghiChu = x.ghiChu;

                        context.donHangs.Add(d);
                        context.SaveChanges();

                        CartShop1 gh = Session["GioHang"] as CartShop1;
                        foreach (ctDonHang i in gh.SanPhamDC.Values) {
                            i.soDH = d.soDH;
                            // thành tiền bằng tổng thành tiền của 1 sản phẩm
                            i.ThanhTien = (i.giaBan * i.soLuong - (i.soLuong * (i.giamGia * 1000))) ;
                            context.ctDonHangs.Add(i);
                            // query theo mã sản phẩm
                            var sanPham = context.sanPhams.SingleOrDefault(sp => sp.maSP == i.maSP);
                            if (sanPham != null) {
                                sanPham.SoLuongTonKho -= i.soLuong;
                                if (sanPham.SoLuongTonKho < 0) {
                                    throw new Exception("Không đủ hàng trong kho.");
                                }
                            }
                        }
                        context.SaveChanges();
                        trans.Commit();
                        
                        // lưu tạm dữ liệu vào Session 
                        gh.TaiKhoan = currentUser?.taiKhoan ?? "guest";
                        gh.MaKH = x.maKH;
                        gh.DiaChi = x.diaChi;
                        gh.TenKH = x.tenKH;
                        gh.SoDT = x.soDT;
                        gh.Email = x.email;
                        gh.soDH = d.soDH;
                        gh.ghiChu = x.ghiChu;
                        Session["GioHang"] = gh;

                        return RedirectToAction("Index", "CheckOutSuccess", new { soDH = d.soDH });
                    }
                    catch (Exception e) {
                        trans.Rollback();
                        string errorMessage = $"Error occurred: {e.Message}";
                        Console.WriteLine(errorMessage);
                        throw;
                    }
                }
            }

            return RedirectToAction("Index", "CheckOut");
        }


        private string GenerateRandomCustomerCode(BanBanhOnline context)
        {
            string maKH;
            bool exists;
            do {
                maKH = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                exists = context.khachHangs.Any(kh => kh.maKH == maKH);
            } while (exists);

            return maKH;
        }
    }
}
