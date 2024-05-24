using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace KelimeOyunu.Controllers
{
    public class QuizController : Controller
    {

        private static List<SoruModel> models = new List<SoruModel>();
        public static int SoruCount;
        public static int DogruCount;


        public void SoruGetir()
        {
            DapperHelper helper = new DapperHelper();

            string saydirmasql = "select COUNT(*) from SoruSayaci where SorulacakTarih=(select cast(Getdate() as date)) ";
            string sql = @"select top(select TopSoruSayisi from KullaniciSoruIliski where KullaniciID=@KullaniciID) 
                                S.CumleOrnegi,S.SoruCumlesi,S.Secenek1,S.Secenek2,S.Secenek3,S.Secenek4,s.KelimeID,F.FotografPath
                                from Kelimeler k
                                left join Sorular s on k.KelimeID=s.KelimeID
                                left join SoruSayaci ss on k.KelimeID=ss.KelimeID
                                left join Kullanicilar Ku on ss.KullaniciID=ku.KullaniciID
								left join Fotograf F ON (K.FotografID=F.FotografID)
                                where ss.SorulacakTarih IS NULL and ss.KullaniciID=@KullaniciID";
            models = helper.Query<SoruModel>(sql, new { KullaniciID = UserInfo.KullaniciID }).ToList();

            int soruTarihSorgu = helper.ExecuteScalar<int>(saydirmasql);

            if (soruTarihSorgu > 0)
            {
                string sql2 = @"select S.CumleOrnegi,S.SoruCumlesi,S.Secenek1,S.Secenek2,S.Secenek3,S.Secenek4,s.KelimeID,F.FotografPath
                                from Kelimeler k
                                left join Sorular s on k.KelimeID=s.KelimeID
                                left join SoruSayaci ss on k.KelimeID=ss.KelimeID
                                left join Kullanicilar Ku on ss.KullaniciID=ku.KullaniciID
								left join Fotograf F ON (K.FotografID=F.FotografID)
                                where SorulacakTarih<GETDATE() and Ku.KullaniciID=@KullaniciID";

                var bilinenSoru = helper.Query<SoruModel>(sql2, new { KullaniciID = UserInfo.KullaniciID });
                foreach (SoruModel model in bilinenSoru)
                {
                    models.Add(model);
                }
            }
        }

        public ActionResult Quiz(int? KelimeID, string Cevap)

        {
            if (models.Count != 0)
            {
                SoruKontrol(KelimeID, Cevap);
                SoruCount++;
                if (SoruCount == models.Count || SoruCount > models.Count)
                {

                    ViewBag.rapor = $"Doğru Sayısı: {DogruCount} Yanlış Sayısı {models.Count - DogruCount}";

                    return View();
                }
                return View(models[SoruCount]);
            }
            else
            {
                SoruGetir();
                return View(models[0]);
            }

        }


        public void SoruKontrol(int? KelimeID, string Cevap)
        {
            if (KelimeID > 0)
            {
                DapperHelper helper = new DapperHelper();

                if (Cevap == "False")
                {
                    string yanlisGuncelle = @"Update SoruSayaci set SoruBilmeSayaci=0 , SorulacakTarih=null 
                                        where KelimeID=@KelimeID and KullaniciID=@KullaniciID";

                    helper.Execute(yanlisGuncelle, new { KelimeID = KelimeID, KullaniciID = UserInfo.KullaniciID });
                }
                else
                {
                    DogruCount++;
                    string bilmeSayaciGetir = "select SoruBilmeSayaci from SoruSayaci where KelimeID=@KelimeID and KullaniciID=@KullaniciID";
                    int Sayac = helper.ExecuteScalar<int>(bilmeSayaciGetir, new { KelimeID = KelimeID, KullaniciID = UserInfo.KullaniciID });
                    string dogruGuncelle;
                    switch (Sayac)
                    {
                        case 0:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 1,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(day, 1, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        case 1:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 2,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(week, 1, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        case 2:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 3,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(month, 1, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        case 3:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 4,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(month, 3, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        case 4:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 5,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(month, 6, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        case 5:
                            dogruGuncelle = @"update SoruSayaci set SoruBilmeSayaci = 6,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(year, 1, GETDATE()))
                                           where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                        default:
                            dogruGuncelle = "update SoruSayaci set SoruBilmeSayaci = 6,SoruBilmeTarihi=cast (Getdate() as date),SorulacakTarih = (select dateadd(year, 1, GETDATE())) where KullaniciID = @KullaniciID and  KelimeID=@KelimeID";
                            break;
                    }
                    helper.Execute(dogruGuncelle, new { KelimeID, UserInfo.KullaniciID });


                }
            }


        }
    }
}