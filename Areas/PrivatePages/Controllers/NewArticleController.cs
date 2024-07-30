using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using ShopTechNoLoGy.Areas.PrivatePages.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class NewArticleController : Controller
    {
        // GET: PrivatePages/NewArticle
        [HttpGet]
        public ActionResult Index()
        {
           
            var x = new baiViet {
                ngayDang = DateTime.Now,
                taiKhoan = ThuongDung.getTentaiKhoan(),
                  
            };
            ViewBag.ddhinh = "/Dulieu/Images/image_upload_1.jpg";
            return View(x);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(baiViet x, HttpPostedFileBase exampleInputFile)
        {
            // Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(x.tenBV))
                ModelState.AddModelError("tenBV", "Vui lòng nhập tên bài viết.");
            if (string.IsNullOrWhiteSpace(x.ndTomTat))
                ModelState.AddModelError("ndTomTat", "Vui lòng nhập nội dung tóm tắt.");
            if (string.IsNullOrWhiteSpace(x.noiDung))
                ModelState.AddModelError("noiDung", "Vui lòng nhập nội dung chính.");
            if (exampleInputFile == null || exampleInputFile.ContentLength == 0)
                ModelState.AddModelError("hinhDD", "Vui lòng chọn hình ảnh đại diện.");

            // Nếu không có lỗi, tiến hành lưu bài viết
            if (ModelState.IsValid) {
                try {
                    // Xử lý thông tin bài viết
                    x.maBV = DateTime.Now.ToString("yyMMddHHmm");
                    x.daDuyet = true;
                    x.ngayDang = DateTime.Now;
                    x.taiKhoan = ThuongDung.getTentaiKhoan();
                    x.maLoai = 1;
                    x.solandoc = 1;
                    // Lưu hình vào thư mục chứa bài viết
                    if (exampleInputFile != null && exampleInputFile.ContentLength > 0) {
                        string virpath = "/Dulieu/Images";
                        string phypath = Server.MapPath("~" + virpath);
                        string ext = Path.GetExtension(exampleInputFile.FileName);
                        string fileName = $"HDD{x.maBV}{ext}";
                        string fullPath = Path.Combine(phypath, fileName);

                        // Đảm bảo tên tệp không bị trùng lặp
                        if (System.IO.File.Exists(fullPath)) {
                            System.IO.File.Delete(fullPath);
                        }

                        exampleInputFile.SaveAs(fullPath);
                        x.hinhDD = Path.Combine(virpath, fileName);

                    }

                    // Lưu bài viết vào database
                    using (var db = new BanBanhOnline()) {
               
                        db.baiViets.Add(x);
                        db.SaveChanges();
                    }

                    // Thông báo đăng bài viết thành công bằng TempData
                    TempData["SuccessMessage"] = "Đăng bài viết thành công!";

                    // Redirect về action Index để làm mới trang
                    return RedirectToAction("Index");
                }
                catch (Exception ex) {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu bài viết.");
                    // Log lỗi nếu cần
                    // Logger.Log(ex);
                }
            }

            // Nếu có lỗi, giữ lại hình đã chọn và hiển thị lại form
            ViewBag.ddhinh = x.hinhDD ?? "/Dulieu/Images/image_upload_1.jpg";

            return View(x);
        }
    }
}
