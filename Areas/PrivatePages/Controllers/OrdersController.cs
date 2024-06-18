using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        private void HienThiDonHangChuaXuLy()
        {
            List<donHang> l = db.donHangs.Where(x => x.daKichHoat == false).ToList<donHang>();
            ViewData["DanhSachDonHang1"] = l;
        }
    }
}