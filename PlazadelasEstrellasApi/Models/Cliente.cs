    #nullable enable

namespace PlazadelasEstrellasApi.Models
{
   public partial class Cliente
   {
      public string Nombre { get; set; } = string.Empty;
      public string CorreoElectronico { get; set; } = string.Empty;
      public string Rfc { get; set; } = string.Empty;
      public Ticket? Ticket { get; set; }
      public string? Cfdi { get; set; }
      public string? RegimenFiscal { get; set; }
      public string Calle { get; set; } = string.Empty;
      public string NumExterior { get; set; } = string.Empty;
      public string? NumInterior { get; set; }
      public string Colonia { get; set; } = string.Empty;
      public string Municipio { get; set; } = string.Empty;
      public string Estado { get; set; } = string.Empty;
      public string CodigoPost { get; set; } = string.Empty;
   }
}
