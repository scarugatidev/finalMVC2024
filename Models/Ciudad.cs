using System;

namespace sistemaWEB.Models
{
    public class Ciudad
    {


        //declaracion de variables con properties
        public int id { get; set; }
        public string nombre {  get; set; }
        public List<Hotel> listHoteles { get; set; }

        //foreing key
        public List<Vuelo> listVuelosOrigen { get; set; }
        public List<Vuelo> listVuelosDestino { get; set; }
        //public int idVuelo {  get; set; }//borrar
       


        //Constructores
        public Ciudad() { }
        public Ciudad(int id, string nombre)
        {
            this.id = id;
            this.nombre = nombre;
            listHoteles = new List<Hotel>();
            //listVuelos = new List<Vuelo>();
        }

        public Ciudad(string nombre)
        {
            this.id = id;
            this.nombre = nombre;
            //listHoteles = new List<Hotel>();
            //listVuelos = new List<Vuelo>();
        }

        public string[] ToString()
        {
            return new string[] { id.ToString(), nombre.ToString() };
        }

    }

    }

   
