using System;

namespace aigis.Models
{
    public class Cita
    {
        public string? _id { get; set; } 
        public string? usuarioId { get; set; }
        public DateTime fecha { get; set; }
        public string? hora { get; set; }
        public string? colonia { get; set; }
        public string? calle { get; set; }
        public string? numero { get; set; }
        public string? referencia { get; set; }
        public string? motivo { get; set; }
        public string? estado { get; set; }
    }
}
