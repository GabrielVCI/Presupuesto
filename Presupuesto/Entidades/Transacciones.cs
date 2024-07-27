namespace Presupuesto.Entidades
{
    public class Transacciones
    {
        public int Id { get; set; }
        public string Codigo_transaccion { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public int PersonaIds { get; set; } 
        public int CantidadDepositada { get; set; }
        public Personas personas { get; set; } 
        public string ContentType { get; set; } //This would be like the FILE name for us
        public long TamañoImagen { get; set; }
        public string URL { get; set; }
         
    }
}
