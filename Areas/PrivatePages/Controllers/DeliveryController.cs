using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using System.Data.Entity;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class DeliveryController : Controller
    {
        private BanBanhOnline db = new BanBanhOnline();

        // GET: PrivatePages/Delivery
        public ActionResult Index()
        {
            var ordersForDelivery = db.donHangs
                .Where(order=> order.daKichHoat == true)
                .Include(o => o.khachHang)
                .ToList();

            return View(ordersForDelivery);
        }

        // New action to show shipper selection
        public ActionResult SelectShipper(string maDonhang)
        {
            var donHang = db.donHangs.Find(maDonhang);
            if (donHang == null) {
                return HttpNotFound();
            }

            // Get a list of available shippers
            var shippers = db.Shippers.Where(s => s.trangThai == true).ToList();
            ViewBag.OrderId = maDonhang; // Pass the order ID to the view

            return View(shippers);
        }

        [HttpPost]
        public ActionResult AssignShipper(string maDonhang, string maShipper)
        {
            var donHang = db.donHangs.Find(maDonhang);
            if (donHang == null) {
                return HttpNotFound();
            }

            // Assign the selected shipper
            donHang.maShipper = maShipper;
            donHang.trangThaiGiaoHang = "Đang giao"; // Change status to 'In Progress'
            donHang.ngayGH = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus(string soDH, string newStatus)
        {
            var order = db.donHangs.Find(soDH);
            if (order == null) {
                return HttpNotFound();
            }

            order.trangThaiGiaoHang = newStatus; // e.g., "Delivered" or "Canceled"
            order.ngayGH = DateTime.Now; // If marking as delivered
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
