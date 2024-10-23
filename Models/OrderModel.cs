using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTechNoLoGy.Models
{
    public class OrderModel
    {
    }
    public class OrderDetailsModel
    {
        public donHang Order { get; set; }
        public List<ctDonHang> OrderDetails { get; set; }
        public string soDH { get; set; }

        public DateTime NgayDat { get; set; }
        public List<OrderItemDetailsModel> OrderItems { get; set; }
        public string TenKH { get; set; }
        public string DiaChi { get; set; }
        public string SoDt { get; set; }
        public string Email { get; set; }
        public decimal TongThanhTien { get; set; }
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