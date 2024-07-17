using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class InforCustomerController : Controller
    {
        private readonly BanBanhOnline _context;

        public InforCustomerController()
        {
            _context = new BanBanhOnline();
        }

        // GET: InforCustomer
        public ActionResult Index()
        {
            var loggedInUser = Session["ttDangNhap"] as taiKhoanTV;
            if (loggedInUser == null) {
                return RedirectToAction("Index", "Login");
            }

            return View(loggedInUser);
        }

        // POST: InforCustomer/Update
        [HttpPost]
        public ActionResult Update(taiKhoanTV model)
        {
            if (Session["ttDangNhap"] == null) {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid) {
                using (DbContextTransaction trans = _context.Database.BeginTransaction()) {
                    try {
                        var existingCustomer = _context.taiKhoanTVs.FirstOrDefault(x => x.taiKhoan == model.taiKhoan);

                        if (existingCustomer != null) {
                            existingCustomer.hoDem = model.hoDem;
                            existingCustomer.tenTV = model.tenTV;
                            existingCustomer.gioiTinh = model.gioiTinh;
                            existingCustomer.email = model.email;
                            existingCustomer.soDT = model.soDT;
                            existingCustomer.diaChi = model.diaChi;
                            existingCustomer.trangThai = model.trangThai;

                            _context.SaveChanges();
                            trans.Commit();

                            Session["ttDangNhap"] = existingCustomer;

                            return RedirectToAction("Index");
                        }
                        else {
                            ModelState.AddModelError("", "Không tìm thấy thông tin khách hàng.");
                        }
                    }
                    catch (Exception e) {
                        trans.Rollback();
                        string errorMessage = $"Error occurred: {e.Message}";
                        Console.WriteLine(errorMessage);
                        ModelState.AddModelError("", errorMessage);
                    }
                }
            }

            return View("Index", model);
        }
    }
}
