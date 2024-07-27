using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Presupuesto.Models
{
    public class TransaccionEditarDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Código de la transacción")]
        public string Codigo_transaccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Fecha de la transacción")]
        public DateTime FechaTransaccion { get; set; }

        [Display(Name = "Imagen de la transacción")]
        [Required(ErrorMessage = "Debe agregar una imagen")]
        public IFormFile File { get; set; } 
        public int? PersonaId { get; set; }
    }
}
