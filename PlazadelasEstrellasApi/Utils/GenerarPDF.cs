using System;
using System.IO;
using System.Text.RegularExpressions;
using com.sf.ws.Timbrado;
using PlazadelasEstrellasApi.Models;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;


namespace PlazadelasEstrellasApi.Utils
{
   public class GenerarPDF
   {
      static double LetterWidth = XUnit.FromCentimeter(21.5).Point;
      static double LetterHeight = XUnit.FromCentimeter(27.9).Point;
      static DateTime fechaemision = DateTime.UtcNow.AddHours(-6);

      public static Document CreateDocument( Comprobante iDoc, CFDIResultadoCertificacion Resultado, Factura mFactura, int? foliointerno, Cliente mCliente )
      {
         Document doc = new Document();
         doc.Info.Title = "Factura Electronica";
         doc.Info.Subject = "Factura de Ticket en CFDIv4.0";
         doc.Info.Author = "Galerias Plaza de las Estrellas";

         Section section = doc.AddSection();
         section.PageSetup.TopMargin = XUnit.FromCentimeter(0.6).Point;
         section.PageSetup.BottomMargin = XUnit.FromCentimeter(0.6).Point;
         section.PageSetup.LeftMargin = XUnit.FromCentimeter(0.7).Point;
         section.PageSetup.RightMargin = XUnit.FromCentimeter(0.7).Point;
         section.PageSetup.PageWidth = LetterWidth;
         section.PageSetup.PageHeight = LetterHeight;

         DefineEstilo(doc);
         DefineCabecera(section, mFactura, Resultado);
         DefineCliente(section, mCliente);
         DefineFactura(section, iDoc);
         DefineImpuestos(section, iDoc);
         DefineRetenciones(section, iDoc);
         DefineInfoSat(section, Resultado, iDoc);

         DocumentRenderer docRenderer = new DocumentRenderer(doc);
         docRenderer.PrepareDocument();

         XRect LetterRect = new XRect(0, 0, LetterWidth, LetterHeight);

         int pageCount = docRenderer.FormattedDocument.PageCount;

         return doc;
      }

      public static void DefineEstilo( Document document )
      {
         Style style = document.Styles["Normal"];
         style.Font.Name = "DejaVu Sans Condensed";
         style.Font.Italic = false;
         style.Font.Size = 9;
         style.ParagraphFormat.SpaceAfter = 2;

         style = document.Styles["Heading1"];
         style.Font.Italic = false;
         style.Font.Size = 10;
         style.Font.Bold = true;
         style.Font.Color = Colors.DarkBlue;
         style.ParagraphFormat.PageBreakBefore = true;
         style.ParagraphFormat.SpaceAfter = 2;

         style = document.Styles["Heading2"];
         style.Font.Italic = false;
         style.Font.Size = 9;
         style.Font.Bold = true;
         style.Font.Color = Colors.Black;
         style.ParagraphFormat.PageBreakBefore = false;
         style.ParagraphFormat.SpaceAfter = 2;

         style = document.Styles["Heading3"];
         style.Font.Italic = false;
         style.Font.Size = 8;
         style.Font.Bold = true;
         style.Font.Color = Colors.Black;
         style.ParagraphFormat.PageBreakBefore = false;
         style.ParagraphFormat.SpaceAfter = 2;

      }

        public static void DefineCabecera(Section section, Factura mFactura, CFDIResultadoCertificacion Resultado)
        {
            Table table = new Table();
            Row row = new Row();
            Cell cell = new Cell();

            table.Borders.Width = 0;
            table.AddColumn(Unit.FromCentimeter(3.5));
            table.AddColumn(Unit.FromCentimeter(8.5));
            table.AddColumn(Unit.FromCentimeter(8.3));

            row = table.AddRow();
        /* 
         //Add Image
         cell = row.Cells[0];
         if ( ImageSource.ImageSourceImpl == null )
         ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
         Image image = cell.AddImage(ImageSource.FromFile(Path.Combine(Environment.CurrentDirectory, @"Data/Logo_Plaza_de_las_Estrellas.jpg")));
         image.Width = "2.5cm";
           */
         //Add Emisor Info
         cell = row.Cells[1];
         cell.AddParagraph("PROMOCION URBANA");//AddParagraph("PROMOCION URBANA", "Heading2");
            cell.AddParagraph("R.F.C. :"); //AddParagraph("R.F.C. :", "Heading2");
         cell.AddParagraph("PUR790718MG1");
         cell.AddParagraph("DOMICILIO FISCAL:");//AddParagraph("DOMICILIO FISCAL:", "Heading2");
            cell.AddParagraph("Av. Melchor Ocampo 193, INT. Local J56A, " +
            "Col. Veronica Anzures, Del. Miguel Hidalgo, Ciudad de México, México, C.P. 11300");
         cell.AddParagraph("REGIMEN FISCAL:");//.AddParagraph("REGIMEN FISCAL:", "Heading2");
            cell.AddParagraph("601 - General de Ley Personas Morales");

         //Add Factura Info
         Table tableFactura = new Table();
         tableFactura.TopPadding = 2;
         tableFactura.Borders.Width = 0.5;
         tableFactura.Borders.Color = Colors.LightGray;
         tableFactura.AddColumn(Unit.FromCentimeter(8.1));
         Row rowFactura = tableFactura.AddRow();
         rowFactura.Shading.Color = Colors.LightGray;
         cell = rowFactura.Cells[0];
         cell.AddParagraph("FACTURAS").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("FACTURAS", "Heading1").Format.Alignment = ParagraphAlignment.Center;
            rowFactura = tableFactura.AddRow();
         cell = rowFactura.Cells[0];
         cell.AddParagraph("SERIE: ")//.AddParagraph("SERIE: ", "Heading3")
            .AddFormattedText("N. FOLIO: " + mFactura.Ticket.ToString(), "Normal").Size = 7;
         cell.AddParagraph("FOLIO FISCAL: ")//AddParagraph("FOLIO FISCAL: ", "Heading3")
            .AddFormattedText(Resultado.uuid, "Normal").Size = 7;
         cell.AddParagraph("FECHA Y HORA DE CERTIFICACIÓN: ")//AddParagraph("FECHA Y HORA DE CERTIFICACIÓN: ", "Heading3")
            .AddFormattedText(Resultado.fechaTimbrado.Value.AddHours(-6).ToString("dd/MM/yyyy hh:mm:ss tt") , "Normal").Size = 7;
         cell.AddParagraph("FECHA Y HORA DE EMISIÓN: ")//AddParagraph("FECHA Y HORA DE EMISIÓN: ", "Heading3")
            .AddFormattedText(Resultado.fechaTimbrado.Value.AddHours(-6).AddSeconds(5).ToString("dd/MM/yyyy hh:mm:ss tt"), "Normal").Size = 7;
         cell.AddParagraph("METODO DE PAGO: ")//AddParagraph("METODO DE PAGO: ", "Heading3")
            .AddFormattedText("01-Efectivo", "Normal").Size = 7;
         cell.AddParagraph("NUMERO DE CUENTA DE PAGO: ")//AddParagraph("NUMERO DE CUENTA DE PAGO: ", "Heading3")
            .AddFormattedText("N/A", "Normal").Size = 7;
         cell.AddParagraph("TIPO DE COMPROBANTE: ")//AddParagraph("TIPO DE COMPROBANTE: ", "Heading3")
            .AddFormattedText("Ingreso", "Normal").Size = 7;
         cell.AddParagraph("NÚMERO INTERNO: ")//AddParagraph("NÚMERO INTERNO: ", "Heading3")
            .AddFormattedText(mFactura.Ticket.ToString(), "Normal").Size = 7;
         cell = row.Cells[2];
         cell.Elements.Add(tableFactura);
         section.Add(table);
      }

      public static void DefineCliente( Section section, Cliente mCliente )
      {
         Table table = new Table();
         Row row = new Row();
         Cell cell = new Cell();
         Paragraph para = new Paragraph();

         table.TopPadding = 2;
         table.Borders.Width = 0;
         table.AddColumn(Unit.FromCentimeter(3.5));
         table.AddColumn(Unit.FromCentimeter(16.7));

         row = table.AddRow();
         row.Shading.Color = Colors.LightGray;
         row.Cells[0].MergeRight = 1;
         cell = row.Cells[0];
         cell.AddParagraph("CLIENTE").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("CLIENTE", "Heading1").Format.Alignment = ParagraphAlignment.Center;

            row = table.AddRow();
         cell = row.Cells[0];
         cell.AddParagraph("NOMBRE:");//AddParagraph("NOMBRE:", "Heading2");
            cell = row.Cells[1];
         cell.AddParagraph(mCliente.Nombre);

         row = table.AddRow();
         cell = row.Cells[0];
         cell.AddParagraph("R.F.C. :");//AddParagraph("R.F.C. :", "Heading2");
            cell = row.Cells[1];
         cell.AddParagraph(mCliente.Rfc);

         row = table.AddRow();
         cell = row.Cells[0];
         cell.AddParagraph("DOMICILIO FISCAL:");//AddParagraph("DOMICILIO FISCAL:", "Heading2");
            cell = row.Cells[1];
         para.AddFormattedText("Calle: ", "Heading2").AddFormattedText(mCliente.Calle + " | ", "Normal");
         para.AddFormattedText("No. Ext: ", "Heading2").AddFormattedText(mCliente.NumExterior + " | ", "Normal");
         para.AddFormattedText("No. Int: ", "Heading2").AddFormattedText(mCliente.NumInterior + " | ", "Normal");
         para.AddFormattedText("Colonia: ", "Heading2").AddFormattedText(mCliente.Colonia + " | ", "Normal");
         para.AddFormattedText("Municipio: ", "Heading2").AddFormattedText(mCliente.Municipio + " | ", "Normal");
         para.AddFormattedText("Estado: ", "Heading2").AddFormattedText(mCliente.Estado + " | ", "Normal");
         para.AddFormattedText("C.P. : ", "Heading2").AddFormattedText(mCliente.CodigoPost, "Normal");
         cell.Add(para);

         row = table.AddRow();
         cell = row.Cells[0];
         cell.AddParagraph("USO CFDI:");//AddParagraph("USO CFDI:", "Heading2");
            cell = row.Cells[1];
         cell.AddParagraph(mCliente.Cfdi);

         table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1, Colors.LightGray);
         section.AddParagraph("");
         section.Add(table);
      }

      public static void DefineFactura( Section section, Comprobante iDoc )
      {
         Table table = new Table();
         Row row = new Row();
         Cell cell = new Cell();
         Paragraph para = new Paragraph();

         table.TopPadding = 2;
         table.Borders.Width = 0.5;
         table.Borders.Color = Colors.LightGray;
         table.AddColumn(Unit.FromCentimeter(2));
         table.AddColumn(Unit.FromCentimeter(3.5));
         table.AddColumn(Unit.FromCentimeter(2));
         table.AddColumn(Unit.FromCentimeter(6));
         table.AddColumn(Unit.FromCentimeter(3.7));
         table.AddColumn(Unit.FromCentimeter(3));

         row = table.AddRow();
         row.Shading.Color = Colors.LightGray;
         cell = row.Cells[0];
         cell.AddParagraph("CANTIDAD").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("CANTIDAD", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[1];
         cell.AddParagraph("UNIDAD DE MEDIDA").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("UNIDAD DE MEDIDA", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[2];
         cell.AddParagraph("CÓDIGO").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("CÓDIGO", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[3];
         cell.AddParagraph("DESCRIPCIÓN").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("DESCRIPCIÓN", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[4];
         cell.AddParagraph("VALOR UNITARIO").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("VALOR UNITARIO", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            cell = row.Cells[5];
         cell.AddParagraph("IMPORTE TOTAL").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("IMPORTE TOTAL", "Heading2").Format.Alignment = ParagraphAlignment.Center;

            row = table.AddRow();
         cell = row.Cells[0];
         cell.AddParagraph("1").Format.Alignment = ParagraphAlignment.Right;
         cell = row.Cells[1];
         cell.AddParagraph("Unidad de servicio");
         cell = row.Cells[2];
         cell.AddParagraph("46161505");
         cell = row.Cells[3];
         cell.AddParagraph("Estacionamiento");

         Table tableTraslados = new Table();
         tableTraslados.TopPadding = 2;
         tableTraslados.Borders.Width = 0.5;
         tableTraslados.Borders.Color = Colors.LightGray;
         tableTraslados.AddColumn(Unit.FromCentimeter(5));
         Row rowTraslados = tableTraslados.AddRow();
         rowTraslados.Shading.Color = Colors.LightGray;
         cell = rowTraslados.Cells[0];
         cell.AddParagraph("TRASLADOS").Format.Alignment = ParagraphAlignment.Center;//AddParagraph("TRASLADOS", "Heading2").Format.Alignment = ParagraphAlignment.Center;
            rowTraslados = tableTraslados.AddRow();
         cell = rowTraslados.Cells[0];
         cell.AddParagraph("IVA 16% " + iDoc.Impuestos.TotalImpuestosTrasladados.ToString("N"));
         cell = row.Cells[3];
         cell.Elements.Add(tableTraslados);

         cell = row.Cells[4];
         cell.AddParagraph(iDoc.Conceptos[0].ValorUnitario.ToString("N"));
         cell = row.Cells[5];
         cell.AddParagraph(iDoc.Conceptos[0].Importe.ToString("N"));

         section.AddParagraph("");
         section.Add(table);
      }

      public static void DefineImpuestos( Section section, Comprobante iDoc )
      {
         Table table = new Table();
         Row row = new Row();
         Cell cell = new Cell();
         Paragraph para = new Paragraph();

         table.TopPadding = 2;
         table.Borders.Width = 0.5;
         table.Borders.Color = Colors.LightGray;
         table.Format.Alignment = ParagraphAlignment.Right;
         table.AddColumn(Unit.FromCentimeter(11));
         table.AddColumn(Unit.FromCentimeter(7));
         table.AddColumn(Unit.FromCentimeter(2.2));

         row = table.AddRow();
         cell = row.Cells[0];
         cell.Format.Borders.Width = 0;
         cell.Format.Borders.Top.Color = Colors.White;
         cell.Format.Borders.Bottom.Color = Colors.White;
         cell.Format.Borders.Left.Color = Colors.White;
         cell = row.Cells[1];
         cell.AddParagraph("SUBTOTAL");//AddParagraph("SUBTOTAL", "Heading2");
            cell.AddParagraph("IMPUESTOS FEDERALES TRASLADADOS");//AddParagraph("IMPUESTOS FEDERALES TRASLADADOS", "Heading2");
            cell = row.Cells[2];
         cell.AddParagraph(iDoc.SubTotal.ToString("N"));
         cell.AddParagraph(iDoc.Impuestos.TotalImpuestosTrasladados.ToString("N"));
         row = table.AddRow();
         row.Shading.Color = Colors.LightGray;
         cell = row.Cells[0];
         cell.Format.Borders.Width = 0;
         cell.Format.Borders.Top.Color = Colors.White;
         cell.Format.Borders.Bottom.Color = Colors.White;
         cell.Format.Borders.Left.Color = Colors.White;
         cell.Shading.Color = Colors.White;
         cell = row.Cells[1];
         cell.AddParagraph("TOTAL");//AddParagraph("TOTAL", "Heading2");
            cell = row.Cells[2];
         cell.AddParagraph(iDoc.Total.ToString("N"));//AddParagraph(iDoc.Total.ToString("N"), "Heading2");

            section.AddParagraph("");
         section.Add(table);
      }

      public static void DefineRetenciones( Section section, Comprobante iDoc )
      {
         Table table = new Table();
         Row row = new Row();
         Cell cell = new Cell();
         Paragraph para = new Paragraph();

         section.AddParagraph(" ");
         section.AddParagraph(Numalet.ToString(( float )iDoc.Total).ToUpper()).Format.Alignment = ParagraphAlignment.Right;
         para.AddFormattedText("MONEDA: ", "Heading2");
         para.AddFormattedText("MXN ", "Normal");
         para.AddFormattedText("TIPO DE CAMBIO:", "Heading2");
         para.AddFormattedText("1.00 ", "Normal");
         para.AddFormattedText("Pago en una sola exhibición");
         para.Format.Alignment = ParagraphAlignment.Right;
         section.Add(para);
         section.AddParagraph(" ");

         table.TopPadding = 2;
         table.Borders.Width = 0.5;
         table.Borders.Color = Colors.LightGray;
         table.Format.Alignment = ParagraphAlignment.Center;
         table.AddColumn(Unit.FromCentimeter(5.05));
         table.AddColumn(Unit.FromCentimeter(5.05));
         table.AddColumn(Unit.FromCentimeter(5.05));
         table.AddColumn(Unit.FromCentimeter(5.05));

         row = table.AddRow();
         row.Shading.Color = Colors.LightGray;
         cell = row.Cells[0];
         cell.AddParagraph("RETENCIONES LOCALES").Format.Alignment = ParagraphAlignment.Center; ;//AddParagraph("RETENCIONES LOCALES", "Heading2").Format.Alignment = ParagraphAlignment.Center; ;
            cell = row.Cells[1];
         cell.AddParagraph("TRASLADOS LOCALES").Format.Alignment = ParagraphAlignment.Center; ;//AddParagraph("TRASLADOS LOCALES", "Heading2").Format.Alignment = ParagraphAlignment.Center; ;
            cell = row.Cells[2];
         cell.AddParagraph("RETENCIONES FEDERALES").Format.Alignment = ParagraphAlignment.Center; ;//AddParagraph("RETENCIONES FEDERALES", "Heading2").Format.Alignment = ParagraphAlignment.Center; ;
            cell = row.Cells[3];
         cell.AddParagraph("TRASLADOS FEDERALES").Format.Alignment = ParagraphAlignment.Center; ;//AddParagraph("TRASLADOS FEDERALES", "Heading2").Format.Alignment = ParagraphAlignment.Center; ;
            row = table.AddRow();
         row.Format.Alignment = ParagraphAlignment.Center;
         cell = row.Cells[3];
         cell.AddParagraph("IVA 16% " + iDoc.Impuestos.TotalImpuestosTrasladados.ToString("N"));

         section.Add(table);
      }

      public static void DefineInfoSat( Section section, CFDIResultadoCertificacion Resultado, Comprobante iDoc )
      {
         Table table = new Table();
         Row row = new Row();
         Cell cell = new Cell();
         Paragraph para = new Paragraph();

         table.TopPadding = 3;
         table.Borders.Width = 0.5;
         table.Borders.Color = Colors.LightGray;
         table.AddColumn(Unit.FromCentimeter(5));
         table.AddColumn(Unit.FromCentimeter(15.2));

         row = table.AddRow();
         cell = row.Cells[0];
            
         if ( ImageSource.ImageSourceImpl == null )
            ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();

         using ( SixLabors.ImageSharp.Image imageQR = SixLabors.ImageSharp.Image.Load<Rgba32>(Resultado.qrCode) )
         {
            using ( MemoryStream stream = new MemoryStream() )
            {
               imageQR.Save(stream, new PngEncoder());
               stream.Position = 0;
            Image image = cell.AddImage(ImageSource.FromStream("QRCode", () => stream, 100));
             image.Width = "4.8cm";
             image.Height = "4.8cm";
            }
         }
         
         cell = row.Cells[1];
         cell.AddParagraph("NÚMERO DE SERIE DEL CERTIFICADO DEL SAT: ")//AddParagraph("NÚMERO DE SERIE DEL CERTIFICADO DEL SAT: ", "Heading3")
            .AddFormattedText(Resultado.certificadoSAT, "Normal");
         cell.AddParagraph("NÚMERO DE SERIE DEL CSD DEL EMISOR: ");//AddParagraph("NÚMERO DE SERIE DEL CSD DEL EMISOR: ", "Heading3");
            cell.AddParagraph(Regex.Replace(iDoc.Certificado, ".{110}", "$0 ")).Format.Font.Size = 6;//AddParagraph(Regex.Replace(iDoc.Certificado, ".{110}", "$0 "), "Normal").Format.Font.Size = 6;
            cell.AddParagraph("SELLO DIGITAL DEL SAT: ");//AddParagraph("SELLO DIGITAL DEL SAT: ", "Heading3");
            cell.AddParagraph(Regex.Replace(Resultado.selloSAT, ".{110}", "$0 ")).Format.Font.Size = 6;//AddParagraph(Regex.Replace(Resultado.selloSAT, ".{110}", "$0 "), "Normal").Format.Font.Size = 6;
            cell.AddParagraph("SELLO DIGITAL DEL CFDI: ");//AddParagraph("SELLO DIGITAL DEL CFDI: ", "Heading3");
            cell.AddParagraph(Regex.Replace(iDoc.Sello, ".{110}", "$0 ")).Format.Font.Size = 6;//AddParagraph(Regex.Replace(iDoc.Sello, ".{110}", "$0 "), "Normal").Format.Font.Size = 6;
            cell.AddParagraph("CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACIÓN DIGITAL DEL SAT: ");//AddParagraph("CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACIÓN DIGITAL DEL SAT: ", "Heading3");
            cell.AddParagraph(Regex.Replace(Resultado.cadenaOriginal, ".{110}", "$0 ")).Format.Font.Size = 6;//AddParagraph(Regex.Replace(Resultado.cadenaOriginal, ".{110}", "$0 "), "Normal").Format.Font.Size = 6;

            section.AddParagraph("");
         section.Add(table);
      }
   }
}
