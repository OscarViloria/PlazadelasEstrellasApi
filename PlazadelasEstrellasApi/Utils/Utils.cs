using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using com.sf.ws.Timbrado;
using PlazadelasEstrellasApi.Models;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;

namespace PlazadelasEstrellasApi.Utils
{
   public class Utils
   {
      public static void CreaPDF( Comprobante iDoc, CFDIResultadoCertificacion Resultado, Factura mFactura, int? foliointerno, string lDataPath, Cliente mCliente )
      {
         try
         {
            string lRutaPdf = Path.Combine(lDataPath, @"Facturas");
            lRutaPdf = Path.Combine(lRutaPdf, mFactura.Ticket + "-" + mFactura.FechaTicket.Value.ToString("yyy-MM-dd-HH-mm") + ".pdf");

            Document document = new Document();
            document = GenerarPDF.CreateDocument(iDoc, Resultado, mFactura, foliointerno, mCliente);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            pdfRenderer.PdfDocument.Save(lRutaPdf);
         }
         catch ( Exception ex )
         {
           throw ( ex );
         }
      }

      public static string ObtenerRuta()
      {
         var fakeRoot = Environment.CurrentDirectory; // Gives us a cross platform full path
         var combined = Path.Combine(fakeRoot, @"Data");
         return Path.GetFullPath(combined);
      }
   }
}
