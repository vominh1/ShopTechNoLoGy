using System;
using System.Linq;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class ArticleContentController : Controller
    {
        // GET: ArticleContent/Index/{maBV}
        public ActionResult Index(string maBV)
        {
            using (BanBanhOnline db = new BanBanhOnline()) {
                // Lấy bài viết từ cơ sở dữ liệu
                baiViet x = db.baiViets.FirstOrDefault(z => z.maBV == maBV);

                if (x == null) {
                    return HttpNotFound();
                }

                // Tăng số lần đọc và lưu vào cơ sở dữ liệu
                x.solandoc++;
                db.SaveChanges();

                // Pass dữ liệu bài viết tới view
                ViewData["Baicanxem"] = x;
            }

            return View();
        }
    }
}
