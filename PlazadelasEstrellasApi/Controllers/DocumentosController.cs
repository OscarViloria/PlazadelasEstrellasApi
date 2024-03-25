using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PlazadelasEstrellasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace PlazadelasEstrellasApi.Controllers
{
   [Route("Data/Facturas")]
   [ApiController]
   public class DocumentosController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;
      private readonly IConfiguration _config;

      public DocumentosController( PlazaEstrellasDBContext context, IConfiguration config )
      {
         _context = context;
         _config = config;
      }

      // GET: Data/Facturas/54321.pdf
      [HttpGet("{documento}")]
      public async Task<ActionResult<MemoryStream>> GetDocumento( string documento )
      {
         var lRuta = Path.Combine(Utils.Utils.ObtenerRuta(), "Facturas");
         string rutaFile = Path.Combine(lRuta, documento);

         var fileBytes = System.IO.File.ReadAllBytes(rutaFile);

         var provider = new FileExtensionContentTypeProvider();
         if ( !provider.TryGetContentType(rutaFile, out var contentType) )
         {
            contentType = "application/octet-stream";
         }

         return File(fileBytes, contentType, Path.GetFileName(rutaFile));
      }
   }
}
