using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using System.Web.Mvc;

namespace KelimeOyunu.Controllers
{
    public class AccountController : Controller
    {
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
                return RedirectToAction("KelimeEkleme", "Word");
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

            string sql2 = "insert into KullaniciSoruIliski(KullaniciID) values((select KullaniciID from Kullanicilar where KullaniciMail=@mail))";
            helper.Execute(sql2, new { mail = mail });

            string kelimeSayisi = "select count(*) from Kelimeler";
            int kelimeSayisiTutucu = helper.ExecuteScalar<int>(kelimeSayisi);

            for (int i = 1; i <= kelimeSayisiTutucu; i++)
            {

                string insertintoToplu = @"insert into SoruSayaci(SoruBilmeSayaci,KelimeID,KullaniciID) values(0," + i +
                                         ",(select KullaniciID from Kullanicilar where KullaniciMail=@mail))";
                helper.Execute(insertintoToplu, new { mail = mail });

            }
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


            helper.Execute(sql, new { mail = mail });

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