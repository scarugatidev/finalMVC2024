namespace sistemaWEB.Models
{
    public class ReservaHotel
    {
        public int idReservaHotel { get; set; }
        public Hotel miHotel { get; set; }
        
        public int idHotel { get; set; }//foreign key
        public Usuario miUsuario { get; set; }
        public int cantidadPersonas { get; set; }
        public int idUsuario { get; set; } //foreign key
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public double pagado { get; set; }



        //constructor
        public ReservaHotel() { }
        public ReservaHotel(Hotel miHotel,
                            Usuario miUsuario,
                            DateTime fechaDesde,
                            DateTime fechaHasta,
                            double pagado,int cantidadPersonas)
        {
            this.miHotel = miHotel;
            this.miUsuario = miUsuario;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.pagado = pagado;
            this.cantidadPersonas = cantidadPersonas;
        }


        //metodos
    
   


    }
}