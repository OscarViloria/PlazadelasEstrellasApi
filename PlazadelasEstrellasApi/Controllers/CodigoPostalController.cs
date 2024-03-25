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
   public class CodigoPostalController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;

      public CodigoPostalController( PlazaEstrellasDBContext context )
      {
         _context = context;
      }

      // GET: api/CodigoPostal/56500
      [HttpGet( "{CodigoPost}/Consultar" )]
      public async Task<ActionResult<Codigopostal>> GetCodigopostal( string CodigoPost )
      {
         var lCodigoPostal = await _context.CodigoPostal.
                        Where( F => F.CodigoPost == CodigoPost )
                        .ToListAsync();
 
         if ( lCodigoPostal.Any() )
         {
            return Ok( lCodigoPostal );
         }

         return NotFound( "No se encontro el codigo postal." );
      }

      private bool CodigopostalExists( int? id )
      {
         return _context.CodigoPostal.Any( e => e.Id == id );
      }
   }
}
