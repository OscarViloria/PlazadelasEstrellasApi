using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlazadelasEstrellasApi.Models;
using System.Xml;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using CFDIv4.Utils;
using System.Text;
using com.sf.ws.Timbrado;
using static com.sf.ws.Timbrado.TimbradoPortTypeClient;
using com.sf.ws.Utilerias;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using PlazadelasEstrellasApi.Utils;


namespace PlazadelasEstrellasApi.Controllers
{
   [Route( "api/[controller]" )]
   [ApiController]
   public class FacturasController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;
      private readonly IConfiguration _config;
     private Validaciones _validaciones;

      public FacturasController( PlazaEstrellasDBContext context, IConfiguration config )
      {
         _context = context;
         _config = config;
        _validaciones = new Validaciones(context);
      }

      // GET: api/Facturas/Consultar
      [HttpPost( "Consultar" )]
      public async Task<ActionResult<Factura>> GetFactura( Ticket ticket )
      {
         try
         {
            var factura = await _context.Factura.
                  Where(F => F.Ticket == ticket.Folio && F.Rfc != null && F.FechaTicket == ticket.Fecha)
                  .ToListAsync();

            if ( factura.Any() )
            {
               if ( factura[0].Fecha < DateTime.Today.AddMonths(-1) || ticket.Fecha < DateTime.Today.AddMonths(-1) )
                  throw new Exception();

               return Ok(factura.ToArray()[0]);
            }

            return NotFound("Este ticket no esta facturado.");
         } catch
         {
            return Conflict("Error: La Factura o el ticket ya tiene mas de 1 mes de haberse generado.");
         }
      }
        
      // PUT: api/Facturas/54321
      // To protect from overposting attacks, enable the specific properties you want to bind to, for
      // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
      [HttpPut( "{id}" )]
      public async Task<ActionResult<Factura>> PutFactura( int? id, Factura factura )
      {
         if ( id != factura.Id )
         {
            return BadRequest();
         }

         _context.Entry( factura ).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch ( DbUpdateConcurrencyException )
         {
            if ( !FacturaExists( id ) )
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }
        
      // POST: api/Facturas
      // To protect from overposting attacks, enable the specific properties you want to bind to, for
      // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
      [HttpPost]
      public async Task<ActionResult<Factura>> PostFactura( Factura factura )
      {
         _context.Factura.Add( factura );
         await _context.SaveChangesAsync();

         return CreatedAtAction( "GetFactura", new { id = factura.Id }, factura );
      }

      // POST: api/Facturas/Emitir
      [HttpPost( "Emitir" )]
      public async Task<ActionResult<Factura>> PostEmitir( Cliente pCliente )
            {
         var lGenerarFactura = new GenerarFactura( _context );
         var lDataPath = Utils.Utils.ObtenerRuta();
         try
         {
            if ( pCliente.Ticket.Fecha < DateTime.Today.AddMonths(-1) )
               throw new Exception("El ticket ya tiene mas de 1 mes de haberse generado.");

            Console.WriteLine("Inicia proceso de Timbrado");

            Usocfdi lUsoCfdi = _validaciones.ValidarCfdi(pCliente);

            RegimenFiscal lRegimenFiscal = _validaciones.ValidarRegimenFiscal(pCliente);

            //Realiza la conexión a la base y extrae la info del ticket correspondiente
            DateTime lFechaCst = DateTime.UtcNow.AddHours(-6);
            Console.WriteLine(lFechaCst);
            Ticket lTicket = pCliente.Ticket;
            Empresa lEmpresa = _context.Empresa
                            .ToList()[0];
            Factura lFactura = new Factura()
            {
               Ticket = ""+lTicket.Folio,
               CorreoElectronico = pCliente.CorreoElectronico,
               Rfc = pCliente.Rfc,
               Importe = lTicket.Importe,
               Fecha = lFechaCst,
               FechaTicket = lTicket.Fecha
            };
            lEmpresa.ConteoInvocaciones++;
            decimal lSubtotal = ( decimal )( lTicket.Importe / 1.16 );

            Console.WriteLine("Validación de CFDI: Exitosa");

            Comprobante iDoc = lGenerarFactura.GenerarComprobante(pCliente, lSubtotal, lTicket, lUsoCfdi, lRegimenFiscal, lFechaCst);

            Console.WriteLine("Creación de objeto: Exitoso");

            // Obtener certificados ---------------------------------------------------
            AppContext.SetSwitch( "Switch.System.Xml.AllowDefaultResolver", true );
            string lCertificado = Path.Combine( lDataPath, _config["AppSettings:CER"] );
            string lLlave = Path.Combine( lDataPath, _config["AppSettings:KEY"] );
            string lClavePrivada = _config["AppSettings:ClavePrivada"];
            string lRutaCfdi = Path.Combine( lDataPath, @"Facturas" );

            // Obtener numero de certificado ------------------------------------------
            string lNumeroCertificado, aa, b, c;
            var sello = SelloDigital.leerCER( lCertificado, out aa, out b, out c, out lNumeroCertificado );
           iDoc.NoCertificado = lNumeroCertificado;

            var lArchivoXml = Path.Combine( lRutaCfdi, @"Temp.xml" );
           SelloDigital.CrearXML( iDoc, lArchivoXml );

            var xml = System.IO.File.ReadAllText( lArchivoXml, Encoding.UTF8 );
            xml = SelloDigital.FormateaXML( xml );

            Console.WriteLine("Obtener certificado: Exitoso");

            //Creación de cadena original ---------------------------------------------
            SelloDigital selloDigital = new SelloDigital();
            string cadenaOriginal = "";
            string lArchivoXsl = Path.Combine( lDataPath, _config["AppSettings:CadenaOriginal"] );
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform( true );
            transformador.Load( lArchivoXsl );

            using ( StringWriter sw = new StringWriter() )
            using ( XmlWriter xwo = XmlWriter.Create( sw, transformador.OutputSettings ) )
            {
               transformador.Transform( lArchivoXml, xwo );
               cadenaOriginal = sw.ToString();
            }
           iDoc.Certificado = selloDigital.Certificado( lCertificado );

            Console.WriteLine("Obtener cadena original: Exitoso");

            //Sellado del XML ---------------------------------------------------------
           iDoc.Sello = selloDigital.Sellar( cadenaOriginal, lLlave, lClavePrivada );
           SelloDigital.CrearXML( iDoc, lArchivoXml );

            var xmlBytes = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText( lArchivoXml ));

            xml = SelloDigital.FormateaXML(xml);

            Console.WriteLine("Obtener sello: Exitoso");

            #region Timbra XML
                
            bool esPrueba = string.Compare( _config["AppSettings:EsPruebas"], "true") == 0;
            var Timbrador = new TimbradoPortTypeClient( esPrueba ? EndpointConfiguration.TimbradoHttpsSoap12Endpoint : EndpointConfiguration.TimbradoHttpsSoap11Endpoint );
            var Respuesta = Timbrador.timbrarAsync( esPrueba ? "testing@solucionfactible.com" : "cesar_nava@yahoo.com",
               esPrueba ? "timbrado.SF.16672" : "Aa123.123%", xmlBytes, false );
            var Resultado = Respuesta.Result;

            Console.WriteLine("Obtener Timbrado: Exitoso");
                
            if ( Resultado.@return.status == 200 && Resultado.@return.resultados[0].status == 200)
            {

               var RutaFileCFDI = Path.Combine( lRutaCfdi, lFactura.Ticket + "-" + lFactura.FechaTicket.Value.ToString("yyy-MM-dd-HH-mm") + ".xml");

               System.IO.File.WriteAllBytes( RutaFileCFDI, Resultado.@return.resultados[0].cfdiTimbrado );
               xml = System.IO.File.ReadAllText( RutaFileCFDI, Encoding.UTF8 );

               Console.WriteLine("Obtener xml: Exitoso");

                    Utils.Utils.CreaPDF(iDoc, Resultado.@return.resultados[0], lFactura, lEmpresa.ConteoInvocaciones, lDataPath, pCliente);

                    Console.WriteLine("Obtener pdf: Exitoso");

                    //lFactura.Fecha = lFechaCst; - Se paso la fecha al momento de crear el objeto
                    lFactura.Axml = _config["UrlServer:UrlFacturas"] + "/" + lFactura.Ticket + "-" + lFactura.FechaTicket.Value.ToString("yyy-MM-dd-HH-mm") + ".xml";
               lFactura.Apdf = _config["UrlServer:UrlFacturas"] + "/" + lFactura.Ticket + "-" + lFactura.FechaTicket.Value.ToString("yyy-MM-dd-HH-mm") + ".pdf";

               var lResultado = await PostFactura(lFactura);
               await lGenerarFactura.ActualizarEmpresa( lEmpresa );

               if ( string.Compare(_config["AppSettings:EnviarCorreo"], "true") == 0 )
               {
                  EnviarCorreo(lFactura, lDataPath);
               };

               return lResultado;
            }
            else
            {
               Console.WriteLine("Resultado de Timbrado Incorrecto: " + Resultado.@return.status);

               if ( Resultado.@return.status == 630 || Resultado.@return.resultados[0].status == 631 )
               {
                  var message = new MimeMessage
                  {
                     Subject = "Ya se terminaron los timbres.",
                     Body = new BodyBuilder
                     {
                        TextBody = Resultado.@return.status + Environment.NewLine + Resultado.@return.mensaje
                     }.ToMessageBody()
                  };
                  message.From.Add(new MailboxAddress("Factura Electronica", _config["AppSettings:Usuario"]));
                  message.To.Add(new MailboxAddress("", "elias.valencia@sissamx.com.mx"));
                  message.Cc.Add(new MailboxAddress("", "estacionamiento@plazadelasestrellas.com.mx"));

                  using ( var client = new SmtpClient() )
                  {
                     client.ServerCertificateValidationCallback = ( s, c, h, e ) => true;
                     client.Connect(_config["AppSettings:Servidor"], int.Parse(_config["AppSettings:Puerto"]), false);
                     client.Authenticate(_config["AppSettings:Usuario"], _config["AppSettings:Clave"]);
                     client.Send(message);
                     client.Disconnect(true);
                  }

                  Console.WriteLine("Envio Correo: Exitoso");
               }
               throw new Exception( Resultado.@return.resultados[0].status + " - " + Resultado.@return.resultados[0].mensaje );
            }

            #endregion
         }
         catch ( Exception ex )
         {
            var sw = new StreamWriter( Path.Combine( lDataPath, "Logs/Log_" ) + DateTime.Now.ToString( "yyyyMMdd" ) + ".txt", true, Encoding.UTF8 );
            sw.WriteLine( DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) + " Error: " + ex.Message );
            sw.Close();

            return Conflict( "Error: " + ex.Message );
         }
      }

      private bool FacturaExists( int? id )
      {
         return _context.Factura.Any( e => e.Id == id );
      }
        
      private void EnviarCorreo( Factura factura, string lDataPath )
      {
         try
         {
            var builder = new BodyBuilder
            {
               TextBody = "Hola." + Environment.NewLine + Environment.NewLine + "Enviamos su factura electronica " + factura.Ticket + "."
            };
            string lRuta = Path.Combine(lDataPath, @"Facturas");
            builder.Attachments.Add(Path.Combine(lRuta, factura.Ticket + ".xml"));
            builder.Attachments.Add(Path.Combine(lRuta, factura.Ticket + ".pdf"));

            var message = new MimeMessage
            {
               Subject = "Factura electronica - " + factura.Ticket + " - Galerias Plaza de las estrellas ",
               Body = builder.ToMessageBody()
            };

            message.From.Add(new MailboxAddress("Factura Electronica", _config["AppSettings:Usuario"]));
            message.To.Add(new MailboxAddress("", factura.CorreoElectronico));
            message.Cc.Add(new MailboxAddress("", "estacionamiento@plazadelasestrellas.com.mx"));

            using ( var client = new SmtpClient() )
            {
               client.ServerCertificateValidationCallback = ( s, c, h, e ) => true;
               client.Connect(_config["AppSettings:Servidor"], int.Parse(_config["AppSettings:Puerto"]), false);
               client.Authenticate(_config["AppSettings:Usuario"], _config["AppSettings:Clave"]);
               client.Send(message);
               client.Disconnect(true);
            }

            Console.WriteLine("Envio Correo: Exitoso");
         }
         catch ( Exception ex )
         {
            var sw = new StreamWriter(Path.Combine(lDataPath, "Logs/Log_") + DateTime.Now.ToString("yyyyMMdd") + ".txt", true, Encoding.UTF8);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error: " + ex.Message);
            sw.Close();
         }
      }
        
     
   }
}
