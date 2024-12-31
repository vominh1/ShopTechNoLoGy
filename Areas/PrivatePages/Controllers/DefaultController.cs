using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class DefaultController : Controller
    {
        // GET: PrivatePages/Default
        public ActionResult Index()
        {
            return View();
        }

        // GET: PrivatePages/Default/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PrivatePages/Default/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrivatePages/Default/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PrivatePages/Default/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PrivatePages/Default/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PrivatePages/Default/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PrivatePages/Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
