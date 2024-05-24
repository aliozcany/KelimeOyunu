using KelimeOyunu.Helper;
using KelimeOyunu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aspose.Pdf;
using Aspose.Pdf.Drawing;

namespace KelimeOyunu.Controllers
{
    public class GrafikModel
    {
        public string CategoryName { get; set; }
        public int CategoryCount { get; set; }
    }
    public class ReportsController : Controller
    {
        public ActionResult Rapor()

        {
            return View();
        }

        public ActionResult CategoryChart()
        {
            return Json(BlogList(), JsonRequestBehavior.AllowGet);
        }

        public List<GrafikModel> BlogList()
        {
           DapperHelper helper = new DapperHelper();
            return helper.Query<GrafikModel>("select Count(k.KelimeID)as'CategoryCount',ko.KonuAd as 'CategoryName'\r\nfrom Kelimeler k \r\nleft join Konu ko on k.KonuID=ko.KonuID\r\nleft join SoruSayaci ss on k.KelimeID=ss.KelimeID\r\nleft join Kullanicilar ku on ss.KullaniciID=ku.KullaniciID\r\nwhere ss.SoruBilmeSayaci=6 and ku.KullaniciID=@KullaniciID \r\ngroup by ko.KonuAd", 
                new { UserInfo.KullaniciID }).ToList();
        }
        [HttpGet]
        public void DownloadPdf()
        {

            Document pdfDocument = new Document();

            Page pdfPage = pdfDocument.Pages.Add();

            Graph graph = new Graph(400, 400);

            Aspose.Pdf.Drawing.Rectangle rect = new Aspose.Pdf.Drawing.Rectangle(100, 350, 300, 300);
            graph.Shapes.Add(rect);

            Aspose.Pdf.Table table = new Aspose.Pdf.Table();
            table.ColumnWidths = "100 100";
            table.Border = new BorderInfo(BorderSide.All, 1F);
            table.DefaultCellBorder = new BorderInfo(BorderSide.All, 0.5F);
            Row row = table.Rows.Add();
            row.Cells.Add("Kategori");
            row.Cells.Add("Bilinen Soru Sayısı");
            string[] categories = { "Aletler ve Makineler", "İş ve Eğitim", "Yiyecekler ve İçecekler" };
            int[] values = { 1, 1, 2 };

            for (int i = 0; i < categories.Length; i++)
            {
                row = table.Rows.Add();
                row.Cells.Add(categories[i]);
                row.Cells.Add(values[i].ToString());
            }
            pdfPage.Paragraphs.Add(table);

            float xPosition = 100;
            for (int i = 0; i < values.Length; i++)
            {
                float barHeight = values[i] * 10;
                Aspose.Pdf.Drawing.Rectangle bar = new Aspose.Pdf.Drawing.Rectangle(xPosition, 350 - barHeight, 50, barHeight);
                bar.GraphInfo.FillColor = Aspose.Pdf.Color.Blue;
                graph.Shapes.Add(bar);
                xPosition += 70;
            }

            pdfPage.Paragraphs.Add(graph);

            using (var memoryStream = new System.IO.MemoryStream())
            {
                pdfDocument.Save(memoryStream);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=output.pdf");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }

        }

    }
}