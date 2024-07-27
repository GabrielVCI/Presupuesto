using Microsoft.EntityFrameworkCore;

namespace Presupuesto.Entidades
{
    public class ImagenTransaccion
    {
        public int Id { get; set; }
        public string Titulo { get; set;}
        public string ContentType { get; set;}
        public long Tamaño { get; set;}
        public int TransaccionId { get; set; }
        public Transacciones transacciones { get; set; }
    }
}
