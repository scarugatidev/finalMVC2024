using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace sistemaWEB.Models
{
    public class Usuario 
    {

        public int id { get; set; }
        public string name { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
        public int intentosFallidos { get; set; }
        public bool bloqueado { get; set; }
        public List<ReservaHotel> listMisReservasHoteles { get; set; }

        public List<ReservaVuelo> listMisReservasVuelo { get; set; }
        public List<Vuelo> listVuelosTomados { get; set; }//Icoleccion en vez de List?

        public List<Hotel> listHotelesVisitados { get; set; }//Icoleccion en vez de List?
        public double credito { get; set; }
        public bool esAdmin { get; set; }

        public List<HotelUsuario> hotelUsuario { get; set; }
        public List<VueloUsuario> vueloUsuarios { get; set; }

        public Usuario() { }

        //constructor para formUsuarioRegistro
        //nombre, apellido, dni, mail, password
        public Usuario( string name, string apellido, string dni, string mail,string password)
        {
            
            this.name = name;
            this.apellido = apellido;
            this.dni = dni;
            this.mail = mail;
            this.password = password;
            esAdmin = false;
            listMisReservasHoteles = new List<ReservaHotel>();
            listMisReservasVuelo = new List<ReservaVuelo>();
            listHotelesVisitados = new List<Hotel>();
            listVuelosTomados = new List<Vuelo>();
        }
        //constructor para formUsuario
        
        public Usuario(int id,string name, string apellido, string dni, string mail)
        {
            this.id = id;
            this.name = name;
            this.apellido = apellido;
            this.dni = dni;
            this.mail = mail;
            password = "password";
            esAdmin = false;
            listMisReservasHoteles = new List<ReservaHotel>();
            listMisReservasVuelo = new List<ReservaVuelo>();
            listHotelesVisitados = new List<Hotel>();
            listVuelosTomados = new List<Vuelo>();
        }
        
        public Usuario(int id, Int32 dni, string name, string apellido, string mail, string password, Int32 intentosFallidos, bool bloqueado, double credito, bool esAdmin)
        {
            this.id = id;
            this.name = name;
            this.apellido = apellido;
            this.dni = Convert.ToString(dni);
            this.mail = mail;
            this.password = password;
            this.intentosFallidos = intentosFallidos;
            this.bloqueado = bloqueado;
            this.credito = credito;
            this.esAdmin = esAdmin;
            listMisReservasHoteles = new List<ReservaHotel>();
            listMisReservasVuelo = new List<ReservaVuelo>();
            listHotelesVisitados = new List<Hotel>();
            listVuelosTomados = new List<Vuelo>();
        }
        /*
        public Usuario(int id, string name, string apellido, string dni, string mail, bool esADM, string password)
        {
            this.id = id;
            this.name = name;
            this.apellido = apellido;
            this.dni = dni;
            this.esAdmin = false;
            this.password = password;
            this.mail = mail;
            listMisReservasHoteles = new List<ReservaHotel>();
            listMisReservasVuelo = new List<ReservaVuelo>();
            listHotelesVisitados = new List<Hotel>();
            listVuelosTomados = new List<Vuelo>();
        }
        */
        
        public Usuario(int id, string dni, string name, string apellido,string mail,string password, bool EsADM, bool Bloqueado)
        {
            this.id = id;
            this.dni = dni;
            this.name = name;
            this.apellido = apellido;
            this.mail = mail;
            this.password = password;
            this.esAdmin = EsADM;
            this.bloqueado = Bloqueado;
            listMisReservasHoteles = new List<ReservaHotel>();
            listMisReservasVuelo = new List<ReservaVuelo>();
            listHotelesVisitados = new List<Hotel>();
            listVuelosTomados = new List<Vuelo>();
        }


        //metodos

        public void setReservaHotel(ReservaHotel reserva)
        {
            listMisReservasHoteles.Add(reserva);
        }

        public void agregarIntentoFallido()
        {
            intentosFallidos++;
            if (intentosFallidos >= 3)
            {
                bloqueado = true;
            }
        }

        public void agregarReservaVuelo(ReservaVuelo reserva)
        {
            listMisReservasVuelo.Add(reserva);
        }

        public void agregarVueloTomado(Vuelo vuelo)
        {
            listVuelosTomados.Add(vuelo);
        }

    }
}