using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Areas.PrivatePages.Models
{
    public class ThuongDung
    {
        /// <summary>
        /// phương thức cho phép đọc thông tin của tài khoản đã đăng nhập trong session
        /// </summary>
        /// <returns></returns>
        public static taiKhoanTV GetTaiKhoanHH()
        {
            taiKhoanTV kq = new taiKhoanTV();
            kq = (taiKhoanTV)HttpContext.Current.Session["ttDangNhap"];
            return kq;
        }

        /// <summary>
        /// lấy tên tài khoản đã đăng nhập trong hệ thống
        /// </summary>
        /// <returns></returns>
        public static string getTentaiKhoan()
        {
            return GetTaiKhoanHH().taiKhoan;
        }
       
    }
}