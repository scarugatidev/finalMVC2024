namespace sistemaWEB.Models.Business.ReservaHotel
{
    public class BusinessDispReservaHotel
    {

        public int id { get; set; }
        public string ubicacion { get; set; }
        public int disponibilidad { get; set; }
        public double costo { get; set; }
        public string nombre { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

    }
}
