#nullable enable

using System;
using System.Collections.Generic;

namespace PlazadelasEstrellasApi.Models
{
    public partial class Codigopostal
    {
        public int? Id { get; set; }
        public string CodigoPost { get; set; } = null!;
        public string? Colonia { get; set; }
        public string Municipio { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string? Ciudad { get; set; }
    }
}
