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
   [Route( "api/[controller]" )]
   [ApiController]
   public class UsocfdisController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;

      public UsocfdisController( PlazaEstrellasDBContext context )
      {
         _context = context;
      }

      // GET: api/Usocfdis
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Usocfdi>>> GetUsoCfdi()
      {
         return await _context.UsoCfdi.ToListAsync();
      }

      // GET: api/Usocfdis/G01
      [HttpGet( "{clave}" )]
      public async Task<ActionResult<Usocfdi>> GetUsocfdi( string clave )
      {
         var usocfdi = await _context.UsoCfdi
                            .Where(C => C.Clave == clave)
                            .ToListAsync();

         if ( !usocfdi.Any() )
         {
            return NotFound();
         }

         return usocfdi.ToArray()[0];
      }

      private bool UsocfdiExists( int? id )
      {
         return _context.UsoCfdi.Any( e => e.Id == id );
      }
   }
}
