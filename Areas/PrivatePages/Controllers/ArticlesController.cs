using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class ArticlesController : Controller
    {
        private static BanBanhOnline db = new BanBanhOnline();

        [HttpGet]
        // GET: PrivatePages/Articles
        public ActionResult Index()
        {
            CapNhatDuLieuChoGiaoDien();
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string maBaiViet)
        {
            baiViet x = db.baiViets.Find(maBaiViet);
            db.baiViets.Remove(x);
            db.SaveChanges();
            CapNhatDuLieuChoGiaoDien();
            return View("Index");
        }
        public ActionResult Active(string maBaiViet)
        {
            baiViet x = db.baiViets.Find(maBaiViet);
            x.daDuyet = false;

            db.SaveChanges();
            CapNhatDuLieuChoGiaoDien();
            return View("Index");
        }
        private void CapNhatDuLieuChoGiaoDien()
        {
            List<baiViet> l = db.baiViets.Where(x => x.daDuyet == true).ToList<baiViet>();
            ViewData["DanhSachBV"] = l;
        }
    }
}