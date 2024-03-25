using System;
using System.Collections.Generic;

namespace PlazadelasEstrellasApi.Models
{
    public partial class Usocfdi
    {
        public int? Id { get; set; }
        public string Clave { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Fisica { get; set; } = false;
        public bool Moral { get; set; } = false;
    }
}
