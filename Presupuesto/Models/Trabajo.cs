using Presupuesto.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class Trabajo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string Nombre { get; set; } 
        public int Aporte { get; set; }
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nota { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido. Debe ser al menos una persona.")]
        [Display(Name = "Cantidad de adultos")]
        public int Cantidad_personas { get; set; }
        public DateTime TiempoDeposito { get; set; }
        public int Total_a_pagar { get; set; }
        public int Pago_restante { get; set; }
        public int ObjetivoId { get; set; }
        public int CantidadMenores { get; set; }
        public int TotalPersonas { get; set; }
    }
}
