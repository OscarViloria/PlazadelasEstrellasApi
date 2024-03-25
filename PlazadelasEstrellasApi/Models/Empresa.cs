#nullable enable

using System;
using System.Collections.Generic;

namespace PlazadelasEstrellasApi.Models
{
  public partial class Empresa
  {
    public string Id { get; set; } = null!;
    public string Clave { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? RazonSocial { get; set; }
    public string Rfc { get; set; } = null!;
    public string Calle { get; set; } = null!;
    public string NumeroExterior { get; set; } = null!;
    public string? NumeroInterior { get; set; }
    public string? Colonia { get; set; }
    public string CodigoPost { get; set; } = null!;
    public string? Delegacion { get; set; }
    public string Estado { get; set; } = null!;
    public string Municipio { get; set; } = null!;
    public string Codigo { get; set; } = null!;
    public string? Urlservicio { get; set; }
    public int? Dias { get; set; }
    public int? ConteoInvocaciones { get; set; }
    public string AsuntoCorreo { get; set; } = null!;
  }
}
