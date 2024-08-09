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
     
       


        //Constructores
        public Ciudad() { }
        public Ciudad(int id, string nombre)
        {
            this.id = id;
            this.nombre = nombre;
            listHoteles = new List<Hotel>();
        }

        public Ciudad(string nombre)
        {
            this.id = id;
            this.nombre = nombre;
        }

        public string[] ToString()
        {
            return new string[] { id.ToString(), nombre.ToString() };
        }

    }

    }

   
