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
    public class NewProductController : Controller
    {
   
        // GET: PrivatePages/NewProduct
        [HttpGet]
        public ActionResult Index()
        {
            sanPham z = new sanPham();
            // thiết lập 1 số thông tin mặc định gần gán cho đối tượng bài viết
            z.ngayDang = DateTime.Now;
      
            z.taiKhoan = ThuongDung.getTentaiKhoan();
            // ------------ đưa đường dẫn hình ra ngoài 
            // lưu hình bên ngoài vào thư mục dữ liệu --->Images
            ViewBag.ddhinh = "/Dulieu/Images/image_upload_1.jpg";
            return View(z);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(sanPham z, HttpPostedFileBase hinhdaidien)
        {
            ///b1:sử lí thông tin nhận về từ view
            z.maSP = string.Format("{0:yyMMddhhmm}", DateTime.Now);
            z.daDuyet = true;
            z.ngayDang = DateTime.Now;
            z.taiKhoan = ThuongDung.getTentaiKhoan();
         
            //sản phẩm vào là 1 
            z.maLoai = 1;
            //------------lưu hình vào thư mục chứa bài viết 
            if (hinhdaidien != null) {
                string virpath = "/images/image_product";
            
                string phypath = Server.MapPath("~/" + virpath);//xác định vị trí lưu hình sao khi upload
                string ext = Path.GetExtension(hinhdaidien.FileName);
                string fileName = "HDD" + z.maSP + ext;
                hinhdaidien.SaveAs(phypath + fileName);//lưu dựa trên đường dẫn vật lý 
                                                       //ghi nhận đường dẫn truy cập tới hình đã lưu dựa vào domain
                z.hinhDD = virpath + fileName;// đường dẫn ảo theo domain

                ViewBag.ddhinh = z.hinhDD;
            }
            else
                z.hinhDD = "";


            ///b2: cập nhật đối tượng sản phẩm loại 1 vừa đăng vào models
             BanBanhOnline dbd = new BanBanhOnline();
            dbd.sanPhams.Add(z);
            ///b3:lưu thông tin xuống database
            dbd.SaveChanges();
            return View(z);
        }
    }

}