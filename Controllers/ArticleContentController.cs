using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
namespace ShopTechNoLoGy.Controllers
{
    public class ArticleContentController : Controller
    {
        // GET: ArticleContent
        public ActionResult Index(string maBV)
        {
            BanBanhOnline db = new BanBanhOnline();
            baiViet x = db.baiViets.Where(z => z.maBV == maBV).First<baiViet>();
            ViewData["Baicanxem"] = x;
            return View();
            
        }
    }
}