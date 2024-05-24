﻿using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;


namespace KelimeOyunu.Controllers
{


    public class QuizController : Controller
    {

        private List<SoruModel> models = new List<SoruModel>();


        int sormamgerekensoru = 10;
        public ActionResult Quiz()
        {


            DapperHelper helper = new DapperHelper();

            string saydirmasql = "select COUNT(*) from SoruSayaci where SorulacakTarih=(select cast(Getdate() as date)) ";
            int soruTarihSorgu = helper.ExecuteScalar<int>(saydirmasql);


            


            string deneme = null;

            if (soruTarihSorgu > 0)
            {

                
                string sql2 = @"select S.CumleOrnegi,S.SoruCumlesi,S.Secenek1,S.Secenek2,S.Secenek3,S.Secenek4 from Kelimeler k 
                                left join SoruSayaci ss on k.KelimeID=ss.KelimeID
                                left join Sorular s on k.KelimeID=s.KelimeID
                                left join Kullanicilar Ku on ss.KullaniciID=ku.KullaniciID
                                where SorulacakTarih<GETDATE() and Ku.KullaniciID=@KullaniciID";

                string sql = @"select top(select TopSoruSayisi from KullaniciSoruIliski where KullaniciID=@KullaniciID) 
                                S.CumleOrnegi,S.SoruCumlesi,S.Secenek1,S.Secenek2,S.Secenek3,S.Secenek4 
                                from Kelimeler k
                                left join Sorular s on k.KelimeID=s.KelimeID
                                left join SoruSayaci ss on k.KelimeID=ss.KelimeID
                                left join Kullanicilar Ku on ss.KullaniciID=ku.KullaniciID
                                where ss.SorulacakTarih IS NULL and ss.KullaniciID=@KullaniciID ";

                var normalSoru = helper.Query<SoruModel>(sql,new {KullaniciID=UserInfo.KullaniciID}).ToList();
                var bilinenSoru = helper.Query<SoruModel>(sql2, new {KullaniciID=UserInfo.KullaniciID});
                foreach (SoruModel model in bilinenSoru)
                {
                    normalSoru.Add(model);
                }
                return View(normalSoru);


            }




            return View(helper.Query<SoruModel>(sql));
        }
        public void SoruKontrol(int soruid, int cevapid)
        {
            //check answer
            //update answercount

        }
    }
}