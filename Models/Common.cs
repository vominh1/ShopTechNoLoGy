using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace ShopTechNoLoGy.Models
{
    public class Common
    {
        static DbContext cn = new DbContext("name=BanBanhOnline");
        public static List<sanPham>GetProducts()
        {
            List<sanPham> l = new List<sanPham>();
            DbContext cn = new DbContext("name=BanBanhOnline");
            l = cn.Set<sanPham>().ToList<sanPham>();
            return l;
        }
        public static List<sanPham> getProductByLoaiSP(int maLoai)
        {
            List<sanPham> l = new List<sanPham>();
            DbContext cn = new DbContext("name=BanBanhOnline");
            l = cn.Set<sanPham>().Where(x=>x.maLoai==maLoai).ToList<sanPham>();
            return l;
        }
        public static List<loaiSP> getCategories()
        {
            return new DbContext("name=BanBanhOnline").Set<loaiSP>().ToList<loaiSP>();
        }
        public  static List<baiViet>getArticles(int n)
        {
            List<baiViet> l = new List<baiViet>();
            BanBanhOnline db = new BanBanhOnline();
            l = db.baiViets.OrderByDescending(bv => bv.ngayDang).Take(n).ToList<baiViet>();
            return l;
        }
        public static sanPham getProductById(string maSP)
        {
            return cn.Set<sanPham>().Find(maSP);
        }
        /// <summary>
        /// lấy tên của product
        /// </summary>
        /// <param name="maSP"></param>
        /// <returns></returns>
        //public static string getNameOfProductById (string maSP)
        //{
        //    return cn.Set<sanPham>().Find(maSP).tenSP;
        //}
        public static string getNameOfProductById(string maSP)
        {
            using (var context = new DbContext("name=BanBanhOnline")) {
                return context.Set<sanPham>().Find(maSP).tenSP;
            }
        }

        /// <summary>
        /// lấy hình ảnh của images
        /// </summary>
        /// <param name="maSP"></param>
        /// <returns></returns>
        public static string getImagesOfProductById(string maSP)
        {
            return cn.Set<sanPham>().Find(maSP).hinhDD;
        }
        public static string getImagesOfDonHangByID(string soDH)
        {
            return cn.Set<donHang>().Find(soDH).soDH;
        }

    }
}