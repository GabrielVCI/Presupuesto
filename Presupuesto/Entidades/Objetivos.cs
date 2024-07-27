using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Presupuesto.Entidades
{
    public class Objetivos
    {
        public int Id { get; set; }
        public string NombreObjetivo { get; set; }
        public string Descripcion { get; set; }
        public int ObjetivoMonetario { get; set; }
        public int CantidadAdicional { get; set; }
        public string FormaPago { get; set; }
        public DateTime FechaLimite { get; set; }
        public string UsuarioCreacionId { get; set; }
        public IdentityUser UsuarioCreacion { get; set; }
        public List<Personas> personas { get; set; }
        public bool Activo { get; set; }
        public bool HaSidoEditado { get; set; }

        public Objetivos()
        {
            HaSidoEditado = false;
        }
    }
}
