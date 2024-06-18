using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ShopTechNoLoGy.Controllers
{
    public class ArticlesListController : Controller
    {
        // GET: ArticlesList
        public ActionResult Index()
        {
            return View();
        }
    }
}