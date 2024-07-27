namespace Presupuesto.Entidades
{
    public class Aportes
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public int PersonaId { get; set; }
        public int TransaccionId { get; set; }
        public string CodigoFactura {  get; set; }
        public Personas Persona { get; set; }
        public DateTime FechaRealizacion { get; set;}
        public string CodigoTransaccion { get; set; }
        public bool Aprobado {  get; set; }
    }
}
