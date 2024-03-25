#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlazadelasEstrellasApi.Models
{
  public partial class Factura
  {
    public int? Id { get; set; }
    public string Ticket { get; set; } = null!;
    public string Rfc { get; set; } = null!;
    public string CorreoElectronico { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public DateTime? FechaTicket { get; set; }
    public double Importe { get; set; }
    public string? Apdf { get; set; }
    public string? Axml { get; set; }
  }
}
