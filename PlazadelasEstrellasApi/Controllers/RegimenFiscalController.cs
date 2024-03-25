using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlazadelasEstrellasApi.Models;

namespace PlazadelasEstrellasApi.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class RegimenFiscalController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;

      public RegimenFiscalController( PlazaEstrellasDBContext context )
      {
         _context = context;
      }

      // GET: api/RegimenFiscal
      [HttpGet]
      public async Task<ActionResult<IEnumerable<RegimenFiscal>>> GetRegimenFiscal()
      {
         return await _context.RegimenFiscal.ToListAsync();
      }

      // GET: api/RegimenFiscal/601
      [HttpGet("{clave}")]
      public async Task<ActionResult<RegimenFiscal>> GetRegimenFiscal( string clave )
      {
         var regimenFiscal = await _context.RegimenFiscal
               .Where(C => C.Clave == clave)
               .ToListAsync();

         if ( !regimenFiscal.Any() )
         {
            return NotFound();
         }

         return regimenFiscal.ToArray()[0];
      }

      private bool RegimenFiscalExists( int? id )
      {
         return _context.RegimenFiscal.Any(e => e.Id == id);
      }
   }
}
