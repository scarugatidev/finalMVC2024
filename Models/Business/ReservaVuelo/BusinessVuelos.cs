using System.Reflection.Metadata.Ecma335;

namespace sistemaWEB.Models.Business.ReservaVuelo
{
    public class BusinessVuelos
    {
        public int id { get; set; }
        public string vueloOrigenNombre { get; set; }
        public string vueloDestinoNombre { get; set; }
        public int capacidad { get; set; }
        public string costoTotal { get; set; }
        public DateTime fechaFormateada { get; set; }
        public string aerolinea { get; set; }
        public string avion { get; set; }
    }
}
