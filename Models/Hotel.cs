using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistemaWEB.Models
{
    public class Hotel
    {


        //declaracion de variables publicac on properties
        public int id { get; set; }
        public Ciudad ubicacion { get; set; }
        public int capacidad { get; set; }
        public double costo { get; set; }
        public List<Usuario> listHuespedes { get; set; }//ICollection?

        public string nombre { get; set; }

        //public List<ReservaHotel> listMisReservas { get; set; }
        public ICollection<ReservaHotel> listMisReservas { get; } = new List<ReservaHotel>();

        public List<HotelUsuario> hotelUsuario { get; set; }

        //fereing key
        public int idCiudad { get; set; }

       // public int idUsuario {  get; set; }



        //metodos constructores
        public Hotel() { }
        public Hotel(int id, Ciudad ubicacion, int capacidad, double costo, string nombre, List<Usuario> listHuespedes, List<ReservaHotel> listMisReservas)
        {
            this.id = id;
            this.ubicacion = ubicacion;
            this.capacidad = capacidad;
            this.costo = costo;
            this.listHuespedes = listHuespedes;
            this.listMisReservas = listMisReservas;
            this.nombre = nombre;
        }

        public Hotel(int id,Ciudad ubicacion, int capacidad, double costo, string nombre)
        {
            this.id = id;
            this.ubicacion = ubicacion;
            this.capacidad = capacidad;
            this.costo = costo;
            listHuespedes = new List<Usuario>();
            this.listMisReservas = new List<ReservaHotel>();
            this.nombre = nombre;
        }

        public Hotel(Ciudad ubicacion, Int32 capacidad, double costo, string nombre)
        {
            this.ubicacion = ubicacion;
            this.capacidad = capacidad;
            this.costo = costo;
            this.nombre = nombre;
        }



        //metodos
        public string[] ToString()
        {
            string[] ubicacionArr = ubicacion.ToString();
            return new string[] { id.ToString(), ubicacionArr[1], capacidad.ToString(), costo.ToString(), nombre.ToString() };
        }
    }
}
