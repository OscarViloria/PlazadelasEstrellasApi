using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CFDIv4.Utils
{
   public class SelloDigital
   {
      public string Sellar( string CadenaOriginal, string ArchivoClavePrivada, string lPassword )
      {
         byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
         byte[] bytesFirmados = null;
         byte[] bCadenaOriginal = null;

         SecureString lSecStr = new SecureString();
         SHA256Managed sham = new SHA256Managed();
         // SHA1Managed sham = new SHA1Managed(); version 3.2
         lSecStr.Clear();

         foreach ( char c in lPassword.ToCharArray() )
            lSecStr.AppendChar(c);

         RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
         bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
         try
         {
            bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

         }
         catch ( NullReferenceException ex )
         {
            throw new NullReferenceException("Clave privada incorrecta, revisa que la clave que escribes corresponde a los sellos digitales cargados");
         }
         string sellodigital = Convert.ToBase64String(bytesFirmados);
         return sellodigital;

      }
      /// <summary>
      /// metodo que realiza el sello reciviendo el archivo key como matriaz de bytes
      /// </summary>
      /// <param name="CadenaOriginal"></param>
      /// <param name="ArchivoClavePrivada"></param>
      /// <param name="lPassword"></param>
      /// <returns></returns>
      public string Sellar( string CadenaOriginal, byte[] ArchivoClavePrivada, string lPassword )
      {
         byte[] ClavePrivada = ArchivoClavePrivada;
         byte[] bytesFirmados = null;
         byte[] bCadenaOriginal = null;

         SecureString lSecStr = new SecureString();
         SHA256Managed sham = new SHA256Managed();
         lSecStr.Clear();

         foreach ( char c in lPassword.ToCharArray() )
            lSecStr.AppendChar(c);

         RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
         bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
         try
         {
            bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

         }
         catch ( NullReferenceException )
         {
            throw new NullReferenceException("Clave privada incorrecta.");
         }
         string sellodigital = Convert.ToBase64String(bytesFirmados);
         return sellodigital;

      }

      public bool verificarSello( string CadenaOriginal, string ArchivoClavePrivada, string lPassword, string ArchivoClavePublica )
      {
         byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
         byte[] bytesFirmados = null;
         byte[] bCadenaOriginal = null;

         SecureString lSecStr = new SecureString();
         SHA1Managed sham = new SHA1Managed();
         lSecStr.Clear();

         foreach ( char c in lPassword.ToCharArray() )
            lSecStr.AppendChar(c);

         RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
         bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
         try
         {
            bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

         }
         catch ( NullReferenceException )
         {
            throw new NullReferenceException("Clave privada incorrecta.");
         }

         string sellodigital = Convert.ToBase64String(bytesFirmados);

         RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
         SHA1Managed hash = new SHA1Managed();
         byte[] hashedData;

         //rsaCSP.ImportParameters(rsaParams);
         //rsaCSP = JavaScience.opensslkey.(File.ReadAllBytes(ArchivoClavePublica));
         bool dataOK = rsaCSP.VerifyData(Encoding.UTF8.GetBytes(CadenaOriginal), CryptoConfig.MapNameToOID("SHA1"), bytesFirmados);
         hashedData = hash.ComputeHash(bytesFirmados);
         return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), Encoding.UTF8.GetBytes(CadenaOriginal));
      }//*/

      public string SellarMD5( string CadenaOriginal, string ArchivoClavePrivada, string lPassword )
      {
         byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
         byte[] bytesFirmados = null;
         byte[] bCadenaOriginal = null;
         SecureString lSecStr = new SecureString();
         lSecStr.Clear();
         foreach ( char c in lPassword.ToCharArray() )
            lSecStr.AppendChar(c);
         RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
         MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
         bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
         hasher.ComputeHash(bCadenaOriginal);
         bytesFirmados = lrsa.SignData(bCadenaOriginal, hasher);
         string sellodigital = Convert.ToBase64String(bytesFirmados);
         return sellodigital;

      }
      public string Certificado( string ArchivoCER )
      {
         byte[] Certificado = File.ReadAllBytes(ArchivoCER);
         return Base64_Encode(Certificado);
      }
      public string Certificado( byte[] ArchivoCER )
      {
         return Base64_Encode(ArchivoCER);
      }
      string Base64_Encode( byte[] str )
      {
         return Convert.ToBase64String(str);
      }
      byte[] Base64_Decode( string str )
      {
         try
         {
            byte[] decbuff = Convert.FromBase64String(str);
            return decbuff;
         }
         catch
         {
            { return null; }
         }
      }
      public static string getCadenaOriginal( string NombreXML )
      {
         System.Xml.Xsl.XslCompiledTransform transformer = new System.Xml.Xsl.XslCompiledTransform();
         //Encoding utf8 = Encoding.UTF8;
         //byte[] encodedBytes;
         StringWriter strwriter = new StringWriter();
         if ( File.Exists("cadenaoriginal_4_0.xslt") )
         {
            //cargamos el xslt transformer
            try
            {
               transformer.Load("cadenaoriginal_4_0.xslt");
               //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
               transformer.Transform(NombreXML, null, strwriter);
               //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
               //encodedBytes = utf8.GetBytes(strwriter.ToString());
               return strwriter.ToString();
            }
            catch ( Exception e )
            {
               Console.WriteLine(e.Message);
            }
         }

         if ( File.Exists("cadenaoriginal_4_0.xslt") )
         {
            //cargamos el xslt transformer
            try
            {
               transformer.Load("cadenaoriginal_4_0.xslt");
               //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
               transformer.Transform(NombreXML, null, strwriter);
               //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
               //encodedBytes = utf8.GetBytes(strwriter.ToString());
               return strwriter.ToString();
            }
            catch ( Exception e )
            {
               Console.WriteLine(e.Message);
               throw e;
            }
         }
         else return "Error al cargar el validador.";


      }
      public static string md5( string Value )
      {


         //Declarations
         Byte[] originalBytes;
         Byte[] encodedBytes;
         MD5 md5;

         //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
         md5 = new MD5CryptoServiceProvider();
         originalBytes = Encoding.UTF8.GetBytes(Value);
         encodedBytes = md5.ComputeHash(originalBytes);

         //Convert encoded bytes back to a 'readable' string
         string ret = "";
         for ( int i = 0; i < encodedBytes.Length; i++ )
            ret += encodedBytes[i].ToString("x2").ToLower();
         return ret;

      }
      public static bool leerCER( string NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero )
      {




         if ( NombreArchivo.Length < 1 )
         {
            Inicio = "";
            Final = "";
            Serie = "INVALIDO";
            Numero = "";
            return false;
         }
         X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
         StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

         //Detalle
         objSB.AppendLine("Persona = " + objCert.Subject);
         objSB.AppendLine("Emisor = " + objCert.Issuer);
         objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
         Inicio = objCert.NotBefore.ToString();
         objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
         Final = objCert.NotAfter.ToString();
         objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
         objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
         Serie = objCert.SerialNumber.ToString();

         objSB.AppendLine("Hash = " + objCert.Thumbprint);
         //Numero = "?";
         string tNumero = "", rNumero = "", tNumero2 = "";

         int X;
         if ( Serie.Length < 2 )
            Numero = "";
         else
         {
            foreach ( char c in Serie )
            {
               switch ( c )
               {
                  case '0': tNumero += c; break;
                  case '1': tNumero += c; break;
                  case '2': tNumero += c; break;
                  case '3': tNumero += c; break;
                  case '4': tNumero += c; break;
                  case '5': tNumero += c; break;
                  case '6': tNumero += c; break;
                  case '7': tNumero += c; break;
                  case '8': tNumero += c; break;
                  case '9': tNumero += c; break;
               }
            }
            for ( X = 1; X < tNumero.Length; X++ )
            {
               //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
               X += 1;
               //rNumero = rNumero + 
               tNumero2 = tNumero.Substring(0, X);
               rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
            }
            Numero = rNumero;

         }

         if ( DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore )
         {
            return true;
         }



         return false;
      }

      /// <summary>
      /// lee el codigo del certificado enviado este como matriz de bytes
      /// </summary>
      /// <param name="NombreArchivo">certificado en matriz de bytes</param>
      /// <param name="Inicio"></param>
      /// <param name="Final"></param>
      /// <param name="Serie"></param>
      /// <param name="Numero"></param>
      /// <returns></returns>
      public static bool leerCER( byte[] NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero )
      {

         if ( NombreArchivo.Length < 1 )
         {
            Inicio = "";
            Final = "";
            Serie = "INVALIDO";
            Numero = "";
            return false;
         }
         X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
         StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

         //Detalle
         objSB.AppendLine("Persona = " + objCert.Subject);
         objSB.AppendLine("Emisor = " + objCert.Issuer);
         objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
         Inicio = objCert.NotBefore.ToString();
         objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
         Final = objCert.NotAfter.ToString();
         objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
         objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
         Serie = objCert.SerialNumber.ToString();

         objSB.AppendLine("Hash = " + objCert.Thumbprint);
         //Numero = "?";
         string tNumero = "", rNumero = "", tNumero2 = "";

         int X;
         if ( Serie.Length < 2 )
            Numero = "";
         else
         {
            foreach ( char c in Serie )
            {
               switch ( c )
               {
                  case '0': tNumero += c; break;
                  case '1': tNumero += c; break;
                  case '2': tNumero += c; break;
                  case '3': tNumero += c; break;
                  case '4': tNumero += c; break;
                  case '5': tNumero += c; break;
                  case '6': tNumero += c; break;
                  case '7': tNumero += c; break;
                  case '8': tNumero += c; break;
                  case '9': tNumero += c; break;
               }
            }
            for ( X = 1; X < tNumero.Length; X++ )
            {
               //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
               X += 1;
               //rNumero = rNumero + 
               tNumero2 = tNumero.Substring(0, X);
               rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
            }
            Numero = rNumero;

         }

         if ( DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore )
         {
            return true;
         }

         return false;
      }


      public static bool validarCERKEY( string NombreArchivoCER, string NombreArchivoKEY, string ClavePrivada )
      {
         X509Certificate2 certificado = new X509Certificate2(File.ReadAllBytes(NombreArchivoCER));
         //initialze the byte arrays to the public key information.
         byte[] pk = certificado.GetPublicKey();
         X509Certificate2 certPrivado = new X509Certificate2(File.ReadAllBytes(NombreArchivoKEY));
         return false;
      }

      public static void CrearXML( Comprobante oComprobante, string rutaXML )
      {
         //Serialización -----------------------------------------------------------
         XmlSerializerNamespaces xmlNamespace = new XmlSerializerNamespaces();
         xmlNamespace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
         xmlNamespace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
         xmlNamespace.Add("cfdi", "http://www.sat.gob.mx/cfd/4");

         XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
         string sXml = "";

         using ( var sww = new StringWriterEncoding(Encoding.UTF8) )
         {
            using ( XmlWriter writter = XmlWriter.Create(sww) )
            {
               CustomWriter customWriter = new CustomWriter(writter)
               {
                  SchemaLocation = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"
               };
               oXmlSerializar.Serialize(customWriter, oComprobante, xmlNamespace);
               sXml = sww.ToString();
            }
         }

         //Guarda el string en un archivo ------------------------------------------
         System.IO.File.WriteAllText(rutaXML, sXml);
      }

      public static string FormateaXML( string xml )
      {
         //var fecha = xml.Substring(xml.IndexOf(" Fecha=") + 8);
         //fecha = fecha.Substring(0, fecha.IndexOf('"'));
         //var fechanueva = fecha.Substring(0, 19);
         //xml = xml.Replace(fecha, fechanueva);
         var descuentos = xml;
         decimal d = 0;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" Descuento=") + 12;
            if ( posicion < 12 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("###0.00");
            if ( d == 0 )
               d = decimal.Parse(descuentonuevo);
            xml = xml.Replace(descuento, descuentonuevo);
         }
         descuentos = xml;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" ValorUnitario=") + 16;
            if ( posicion < 16 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F2");
            xml = xml.Replace(" ValorUnitario=\"" + descuento + "\"", " ValorUnitario=\"" + descuentonuevo + "\"");
         }
         descuentos = xml;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" Base=") + 7;
            if ( posicion < 7 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F2");
            xml = xml.Replace(" Base=\"" + descuento + "\"", " Base=\"" + descuentonuevo + "\"");
         }
         descuentos = xml;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" Importe=") + 10;
            if ( posicion < 10 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F4");
            xml = xml.Replace(" Importe=\"" + descuento + "\"", " Importe=\"" + descuentonuevo + "\"");
         }
         xml = xml.Replace(" TasaOCuota=\"0.16\"", " TasaOCuota=\"0.16\"");
         xml = xml.Replace(" TasaOCuota=\"0\"", " TasaOCuota=\"0.00\"");
         descuentos = xml;
         descuentos = descuentos.Replace(" TasaOCuota=\"0.16\"", " TasaOCuota=\"0.16\"");
         descuentos = descuentos.Replace(" TasaOCuota=\"0\"", " TasaOCuota=\"0.00\"");
         decimal iva = 0;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" TotalImpuestosTrasladados=") + 28;
            if ( posicion < 28 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf(" Importe=") + 10;
            if ( posicion < 10 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F2");
            iva = decimal.Parse(descuentonuevo);
            xml = xml.Replace(descuento, descuentonuevo);
         }
         descuentos = xml;
         decimal subtotal = 0;
         while ( true )
         {
            var posicion = descuentos.IndexOf("SubTotal=") + 10;
            if ( posicion < 10 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F2");
            subtotal = decimal.Parse(descuentonuevo);
            xml = xml.Replace(" SubTotal=\"" + descuento + "\"", " SubTotal=\"" + descuentonuevo + "\"");
         }
         descuentos = xml;
         decimal total = 0;
         while ( true )
         {
            var posicion = descuentos.IndexOf(" Total=") + 8;
            if ( posicion < 8 )
               break;
            descuentos = descuentos.Substring(posicion);
            posicion = descuentos.IndexOf('"');
            var descuento = descuentos.Substring(0, posicion);
            var descuentonuevo = decimal.Parse(descuento).ToString("F2");
            total = subtotal - d + iva;
            xml = xml.Replace(" Total=\"" + descuento + "\"", " Total=\"" + total.ToString("F2") + "\"");
         }

         return xml;
      }
   }
}
