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
   public class EmpresasController : ControllerBase
   {
      private readonly PlazaEstrellasDBContext _context;

      public EmpresasController( PlazaEstrellasDBContext context )
      {
         _context = context;
      }

      // GET: api/Empresas/5
      [HttpGet( "{id}" )]
      public async Task<ActionResult<Empresa>> GetEmpresa( string id )
      {
         var empresa = await _context.Empresa.FindAsync( id );

         if ( empresa == null )
         {
            return NotFound();
         }

         return empresa;
      }

      // PUT: api/Empresas/5
      // To protect from overposting attacks, enable the specific properties you want to bind to, for
      // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
      [HttpPut( "{id}" )]
      public async Task<IActionResult> PutEmpresa( string id, Empresa empresa )
      {
         if ( id != empresa.Id )
         {
            return BadRequest();
         }

         _context.Entry( empresa ).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch ( DbUpdateConcurrencyException )
         {
            if ( !EmpresaExists( id ) )
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

      // POST: api/Empresas
      // To protect from overposting attacks, enable the specific properties you want to bind to, for
      // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
      [HttpPost]
      public async Task<ActionResult<Empresa>> PostEmpresa( Empresa empresa )
      {
         _context.Empresa.Add( empresa );
         try
         {
            await _context.SaveChangesAsync();
         }
         catch ( DbUpdateException )
         {
            if ( EmpresaExists( empresa.Id ) )
            {
               return Conflict();
            }
            else
            {
               throw;
            }
         }

         return CreatedAtAction( "GetEmpresa", new { id = empresa.Id }, empresa );
      }

      private bool EmpresaExists( string id )
      {
         return _context.Empresa.Any( e => e.Id == id );
      }
   }
}
