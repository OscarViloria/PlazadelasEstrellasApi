using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlazadelasEstrellasApi.Models
{
   public partial class RegimenFiscal
   {
      public int? Id { get; set; }
      public string Clave { get; set; } = null!;
      public string Descripcion { get; set; } = null!;
      public bool Fisica { get; set; } = false;
      public bool Moral { get; set; } = false;
   }
}
