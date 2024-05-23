using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KelimeOyunu.Controllers
{
    public class WordController : Controller
    {
        public ActionResult KelimeEkleme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KelimeEkle(string KelimeTr, string KelimeEng, int KelimeKonu, HttpPostedFileBase Dosya)
        {
            DapperHelper helper = new DapperHelper();
            int dosyaid = 0;

            if (Dosya != null && Dosya.ContentLength > 0)
            {
                byte[] dosyaDonusturucu;
                using (BinaryReader reader = new BinaryReader(Dosya.InputStream))
                {
                    dosyaDonusturucu = reader.ReadBytes(Dosya.ContentLength);
                }

                string sqldosya = "insert into Fotograf (FotografPath) output inserted.FotografID values (@FotografPath)";
                dosyaid = helper.ExecuteScalar<int>(sqldosya, new
                {
                    FotografPath = dosyaDonusturucu
                });

            }


            string sql = "insert Kelimeler(KelimeTr,KelimeEng,KonuID,FotografID) values (@KelimeTR,@KelimeEng,@KonuID,@FotografID)";
            helper.Execute(sql, new
            {
                KelimeTr = KelimeTr,
                KelimeEng = KelimeEng,
                KonuID = KelimeKonu,
                FotografID = dosyaid
            });

            return RedirectToAction("KelimeEkleme");
        }

        public ActionResult CumleEkleme()
        {
            DapperHelper helper = new DapperHelper();
            string sql = "select * from Kelimeler";


            return View(helper.Query<KelimeModel>(sql));
        }

        [HttpPost]
        public ActionResult CumleEkleme(int KelimeID,string CumleOrnegi,string secenek1, string secenek2, string secenek3)
        {
            DapperHelper helper = new DapperHelper();
            string sql = @"insert into Sorular(CumleOrnegi,SoruCumlesi,Secenek1,Secenek2,Secenek3,SecenekDogru,KelimeID) values(@CumleOrnegi,
                            'What does '+ (select LOWER(KelimeENG) from Kelimeler Where KelimeID=@KelimeID)+' mean?'
                         ,@Secenek1,@Secenek2,@Secenek3,(select KelimeEng from Kelimeler where KelimeID=@KelimeID),@KelimeID)";

            helper.Execute(sql, new { CumleOrnegi = CumleOrnegi, KelimeID = KelimeID,  secenek1 = secenek1, secenek2 = secenek2, secenek3 = secenek3 });

            return RedirectToAction("CumleEkleme");
        }
    }
}