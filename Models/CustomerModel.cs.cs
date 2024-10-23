using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTechNoLoGy.Models
{
    // CustomerModel.cs
    public class CustomerModel
    {
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string DiaChi { get; set; }
        public string soDT { get; set; }
        public string email { get; set; }

        public string taiKhoan { get; set; }
        public string matKhau { get; set; }
        public DateTime ngayTao { get; set; }

        // Thêm các thuộc tính khác nếu cần thiết
    }
  

}