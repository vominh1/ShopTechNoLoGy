using System;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(ShopTechNoLoGy.Startup))]

namespace ShopTechNoLoGy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.UseKentorOwinCookieSaver();// bắt buộc phải có dòng này nếu không có GoogleCallback() is null 
            // Cấu hình xác thực bằng Cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/Index"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = true
            });

            // Dòng này rất quan trọng để giữ thông tin người dùng khi xác thực từ Google
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Cấu hình Google Authentication
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions {
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"],
                CallbackPath = new PathString("/signin-google"),
                Scope = { "email", "profile", "openid" }
            });
        }


    }
}
