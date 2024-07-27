using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class ObjetivoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre del objetivo o actividad")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Descripcion del objetivo o actividad")]
        [StringLength(150, ErrorMessage = "Máximo 30 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Fecha")]
        public DateTime FechaLimite { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Cantidad { get; set; }
        public int CantidadAdicional { get; set; }
        public string FormaPago { get; set; }
        public bool Activo { get; set; }
    }
}
