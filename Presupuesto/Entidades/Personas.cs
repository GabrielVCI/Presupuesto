using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Entidades
{
    public class Personas
    {
        public int Id { get; set; }
        public string Nombre { get; set;}
        public int Cantidad_personas { get; set;}
        public int Aporte { get; set;}
        public int Total_a_pagar { get; set; }
        public string Nota { get; set;} 
        public DateTime TiempoDeposito { get; set;}
        public int Pago_restante { get; set; }
        public List<Transacciones> Transacciones { get; set;}
        public List<Aportes> Aportes { get; set;}
        public int ObjetivosId { get; set; }
        public int CantidadMenores { get; set; }
        public Objetivos Objetivos { get; set; }
        public int TotalPersonas { get; set; }
    }
}
