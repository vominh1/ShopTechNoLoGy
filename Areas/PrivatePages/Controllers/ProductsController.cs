using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class ProductsController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        // GET: PrivatePages/Products
        public ActionResult Index()
        {
            HienThiSanPhamchoGiaodien();
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string maSanPham)
        {
            sanPham x = db.sanPhams.Find(maSanPham);
            db.sanPhams.Remove(x);
            db.SaveChanges();
            HienThiSanPhamchoGiaodien();
            return View("Index");
        }
        public ActionResult Active(string maSanPham)
        {
            sanPham x = db.sanPhams.Find(maSanPham);
            x.daDuyet = false;

            db.SaveChanges();
            HienThiSanPhamchoGiaodien();
            return View("Index");
        }

        private void HienThiSanPhamchoGiaodien()
        {
            List<sanPham> l = db.sanPhams.Where(x => x.daDuyet == true).ToList<sanPham>();
            ViewData["DanhSachSP"] = l;
        }
    }
}