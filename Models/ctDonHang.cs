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
    
    public partial class ctDonHang
    {
        public string soDH { get; set; }
        public string maSP { get; set; }
        public Nullable<int> soLuong { get; set; }
        public Nullable<long> giaBan { get; set; }
        public Nullable<long> giamGia { get; set; }
    
        public virtual sanPham sanPham { get; set; }
        public virtual donHang donHang { get; set; }
    }
}
