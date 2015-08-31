using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Utility
{
    public class PDFFooter : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            //document = new Document(PageSize.A4, 10f, 10f, 50f, 50f);
            base.OnOpenDocument(writer, document);
            //PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            //tabFot.SpacingAfter = 10F;
            //PdfPCell cell;
            //tabFot.TotalWidth = 300F;
            //cell = new PdfPCell(new Phrase("Home Depot"));
            //tabFot.AddCell(cell);
            //tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            //document = new Document(PageSize.A4, 10f, 10f, 50f, 50f);
            string CompanyName = HttpContext.Current.Session["ClientCompanyName"] != null ? HttpContext.Current.Session["ClientCompanyName"].ToString() : "All Clients";
            string UserName = HttpContext.Current.Session["UserName"] != null ? HttpContext.Current.Session["UserName"].ToString() : "";
            string DateValue = HttpContext.Current.Session["DateValue"] != null ? HttpContext.Current.Session["DateValue"].ToString() : "";
            string ReportHeading = HttpContext.Current.Session["VisitType"] != null ? HttpContext.Current.Session["VisitType"].ToString() : "";

            base.OnStartPage(writer, document);
            PdfPTable tabFot = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Phrase(CompanyName, new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD)));
            tabFot.WidthPercentage = 100;
            tabFot.SpacingBefore = 10f;
            tabFot.SpacingAfter = 10f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 10;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            tabFot.AddCell(cell);

            PdfPTable tabFot1 = new PdfPTable(2);
            PdfPCell cell1 = new PdfPCell(new Phrase(ReportHeading, new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD)));
            tabFot1.WidthPercentage = 100;
            tabFot1.SpacingBefore = 10f;
            tabFot1.SpacingAfter = 10f;
            cell1.Border = 0;
            cell1.Colspan = 2;
            cell1.ExtraParagraphSpace = 10;
            cell1.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            tabFot.AddCell(cell1);

            PdfPTable table = new PdfPTable(2);

            table.WidthPercentage = 100;
            table.SpacingBefore = 5f;
            table.SpacingAfter = 5f;

            PdfPCell cellnew = new PdfPCell(new Phrase("Request By :" + UserName, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
            cellnew.Border = 0;
            cellnew.ExtraParagraphSpace = 10;
            cellnew.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            PdfPCell cell2 = new PdfPCell(new Phrase("Date :" + DateValue, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
            cell2.Border = 0;
            // cell2.ExtraParagraphSpace = 10;
            cell2.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right

            table.AddCell(cellnew);
            table.AddCell(cell2);

            document.Add(tabFot);
            document.Add(table);
            // tabFot.WriteSelectedRows(0, -1, 150, document.PageSize.Height - 10, writer.DirectContent);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            //document = new Document(PageSize.A4, 10f, 10f, 50f, 50f);
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            PdfPCell cell;
            tabFot.TotalWidth = 300F;
            cell = new PdfPCell(new Phrase(document.PageNumber.ToString(), new Font(Font.FontFamily.HELVETICA, 14F)));
            cell.BorderWidth = 0f;
            cell.HorizontalAlignment = 1;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 150, document.Bottom, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            //document = new Document(PageSize.A4, 10f, 10f, 50f, 50f);
            base.OnCloseDocument(writer, document);
        }
    }
}