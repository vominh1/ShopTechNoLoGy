using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTechNoLoGy.Models
{
    public class khachHangThanThietModel
    {
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public int soluong { get; set; }
        public decimal ThanhTien { get; set; }
        public string TaiKhoan { get; set; }

    }
    public class chitietmua
    {
        public string TenSP { get; set; } // Tên sản phẩm
        public int SoLuong { get; set; }  // Số lượng đã mua
        public decimal ThanhTien { get; set; }  // Tổng tiền cho sản phẩm
        public decimal DonGia { get; set; }  // Giá sản phẩm
    }
}