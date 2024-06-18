using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using System.Data.Entity;

namespace ShopTechNoLoGy.Controllers
{
    public class CheckOutController : Controller
    {
        // GET: CheckOut
        [HttpGet]
        public ActionResult Index()
        {
            //-- tạo đối tượng khách hàng với thông tin truyền vào cho views
            khachHang x = new khachHang();
            //-- lấy thông tin đã mua từ session và truyền vào cho view thông qua view data
            CartShop1 gh = Session["GioHang"] as CartShop1;
            ViewData["Cart"] = gh;
            return View(x); 
        }
        [HttpPost]
        public ActionResult SaveToDataDase(khachHang x)
        {
            // ứng dụng transaction  lưu đồng thời dữ liệu trên 3 thư mục khác nhau 
            using (var context = new BanBanhOnline()) {
                using (DbContextTransaction trans = context.Database.BeginTransaction()) {
                    try {

                        x.maKH = GenerateRandomCustomerCode(context);
                        context.khachHangs.Add(x);
                        //

                        context.SaveChanges();
                        donHang d = new donHang();
                        d.soDH = String.Format("{0:yyMMddhhmm}", DateTime.Now);
                        d.maKH = x.maKH;
                        d.daKichHoat = false;
                        d.ngayDat = DateTime.Now;
                        d.ngayGH = DateTime.Now.AddDays(2);
                        d.taiKhoan = "admin";
                        d.diaChiGH = x.diaChi;
                        d.ghiChu = x.ghiChu;
                     

                        //add order infor to data model 
                        context.donHangs.Add(d);
                        //
                         context.SaveChanges();
                        //
                        CartShop1 gh = Session["GioHang"] as CartShop1;
                        foreach (ctDonHang i in gh.SanPhamDC.Values) {
                            i.soDH = d.soDH;
                            context.ctDonHangs.Add(i);
                        }
                        //save to database ----------------ctdonhang
                        context.SaveChanges();
                        trans.Commit();
                        // Cập nhật thông tin khách hàng vào giỏ hàng trước khi chuyển hướng
                        gh.MaKH = x.maKH;
                        gh.TaiKhoan = "admin"; // cập nhật từ thông tin tài khoản hiện tại
                       
                        gh.DiaChi = x.diaChi;
                        gh.TenKH = x.tenKH;
                        gh.SoDT = x.soDT;
                        gh.Email = x.email;
                        gh.soDH = d.soDH;
                        Session["GioHang"] =gh;
                        

                        //chuyển đến trang thông báo đã đặt hàng thành công 
                        return RedirectToAction("Index", "CheckOutSuccess", new { soDH = d.soDH });
                    }
                    catch (Exception e) {
                        trans.Rollback();
                        string errorMessage = $"Error occurred: {e.Message}";
                        Console.WriteLine(errorMessage); // Hoặc có thể lưu vào log để dễ dàng theo dõi
                        throw; // Ném lại exception để hiển thị thông báo lỗi ở phía client (nếu cần thiết)
                    }

                }
            }
            // nếu bị lỗi sẽ chuyển trở về checkout
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
