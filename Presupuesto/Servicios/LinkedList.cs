using Presupuesto.Entidades;

namespace Presupuesto.Servicios
{
    public class LinkedList
    {
        public Transacciones Data {  get; set; }
        public LinkedList Next { get; set; }

        public LinkedList(Transacciones value) 
        {
            Data = value;
            Next = null;    
        } 
    }
}
