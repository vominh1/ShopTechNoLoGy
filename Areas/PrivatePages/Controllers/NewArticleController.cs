using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopTechNoLoGy.Models;
using ShopTechNoLoGy.Areas.PrivatePages.Models;
using System.IO;

namespace ShopTechNoLoGy.Areas.PrivatePages.Controllers
{
    public class NewArticleController : Controller
    {
        // GET: PrivatePages/NewArticle
        [HttpGet]
        public ActionResult Index()
        {
            baiViet x = new baiViet();
            // thiết lập 1 số thông tin mặc định gần gán cho đối tượng bài viết
            x.ngayDang = DateTime.Now;
      
            x.taiKhoan = ThuongDung.getTentaiKhoan();
            
            // ------------ đưa đường dẫn hình ra ngoài 
            // lưu hình bên ngoài vào thư mục dữ liệu --->Images
            ViewBag.ddhinh = "/Dulieu/Images/image_upload_1.jpg";
            return View(x);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(baiViet x, HttpPostedFileBase exampleInputFile )
        {
            ///b1:sử lí thông tin nhận về từ view
            x.maBV = string.Format("{0:yyMMddhhmm}",DateTime.Now);
            x.daDuyet = true;
            x.ngayDang = DateTime.Now;
            x.taiKhoan = ThuongDung.getTentaiKhoan();
       
            
            x.maLoai = 1;
            //------------lưu hình vào thư mục chứa bài viết 
            if (exampleInputFile != null) {
                string virpath = "/Dulieu/Images";

                string phypath = Server.MapPath("~/" + virpath);//xác định vị trí lưu hình sao khi upload
                string ext = Path.GetExtension(exampleInputFile.FileName);
                string fileName = "HDD" + x.maBV + ext;
                exampleInputFile.SaveAs(phypath + fileName);//lưu dựa trên đường dẫn vật lý 
                                                            //ghi nhận đường dẫn truy cập tới hình đã lưu dựa vào domain
                x.hinhDD = virpath + fileName;// đường dẫn ảo theo domain

                ViewBag.ddhinh = x.hinhDD;
            }
            else
                x.hinhDD = "";


            ///b2: cập nhật đối tượng bài viết vừa đăng vào models
            BanBanhOnline db = new BanBanhOnline();
            db.baiViets.Add(x);
            ///b3:lưu thông tin xuống database
            db.SaveChanges();
            return View(x);
        }
    }
}