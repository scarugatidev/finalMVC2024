namespace sistemaWEB.Models.Business.ReservaVuelo
{
    public class BusinessReservaVuelo
    {
        public int idVuelo { get; set; }
        public int cantPasajeros { get; set; }
        public int CiudadOrigen { get; set; }
        public int CiudadDestino { get; set; }
        public DateTime? fecha { get; set; }
        public List<BusinessVuelos> vuelos { get; set; }
    }
}
