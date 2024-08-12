using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ShopTechNoLoGy.App_Start;
using System.Web.Optimization;
using ShopTechNoLoGy.Models;
using OfficeOpenXml;

namespace ShopTechNoLoGy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleCollection bundles = BundleTable.Bundles;
            BundleCongig.BundleRegister(bundles);
        }
        public void Session_Start(object sender,EventArgs e)
        {
            Session["ttDangNhap"] = null;
            //--- cũng cấp cho người dùng chưa truy cập vào giỏ hàn chưa có gì cả---///
            Session["GioHang"]= new CartShop1();

        }
       

    }
}
