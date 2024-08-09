using System.Collections;

namespace sistemaWEB.Models.Business.ReservaHotel
{
    public class BusinessReservaHotel
    {
        public int idReservaHotel { get; set; }
        public int Ciudad { get; set; }
        public int cantidadPersonas { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public float monto { get; set; }
        public List<BusinessDispReservaHotel> DispReservaHotel { get; set; }
    }
}
