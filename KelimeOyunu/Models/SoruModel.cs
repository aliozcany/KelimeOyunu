using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KelimeOyunu.Models
{
    public class SoruModel
    {
        public int KelimeID { get; set; }
        public string CumleOrnegi { get; set; }
        public string SoruCumlesi { get; set; }
        public string Secenek1 { get; set; }
        public string Secenek2 { get; set; }
        public string Secenek3 { get; set; }
        public string Secenek4 { get; set; }
        public string SecenekDogru { get; set; }
        public byte[] FotografPath { get; set; }
    }
}