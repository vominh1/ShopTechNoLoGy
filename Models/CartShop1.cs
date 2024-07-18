using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTechNoLoGy.Models
{
    public class CartShop1
    {
       
        public string MaKH { get; set; }
        public string TaiKhoan { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayGiao { get; set; }
        public string DiaChi { get; set; }
        public bool gioiTinh { get; set; }
        public string TenKH { get; set; }
        public string SoDT { get; set; }
        public string Email { get; set; }
        public string soDH { get; set; }
        public string ghiChu { get; set; }
        public string taiKhoan { get; set; }
        public int SoLuongTonKho { get;set; }
        public SortedList<string, ctDonHang> SanPhamDC { get; set; }
        /// <summary>
        /// sử dụng contractor để khởi tạo giá trị ban đầu
        /// </summary>
        public CartShop1()
        {
            this.MaKH = "";
            this.TaiKhoan = "";
            this.soDH = "";
            this.NgayDat = DateTime.Now; 
            this.NgayGiao = DateTime.Now.AddDays(2);
            this.DiaChi = "";
           
            
            
            this.SanPhamDC = new SortedList<string, ctDonHang>();
        }
        public bool IsEmpty()
        {
            return SanPhamDC.Keys.Count == 0;
        }
     

        /// <summary>
        ///   thêm 1 sản phẩm trong giỏ hàng 
        /// </summary>
        /// <param name="maSP"></param>
        public void addItem(string maSP)
        {
            if (SanPhamDC.Keys.Contains(maSP)) {
                
                ctDonHang x = SanPhamDC.Values[SanPhamDC.IndexOfKey(maSP)];
              
                x.soLuong++;
                updateOneItem(x);
            }
            else {
                ctDonHang i = new ctDonHang();
                i.maSP = maSP;
                i.soLuong = 1;
                sanPham z = Common.getProductById(maSP);
                i.giaBan = z.giaBan;
                i.giamGia = z.giamGia;
                SanPhamDC.Add(maSP, i);
            }
        }
        /// <summary>
        /// cập nhât sản phẩm
        /// </summary>
        /// <param name="x"></param>
        public void updateOneItem(ctDonHang x)
        {
            this.SanPhamDC.Remove(x.maSP);
            this.SanPhamDC.Add(x.maSP, x);
        }
        /// <summary>
        /// xóa 1 sản phẩm
        /// </summary>
        /// <param name="maSP"></param>
        public void deleteItem(string maSP)
        {
            if (SanPhamDC.Keys.Contains(maSP)) {
                SanPhamDC.Remove(maSP);
            }

        }
        public void decrease(string maSP)
        {
            ctDonHang x = SanPhamDC.Values[SanPhamDC.IndexOfKey(maSP)];
            if (x.soLuong > 1) {
                x.soLuong--;
                updateOneItem(x);
            }
            else {
                deleteItem(maSP);
            }
        }
        /// <summary>
        /// tính tổng tiền cho một  giỏ hàng
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public long moneyOfOneProduct(ctDonHang x)
        {
            return (long)(x.giaBan * x.soLuong - ( x.soLuong * (x.giamGia*1000)));
        }
        /// <summary>
        /// Hiễn thị số lượng thông qua chi tiết đơn hàng 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int quatilyofoneproduct (ctDonHang x)
        {
            return (int)x.soLuong;
        }
        /// <summary>
        /// tính tổng tiền cho toàn bộ giỏ hàng
        /// </summary>
        /// <returns></returns>
        public long totalOfCartShop()
        {
            long kq = 0;
            foreach (ctDonHang i in SanPhamDC.Values)
                kq += moneyOfOneProduct(i);
            return kq;
        }
        /// <summary>
        /// viết thêm một hàm cập nhật đựa vào việc xóa item củ đi 
        /// </summary>
        /// <param name="x"></param>

    }
}