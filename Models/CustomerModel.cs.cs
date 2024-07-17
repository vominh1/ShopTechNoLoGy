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
    public class OrderDetailsModel
    {
        public donHang Order { get; set; }
        public List<ctDonHang> OrderDetails { get; set; }
        public string soDH { get; set; }
       
        public DateTime NgayDat { get; set; }
        public List<OrderItemDetailsModel> OrderItems { get; set; }
    }
    public class OrderItemDetailsModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public int GiaBan { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

}