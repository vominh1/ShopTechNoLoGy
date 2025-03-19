using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Google.Apis.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ShopTechNoLoGy.Models;

namespace ShopTechNoLoGy.Controllers
{
    public class LoginController : Controller
    {
      
        private Uri RedirectUri {
            get {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FaceBookCallBack");
                return uriBuilder.Uri;
            }
        }
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Acc, string Pass, string returnUrl)
        {
            var context = new BanBanhOnline();
            taiKhoanTV ttdn = context.taiKhoanTVs.FirstOrDefault(x => x.taiKhoan.Equals(Acc.ToLower().Trim()) && x.matKhau.Equals(Pass));
            bool isAuthentic = ttdn != null && ttdn.taiKhoan.Equals(Acc.ToLower().Trim()) && ttdn.matKhau.Equals(Pass);
            if (isAuthentic) {
                Session["ttDangNhap"] = ttdn;

                if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl)) {
                    return Redirect(returnUrl);
                }
                else {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại.";
       
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Đăng nhập với facebook
        public ActionResult LoginFaceBook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email,public_profile"
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        // Xử lý Phản hồi từ Facebook 
        public ActionResult FaceBookCallBack(string code)
        {
            if (string.IsNullOrEmpty(code)) {
                return RedirectToAction("Index", "Login");
            }

            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            // Lấy access_token từ phản hồi của Facebook
            var accessToken = result.access_token;
            
            // Kiểm tra nếu lấy được access_token
            if (!string.IsNullOrEmpty(accessToken)) {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=id,name,email");
                string email = me.email;
                string name = me.name;
                
                // tạo đối tượng và lưu vào csdl
                var context = new BanBanhOnline();
                var user = context.taiKhoanTVs.FirstOrDefault(x => x.email == email);

                if (user == null) {
                    // Tạo tài khoản mới nếu chưa có
                    user = new taiKhoanTV {
                        taiKhoan = email,
                        matKhau = "", // Bạn có thể đặt mật khẩu mặc định hoặc để trống
                        email = email ,
                        tenTV = name ,
                        trangThai = true ,
                        ghiChu= "FaceBook"
                    };
                    context.taiKhoanTVs.Add(user);
                    context.SaveChanges();
                }

                // Lưu vào session
                Session["ttDangNhap"] = user;
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Login");
        }
        // Đăng nhập với Google
        public ActionResult LoginGoogle()
        {
            // Sử dụng middleware OWIN để xác thực với Google
            var properties = new AuthenticationProperties {
                RedirectUri = Url.Action("GoogleCallback", "Login")
            };

            // Bắt đầu quá trình xác thực với Google
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
         
            return new HttpUnauthorizedResult();
        }

        // Xử lý callback từ Google

        public async Task<ActionResult> GoogleCallback()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (authResult == null || authResult.Identity == null) {
                TempData["ErrorMessage"] = "Google login failed. No user info received.";
                return RedirectToAction("Index", "Login");
            }

            var identity = authResult.Identity;
            string email = identity.FindFirstValue(ClaimTypes.Email);
            string name = identity.FindFirstValue(ClaimTypes.Name);
            string givenname = identity.FindFirstValue(ClaimTypes.GivenName);
            if (string.IsNullOrEmpty(email)) {
                TempData["ErrorMessage"] = "Không thể lấy email từ tài khoản Google.";
                return RedirectToAction("Index", "Login");
            }

            using (var context = new BanBanhOnline()) {
                var user = context.taiKhoanTVs.FirstOrDefault(x => x.email == email);

                if (user == null) {
                    user = new taiKhoanTV {
                        taiKhoan = givenname,
                        matKhau = "",
                        email = email,
                        tenTV = name,
                        trangThai = true,
                        ghiChu = "Google"
                    };
                    context.taiKhoanTVs.Add(user); 
                    context.SaveChanges();
                    

                }

                Session["ttDangNhap"] = user;
                return RedirectToAction("Index", "Home");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
          
            Session["ttDangNhap"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
     
    }
}
