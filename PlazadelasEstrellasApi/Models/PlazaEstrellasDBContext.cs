using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlazadelasEstrellasApi.Models
{
   public class PlazaEstrellasDBContext : DbContext
   {
      public PlazaEstrellasDBContext()
      {
      }

      public PlazaEstrellasDBContext( DbContextOptions<PlazaEstrellasDBContext> options )
        : base( options )
      {
      }

      protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
      {
      }

      public virtual DbSet<Codigopostal> CodigoPostal { get; set; } = null!;
      public virtual DbSet<Empresa> Empresa { get; set; } = null!;
     public virtual DbSet<Factura> Factura { get; set; } = null!;
     public virtual DbSet<Usocfdi> UsoCfdi { get; set; } = null!;
      public virtual DbSet<RegimenFiscal> RegimenFiscal { get; set; } = null!;

   }
}
