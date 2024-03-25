using PlazadelasEstrellasApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlazadelasEstrellasApi.Models
{
   public class GenerarFactura
   {
      private readonly PlazaEstrellasDBContext _context;

      public GenerarFactura( PlazaEstrellasDBContext context )
      {
         _context = context;
      }

      public async Task<int> ActualizarEmpresa(Empresa mEmpresa)
      {
         _context.Entry( mEmpresa ).State = EntityState.Modified;

         try
         {
            return await _context.SaveChangesAsync();
         }
         catch ( DbUpdateConcurrencyException )
         {
            throw;
         }
      }

      public Comprobante GenerarComprobante(Cliente pCliente, decimal lSubtotal, Ticket lTicket, Usocfdi lUsoCfdi, RegimenFiscal lRegimenFiscal, DateTime lFechaCst)
      {
         var caracter = pCliente.Rfc.Trim().Substring(3, 1);
         #region Inicializa ComprobanteConcepto
         var lConceptos = new List<ComprobanteConcepto>()
            {
               new ComprobanteConcepto
               {
                  ClaveProdServ = "78181703",
                  NoIdentificacion = "78181703",
                  Cantidad = 1,
                  ClaveUnidad = "E48",
                  Unidad = "Unidad de servicio",
                  Descripcion = "Estacionamiento",
                  ValorUnitario = decimal.Round(lSubtotal, 2),
                  Importe = decimal.Round(lSubtotal, 2),
                  Descuento = 0,
                  ObjetoImp = "02",
                  DescuentoSpecified = false,
                  Impuestos = new ComprobanteConceptoImpuestos
                  {
                     Traslados = new ComprobanteConceptoImpuestosTraslado[1]
                     {
                        new ComprobanteConceptoImpuestosTraslado
                        {
                           Base = decimal.Round(lSubtotal, 2),
                           Impuesto = c_Impuesto.Item002,
                           TipoFactor = c_TipoFactor.Tasa,
                           TasaOCuota = (decimal) 0.16,
                           Importe = decimal.Round(lSubtotal*(decimal)0.16, 2),
                           TasaOCuotaSpecified = true,
                           ImporteSpecified = true,
                        }
                     }
                  }
               }
            };
         #endregion

         //TODO: Revisar el funcionamiento de esta linea.
         var SubTox = lConceptos.Sum(concepto => concepto.Importe);

         #region Datos impuestos
         var IvaDox = lConceptos.Sum(concepto => concepto.Impuestos.Traslados.Sum(iva => iva.Importe));
         var Traslado = new ComprobanteImpuestosTraslado
         {
            Base = decimal.Round(lSubtotal, 2),
            Impuesto = c_Impuesto.Item002,
            TipoFactor = c_TipoFactor.Tasa,
            TasaOCuota = ( decimal )0.16,
            Importe = decimal.Round(IvaDox, 2),
            TasaOCuotaSpecified = true,
            ImporteSpecified = true
         };

         var TotalDoX = IvaDox + SubTox;
         #endregion

         #region Datos generales
         Comprobante iDoc = new Comprobante
         {
            Version = "4.0",
            Serie = "F",
            Folio = ""+lTicket.Folio,
            Fecha = lFechaCst.ToString("yyyy-MM-dd'T'HH:mm:ss"),
            Sello = string.Empty,
            FormaPago = "01",
            NoCertificado = string.Empty,
            Certificado = string.Empty,
            CondicionesDePago = "Contado",
            SubTotal = SubTox,
            Descuento = 0,
            Moneda = c_Moneda.MXN,
            TipoCambio = 1,
            Total = decimal.Round(TotalDoX, 2),
            TipoDeComprobante = c_TipoDeComprobante.I,
            Exportacion = "01",
            MetodoPago = c_MetodoPago.PUE,
            LugarExpedicion = "11300",
            DescuentoSpecified = false,
            FormaPagoSpecified = true,
            TipoCambioSpecified = true,
            MetodoPagoSpecified = true,
            Emisor = new ComprobanteEmisor
            {
               Rfc = "PUR790718MG1",
               Nombre = "PROMOCION URBANA",
               RegimenFiscal = c_RegimenFiscal.Item601,
            },
            Receptor = new ComprobanteReceptor
            {
               Rfc = pCliente.Rfc.ToUpper().Trim(),
               Nombre = pCliente.Nombre.ToUpper().Trim(),
               DomicilioFiscalReceptor = pCliente.CodigoPost,
               RegimenFiscalReceptor = ( c_RegimenFiscal )Enum.Parse(typeof(c_RegimenFiscal), "Item" + lRegimenFiscal.Clave),
               UsoCFDI = ( c_UsoCFDI )Enum.Parse(typeof(c_UsoCFDI), lUsoCfdi.Clave),
               ResidenciaFiscalSpecified = false
            },
            Conceptos = lConceptos.ToArray(),
            Impuestos = new ComprobanteImpuestos
            {
               TotalImpuestosTrasladados = Traslado.Importe,
               Traslados = new ComprobanteImpuestosTraslado[1] { Traslado },
               TotalImpuestosTrasladadosSpecified = true,
               TotalImpuestosRetenidosSpecified = false
            },

         };
         #endregion

         byte numero;
         if ( !( pCliente.Rfc.Trim().Length > 12 || !byte.TryParse(caracter, out numero) ) )
         {
            iDoc.Receptor.RegimenFiscalReceptor = c_RegimenFiscal.Item601;
         }
         if ( pCliente.Rfc.ToUpper().Equals("XEXX010101000") )
         {
            iDoc.Receptor.RegimenFiscalReceptor = c_RegimenFiscal.Item616;
         }

         return iDoc;
      }
   }
}
