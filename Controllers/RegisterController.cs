using System;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class RegisterController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }

            try {
                using (var db = new BanBanhOnline()) {
                    // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa
                    var existingEmail = db.taiKhoanTVs.FirstOrDefault(x => x.email == model.Email);
                    if (existingEmail != null) {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }

                    // Kiểm tra xem số điện thoại đã tồn tại trong cơ sở dữ liệu chưa
                    var existingPhone = db.taiKhoanTVs.FirstOrDefault(x => x.soDT == model.SoDT);
                    if (existingPhone != null) {
                        ModelState.AddModelError("SoDT", "Phone number already exists.");
                        return View(model);
                    }

                    // Tạo mới một đối tượng tài khoản và lưu vào cơ sở dữ liệu
                    var newAccount = new taiKhoanTV {
                        taiKhoan = model.TaiKhoan,
                        matKhau = model.MatKhau, // Mật khẩu đã được mã hóa ở trên view
                        hoDem = model.HoDem,
                        tenTV = model.TenTV,
                        ngaysinh = model.NgaySinh,
                        gioiTinh = model.GioiTinh,
                        soDT = model.SoDT,
                        email = model.Email,
                        diaChi = model.DiaChi,
                        trangThai = true // Trạng thái tài khoản mới là true
                    };

                    // Lưu đối tượng tài khoản vào cơ sở dữ liệu
                    db.taiKhoanTVs.Add(newAccount);
                    db.SaveChanges();

                    // Gọi hàm JavaScript để hiển thị thông báo và chuyển hướng
                    return Content("<script>alert('Chúc mừng bạn đã tạo tài khoản thành công!');window.location.href='/Login/Index';</script>");
                }
            }
            catch (Exception ex) {
                // Xử lý lỗi nếu có
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View(model);
            }
        }

    }
}
