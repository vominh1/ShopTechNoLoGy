using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models; // Ensure you include the right namespace for your models

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class InventoryController : Controller
    {
        private readonly BanBanhOnline db; // Use a non-static instance

        public InventoryController()
        {
            db = new BanBanhOnline(); // Initialize in the constructor
        }

        // GET: PrivatePages/Inventory
        public ActionResult Index()
        {
            var inventoryItems = db.TonKhoes.Include("sanPham").ToList(); // Fetch inventory items
            return View(inventoryItems);
        }

        // Dispose method to release resources
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose(); // Dispose of the DbContext properly
            }
            base.Dispose(disposing);
        }
    }
}
