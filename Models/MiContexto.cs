using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistemaWEB.Models
{
    public class MiContexto : DbContext
    {
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Ciudad> ciudades { get; set; }
        public DbSet<Hotel> hoteles { get; set; }
        public DbSet<HotelUsuario> HotelUsuario { get; set; }
        public DbSet<Vuelo> vuelos { get; set; }
        public DbSet<ReservaHotel> reservaHoteles { get; set; }
        public DbSet<ReservaVuelo> reservaVuelos { get; set; }

        public DbSet<VueloUsuario> vueloUsuarios { get; set; }
        public MiContexto(DbContextOptions<MiContexto> optionsBuilder) : base(optionsBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var connectionString = Configuration.GetConnectionString("Context");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //tablas de la base
            //modelBuilder.Ignore<Agencia>();//dejamos fuera del modelo a la clase logica

            modelBuilder.Entity<Usuario>()
                .ToTable("Usuario")
                .HasKey(u => u.id);

            modelBuilder.Entity<Ciudad>()
                .ToTable("Ciudad")
                .HasKey(c => c.id);

            modelBuilder.Entity<Hotel>()
                .ToTable("Hotel")
                .HasKey(h => h.id);

            modelBuilder.Entity<Vuelo>()
                .ToTable("Vuelo")
                .HasKey(v => v.id);

            modelBuilder.Entity<ReservaHotel>()
                .ToTable("ReservaHotel")
                .HasKey(r => r.idReservaHotel);

            modelBuilder.Entity<ReservaVuelo>()
                .ToTable("ReservaVuelo")
                .HasKey(r => r.idReservaVuelo);


            //RELACIONES

            #region Relaciones de Usuario
            //USUARIO -> RESERVAHOTEL

            modelBuilder.Entity<ReservaHotel>()
               .HasOne(R => R.miUsuario)
               .WithMany(U => U.listMisReservasHoteles)
               .HasForeignKey(R => R.idUsuario)
               .OnDelete(DeleteBehavior.Cascade);


            //USUARIO -> RESERVAVUELO

            modelBuilder.Entity<ReservaVuelo>()
                .HasOne(R => R.miUsuario)
                .WithMany(U => U.listMisReservasVuelo)
                .HasForeignKey(R => R.idUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            //USUARIO -> VUELO many to many
            modelBuilder.Entity<Usuario>()
                .HasMany(U => U.listVuelosTomados)
                .WithMany(V => V.listPasajeros)
                .UsingEntity<VueloUsuario>(
                euv => euv.HasOne(uv => uv.vuelo).WithMany(v => v.vueloUsuarios).HasForeignKey(u => u.idVuelo),
                euv => euv.HasOne(uv => uv.user).WithMany(u => u.vueloUsuarios).HasForeignKey(u => u.idUsuario),
                euv => euv.HasKey(k => new { k.idVuelo, k.idUsuario })
                );

            //USUARIO -> HOTEL many to many
            modelBuilder.Entity<Usuario>()
               .HasMany(u => u.listHotelesVisitados)
               .WithMany(h => h.listHuespedes)
               .UsingEntity<HotelUsuario>(
                ehu => ehu.HasOne(hu => hu.hotel).WithMany(h => h.hotelUsuario).HasForeignKey(u => u.idHotel),
                ehu => ehu.HasOne(hu => hu.user).WithMany(u => u.hotelUsuario).HasForeignKey(u => u.idUsuario),
                ehu => ehu.HasKey(k => new { k.idHotel, k.idUsuario })
                );
            #endregion

            #region Relaciones de Vuelo
            //RELACIONES VUELO

            //VUELO -> USUARIO con la tabla intermedia ReservaHotel
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.listVuelosTomados)
                .WithMany(v => v.listPasajeros)
                .UsingEntity<ReservaVuelo>(
                evu => evu.HasOne(rvu => rvu.miVuelo).WithMany(v => v.listMisReservas).HasForeignKey(v => v.idVuelo),
                evu => evu.HasOne(rvu => rvu.miUsuario).WithMany(u => u.listMisReservasVuelo).HasForeignKey(v => v.idUsuario)

                ); ;

            //VUELO -> CIUDAD
            //CIUDAD -> VUELO one to many

            modelBuilder.Entity<Vuelo>()
                .HasOne(v => v.origen)
                .WithMany(c => c.listVuelosOrigen)
                .HasForeignKey(v => v.CiudadOrigenId)
                .OnDelete(DeleteBehavior.Restrict);//para que no se elimine una ciudad si elimino un vuelo

            modelBuilder.Entity<Vuelo>()
                .HasOne(v => v.destino)
                .WithMany(c => c.listVuelosDestino)
                .HasForeignKey(v => v.CiudadDestinoId)
                .OnDelete(DeleteBehavior.NoAction);




            #endregion

            #region Relaciones de Hotel
            //RELACIONES HOTEL

            //HOTEL -> CIUDAD
            //CIUDAD -> HOTEL one to many

            modelBuilder.Entity<Hotel>()
                .HasOne(H => H.ubicacion)
                .WithMany(C => C.listHoteles)
                .HasForeignKey(H => H.idCiudad)
                .OnDelete(DeleteBehavior.Cascade);

            //HOTEL -> USUARIO con la tabla intermedia ReservaHotel
            modelBuilder.Entity<Usuario>()

                .HasMany(u => u.listHotelesVisitados)
                .WithMany(h => h.listHuespedes)
                .UsingEntity<ReservaHotel>(
                ehu => ehu.HasOne(rh => rh.miHotel).WithMany(h => h.listMisReservas).HasForeignKey(u => u.idHotel),
                ehu => ehu.HasOne(hu => hu.miUsuario).WithMany(u => u.listMisReservasHoteles).HasForeignKey(u => u.idUsuario)


             ); ;


            //HOTEL -> ReservaHotel

            #endregion

            //

            //


            //propiedades de los datos
            modelBuilder.Entity<Usuario>(
                usr =>//armo un array de todas las propiedades que deseo setear, sino seria una instruccion por cada dato
                {
                    usr.Property(u => u.name).HasColumnType("varchar(50)");
                    usr.Property(u => u.apellido).HasColumnType("varchar(50)");
                    usr.Property(u => u.mail).HasColumnType("varchar(256)");
                    usr.Property(u => u.mail).IsRequired(true);
                    usr.Property(u => u.dni).HasColumnType("varchar(10)");
                    usr.Property(u => u.dni).IsRequired(true);
                    usr.Property(u => u.password).HasColumnType("varchar(50)");
                    usr.Property(u => u.password).IsRequired(true);
                    usr.Property(u => u.intentosFallidos).HasColumnType("int");
                    usr.Property(u => u.bloqueado).HasColumnType("bit");
                    usr.Property(u => u.credito).HasColumnType("float");
                    usr.Property(u => u.esAdmin).HasColumnType("bit");

                });

            modelBuilder.Entity<Ciudad>(
                c =>
                {
                    c.Property(c => c.nombre).HasColumnType("varchar(50)");
                    c.Property(c => c.nombre).IsRequired(true);
                }

                );

            modelBuilder.Entity<Hotel>(
                hts =>
                {
                    //hts.Property(h => h.ubicacion).HasColumnType("int");
                    //hts.Property(h => h.ubicacion).IsRequired(true);
                    hts.Property(h => h.capacidad).HasColumnType("int");
                    hts.Property(h => h.capacidad).IsRequired(true);
                    hts.Property(h => h.costo).HasColumnType("float");
                    hts.Property(h => h.costo).IsRequired(true);
                    hts.Property(h => h.nombre).HasColumnType("varchar(50)");
                    hts.Property(h => h.nombre).IsRequired(true);
                }

                );

            modelBuilder.Entity<Vuelo>(
                vue =>
                {//verificar datos y ordes con los metodos
                 //vue.Property(v => v.origen).HasColumnType("int");
                 //vue.Property(v => v.origen).IsRequired(true);
                 //vue.Property(v => v.destino).HasColumnType("int");
                 //vue.Property(v => v.destino).IsRequired(true);
                    vue.Property(v => v.capacidad).HasColumnType("int").IsRequired(true);
                    vue.Property(v => v.vendido).HasColumnType("int").IsRequired(true);
                    vue.Property(v => v.costo).HasColumnType("float").IsRequired(true);
                    vue.Property(v => v.fecha).HasColumnType("datetime").IsRequired(true);
                    vue.Property(v => v.aerolinea).HasColumnType("varchar(50)").IsRequired(true);
                    vue.Property(v => v.avion).HasColumnType("varchar(50)").IsRequired(true);

                }
                );

            modelBuilder.Entity<ReservaHotel>(
                rh =>
                {//verificar el ordden
                    //rh.Property(r => r.miUsuario).HasColumnType("int");
                    //rh.Property(r => r.miUsuario).IsRequired(true); 
                    rh.Property(r => r.fechaDesde).HasColumnType("datetime");
                    rh.Property(r => r.fechaDesde).IsRequired(true);
                    rh.Property(r => r.fechaHasta).HasColumnType("datetime");
                    rh.Property(r => r.fechaHasta).IsRequired(true);
                    rh.Property(r => r.pagado).HasColumnType("float");
                    //rh.Property(r => r.miHotel).HasColumnType("int");
                    //rh.Property(r => r.miHotel).IsRequired(true);



                }
                );

            modelBuilder.Entity<ReservaVuelo>(
                rv =>
                {//verificar orden
                    //rv.Property(r => r.miVuelo).HasColumnType("int");
                    //rv.Property(r => r.miVuelo).IsRequired(true);
                    //rv.Property(r => r.miUsuario).HasColumnType("int");
                    //rv.Property(r => r.miUsuario).IsRequired(true);
                    rv.Property(r => r.pagado).HasColumnType("float");
                    rv.Property(r => r.pagado).IsRequired(true);
                }
                );

            //carga de usuarios amodelo
            #region carga de usuario al modelo
            modelBuilder.Entity<Usuario>().HasData(
        new
        {
            id = 1,
            name = "admin",
            apellido = "admin",
            dni = "10111222",
            mail = "admin@admin.com",
            password = "12345",
            intentosFallidos = 0,
            bloqueado = false,
            credito = 50000.0,
            esAdmin = true
        },
        new
        {
            id = 2,
            name = "juan",
            apellido = "perez",
            dni = "11222333",
            mail = "juan@juan.com",
            password = "12345",
            intentosFallidos = 0,
            bloqueado = false,
            credito = 50000.0,
            esAdmin = false
        },
        new
        {
            id = 3,
            name = "luciana",
            apellido = "perez",
            dni = "33222111",
            mail = "luciana@luciana.com",
            password = "12345",
            intentosFallidos = 0,
            bloqueado = false,
            credito = 10000.0,
            esAdmin = false
        },
        new
        {
            id = 4,
            name = "pedro ",
            apellido = "gomez",
            dni = "22333444",
            mail = "perdo@pedro.com",
            password = "12345",
            intentosFallidos = 0,
            bloqueado = false,
            credito = 20000.0,
            esAdmin = false
        }


        ); ;
            #endregion
            //carga ciudad modelo

            modelBuilder.Entity<Ciudad>().HasData(
                new
                {
                    id = 1,
                    nombre = "Salta"
                },
                new
                {
                    id = 2,
                    nombre = "Buenos Aires"
                },
                new
                {
                    id = 3,
                    nombre = "Mendoza"
                }
                );

            //carga vuelos modelo


            modelBuilder.Entity<Vuelo>().HasData(
                 new
                 {
                     id = 1,
                     CiudadOrigenId = 1,
                     CiudadDestinoId = 2,
                     capacidad = 30,
                     vendido = 0,
                     costo = 1000.0,
                     fecha = DateTime.Parse("2023-11-11"),
                     aerolinea = "AA",
                     avion = "Airbus"
                 }
                 );

        }
    }
}
