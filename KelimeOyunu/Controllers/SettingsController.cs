using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using System.Web.Mvc;

namespace KelimeOyunu.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult Ayar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ayar(int SoruSayi)
        {
            DapperHelper helper = new DapperHelper();
            string sql = "update KullaniciSoruIliski set TopSoruSayisi=@SoruSayi where KullaniciID="+UserInfo.KullaniciID;
            helper.Execute(sql, new { SoruSayi=SoruSayi });
            return View();
        }
    }
}