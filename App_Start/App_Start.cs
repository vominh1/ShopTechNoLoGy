using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ShopTechNoLoGy.App_Start
{
    public class BundleCongig
    {
        public static void BundleRegister(BundleCollection bundle)
        {
            bundle.Add(new StyleBundle("~/bundles/css1").Include("~/Content/bootstrap.min.css",
              "~/Content/font-awesome.min.css",
             "~/Content/prettyPhoto.css",
             "~/Content/price-range.css",
              "~/Content/animate.css",
              "~/Content/main.css",
              "~/Content/responsive.css"));
            bundle.Add(new StyleBundle("~/bundles/css2").Include("~/Content/all.min.css",
              "~/Content/adminlte.min.css",
              "~/Content/summernote-bs4.css",
                "~/Content/dataTables.bootstrap4.css"));
            //--- Add script to the bundle for Public pages
            bundle.Add(new ScriptBundle("~/bundle/moder").Include("~/Scripts/modernizr-2.6.2.js"));
            bundle.Add(new ScriptBundle("~/bundle/scripts1").Include("~/Scripts/jquery-1.10.2.min.js",
             "~/Scripts/bootstrap.min.js",
             "~/Scripts/jquery.scrollup.min.js",
             "~/Scripts/price-range.js",
              "~/Scripts/jquery.prettyPhoto.js",
              "~/Scripts/main.js"));
            bundle.Add(new ScriptBundle("~/bundle/scripts2").Include("~/Scripts/jquery.min.js",
"~/Scripts/bootstrap.bundle.min.js", "~/Scripts/adminlte.min.js",
"~/Scripts/demo.js",
"~/Scripts/jquery.dataTables.js",
"~/Scripts/dataTables.bootstrap4.js"));


        }
    }
}
