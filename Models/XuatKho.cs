//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopTechNoLoGy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class XuatKho
    {
        public int MaPhieuXuat { get; set; }
        public System.DateTime NgayXuat { get; set; }
        public string taiKhoan { get; set; }
        public string maSP { get; set; }
        public Nullable<int> SoLuongXuat { get; set; }
        public string NguoiXuat { get; set; }
        public string LyDoXuat { get; set; }
    
        public virtual sanPham sanPham { get; set; }
        public virtual taiKhoanTV taiKhoanTV { get; set; }
    }
}
