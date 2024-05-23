using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace KelimeOyunu.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            DapperHelper helper = new DapperHelper();
            string sql = "select * from Kullanicilar where KullaniciMail=@username and KullaniciSifre=@password";
            var data = helper.QueryFirstOrDefault<KullaniciModel>(sql, new { username = email, password = password });

            if (data != null)
            {
                UserInfo.KullaniciID = data.KullaniciID;
                UserInfo.KullaniciAd = data.KullaniciAd;
                UserInfo.KullaniciSoyad = data.KullaniciSoyad;
                UserInfo.KullaniciMail = data.KullaniciMail;
                UserInfo.KullaniciSifre = data.KullaniciSifre;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.message = "Kullanıcı Adı veya Şifre Yanlış";
                return View();
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string ad, string soyad, string mail, string sifre)
        {
            DapperHelper helper = new DapperHelper();
            string sql = "insert into Kullanicilar(KullaniciAd,KullaniciSoyad,KullaniciMail,KullaniciSifre) values (@ad,@soyad,@mail,@sifre)";
            helper.Execute(sql, new { ad = ad, soyad = soyad, mail = mail, sifre = sifre });
            return RedirectToAction("Login", "Account");
        }

        public ActionResult SifremiUnuttum()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SifremiUnuttum(string mail)
        {
            DapperHelper helper = new DapperHelper();

            string sql = @"declare @sifre nvarchar(50)
                           set @sifre=(select KullaniciSifre from Kullanicilar where @mail=KullaniciMail)
                           declare @mesaj nvarchar(100)
                           set @mesaj= 'Şifreniz: ' + @sifre
                           EXEC msdb.dbo.sp_send_dbmail
                           @profile_name = 'Ali Özcan',
                           @recipients = @mail,
                           @subject = 'Şifremi Unuttum',
                           @body = @mesaj;";


            helper.Execute(sql, new {mail=mail });

            return RedirectToAction("Login", "Account");
        }



        public ActionResult Logout()
        {
            UserInfo.KullaniciID = 0;
            UserInfo.KullaniciAd = null;
            UserInfo.KullaniciSoyad = null;
            UserInfo.KullaniciMail = null;
            UserInfo.KullaniciSifre = null;

            return RedirectToAction("Login", "Account");

        }
    }
}