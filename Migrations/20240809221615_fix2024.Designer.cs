﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using sistemaWEB.Models;

#nullable disable

namespace sistemaWEB.Migrations
{
    [DbContext(typeof(MiContexto))]
    [Migration("20240809221615_fix2024")]
    partial class fix2024
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("sistemaWEB.Models.Ciudad", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("id");

                    b.ToTable("Ciudad", (string)null);

                    b.HasData(
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
                        });
                });

            modelBuilder.Entity("sistemaWEB.Models.Hotel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("capacidad")
                        .HasColumnType("int");

                    b.Property<double>("costo")
                        .HasColumnType("float");

                    b.Property<int>("idCiudad")
                        .HasColumnType("int");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("id");

                    b.HasIndex("idCiudad");

                    b.ToTable("Hotel", (string)null);
                });

            modelBuilder.Entity("sistemaWEB.Models.HotelUsuario", b =>
                {
                    b.Property<int>("idHotel")
                        .HasColumnType("int");

                    b.Property<int>("idUsuario")
                        .HasColumnType("int");

                    b.Property<int>("cantidad")
                        .HasColumnType("int");

                    b.HasKey("idHotel", "idUsuario");

                    b.HasIndex("idUsuario");

                    b.ToTable("HotelUsuario");
                });

            modelBuilder.Entity("sistemaWEB.Models.ReservaHotel", b =>
                {
                    b.Property<int>("idReservaHotel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idReservaHotel"));

                    b.Property<int>("cantidadPersonas")
                        .HasColumnType("int");

                    b.Property<DateTime>("fechaDesde")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("fechaHasta")
                        .HasColumnType("datetime");

                    b.Property<int>("idHotel")
                        .HasColumnType("int");

                    b.Property<int>("idUsuario")
                        .HasColumnType("int");

                    b.Property<double>("pagado")
                        .HasColumnType("float");

                    b.HasKey("idReservaHotel");

                    b.HasIndex("idHotel");

                    b.HasIndex("idUsuario");

                    b.ToTable("ReservaHotel", (string)null);
                });

            modelBuilder.Entity("sistemaWEB.Models.ReservaVuelo", b =>
                {
                    b.Property<int>("idReservaVuelo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idReservaVuelo"));

                    b.Property<int>("idUsuario")
                        .HasColumnType("int");

                    b.Property<int>("idVuelo")
                        .HasColumnType("int");

                    b.Property<double>("pagado")
                        .HasColumnType("float");

                    b.HasKey("idReservaVuelo");

                    b.HasIndex("idUsuario");

                    b.HasIndex("idVuelo");

                    b.ToTable("ReservaVuelo", (string)null);
                });

            modelBuilder.Entity("sistemaWEB.Models.Usuario", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("apellido")
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("bloqueado")
                        .HasColumnType("bit");

                    b.Property<double>("credito")
                        .HasColumnType("float");

                    b.Property<string>("dni")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<bool>("esAdmin")
                        .HasColumnType("bit");

                    b.Property<int>("intentosFallidos")
                        .HasColumnType("int");

                    b.Property<string>("mail")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<string>("name")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("id");

                    b.ToTable("Usuario", (string)null);

                    b.HasData(
                        new
                        {
                            id = 1,
                            apellido = "admin",
                            bloqueado = false,
                            credito = 50000.0,
                            dni = "10111222",
                            esAdmin = true,
                            intentosFallidos = 0,
                            mail = "admin@admin.com",
                            name = "admin",
                            password = "12345"
                        },
                        new
                        {
                            id = 2,
                            apellido = "perez",
                            bloqueado = false,
                            credito = 50000.0,
                            dni = "11222333",
                            esAdmin = false,
                            intentosFallidos = 0,
                            mail = "juan@juan.com",
                            name = "juan",
                            password = "12345"
                        },
                        new
                        {
                            id = 3,
                            apellido = "perez",
                            bloqueado = false,
                            credito = 10000.0,
                            dni = "33222111",
                            esAdmin = false,
                            intentosFallidos = 0,
                            mail = "luciana@luciana.com",
                            name = "luciana",
                            password = "12345"
                        },
                        new
                        {
                            id = 4,
                            apellido = "gomez",
                            bloqueado = false,
                            credito = 20000.0,
                            dni = "22333444",
                            esAdmin = false,
                            intentosFallidos = 0,
                            mail = "perdo@pedro.com",
                            name = "pedro ",
                            password = "12345"
                        });
                });

            modelBuilder.Entity("sistemaWEB.Models.Vuelo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("CiudadDestinoId")
                        .HasColumnType("int");

                    b.Property<int>("CiudadOrigenId")
                        .HasColumnType("int");

                    b.Property<string>("aerolinea")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("avion")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("capacidad")
                        .HasColumnType("int");

                    b.Property<double>("costo")
                        .HasColumnType("float");

                    b.Property<DateTime>("fecha")
                        .HasColumnType("datetime");

                    b.Property<int>("vendido")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("CiudadDestinoId");

                    b.HasIndex("CiudadOrigenId");

                    b.ToTable("Vuelo", (string)null);

                    b.HasData(
                        new
                        {
                            id = 1,
                            CiudadDestinoId = 2,
                            CiudadOrigenId = 1,
                            aerolinea = "AA",
                            avion = "Airbus",
                            capacidad = 30,
                            costo = 1000.0,
                            fecha = new DateTime(2023, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            vendido = 0
                        });
                });

            modelBuilder.Entity("sistemaWEB.Models.VueloUsuario", b =>
                {
                    b.Property<int>("idVuelo")
                        .HasColumnType("int");

                    b.Property<int>("idUsuario")
                        .HasColumnType("int");

                    b.Property<int>("cantidad")
                        .HasColumnType("int");

                    b.HasKey("idVuelo", "idUsuario");

                    b.HasIndex("idUsuario");

                    b.ToTable("vueloUsuarios");
                });

            modelBuilder.Entity("sistemaWEB.Models.Hotel", b =>
                {
                    b.HasOne("sistemaWEB.Models.Ciudad", "ubicacion")
                        .WithMany("listHoteles")
                        .HasForeignKey("idCiudad")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ubicacion");
                });

            modelBuilder.Entity("sistemaWEB.Models.HotelUsuario", b =>
                {
                    b.HasOne("sistemaWEB.Models.Hotel", "hotel")
                        .WithMany("hotelUsuario")
                        .HasForeignKey("idHotel")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sistemaWEB.Models.Usuario", "user")
                        .WithMany("hotelUsuario")
                        .HasForeignKey("idUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("hotel");

                    b.Navigation("user");
                });

            modelBuilder.Entity("sistemaWEB.Models.ReservaHotel", b =>
                {
                    b.HasOne("sistemaWEB.Models.Hotel", "miHotel")
                        .WithMany("listMisReservas")
                        .HasForeignKey("idHotel")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sistemaWEB.Models.Usuario", "miUsuario")
                        .WithMany("listMisReservasHoteles")
                        .HasForeignKey("idUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("miHotel");

                    b.Navigation("miUsuario");
                });

            modelBuilder.Entity("sistemaWEB.Models.ReservaVuelo", b =>
                {
                    b.HasOne("sistemaWEB.Models.Usuario", "miUsuario")
                        .WithMany("listMisReservasVuelo")
                        .HasForeignKey("idUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sistemaWEB.Models.Vuelo", "miVuelo")
                        .WithMany("listMisReservas")
                        .HasForeignKey("idVuelo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("miUsuario");

                    b.Navigation("miVuelo");
                });

            modelBuilder.Entity("sistemaWEB.Models.Vuelo", b =>
                {
                    b.HasOne("sistemaWEB.Models.Ciudad", "destino")
                        .WithMany("listVuelosDestino")
                        .HasForeignKey("CiudadDestinoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("sistemaWEB.Models.Ciudad", "origen")
                        .WithMany("listVuelosOrigen")
                        .HasForeignKey("CiudadOrigenId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("destino");

                    b.Navigation("origen");
                });

            modelBuilder.Entity("sistemaWEB.Models.VueloUsuario", b =>
                {
                    b.HasOne("sistemaWEB.Models.Usuario", "user")
                        .WithMany("vueloUsuarios")
                        .HasForeignKey("idUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sistemaWEB.Models.Vuelo", "vuelo")
                        .WithMany("vueloUsuarios")
                        .HasForeignKey("idVuelo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");

                    b.Navigation("vuelo");
                });

            modelBuilder.Entity("sistemaWEB.Models.Ciudad", b =>
                {
                    b.Navigation("listHoteles");

                    b.Navigation("listVuelosDestino");

                    b.Navigation("listVuelosOrigen");
                });

            modelBuilder.Entity("sistemaWEB.Models.Hotel", b =>
                {
                    b.Navigation("hotelUsuario");

                    b.Navigation("listMisReservas");
                });

            modelBuilder.Entity("sistemaWEB.Models.Usuario", b =>
                {
                    b.Navigation("hotelUsuario");

                    b.Navigation("listMisReservasHoteles");

                    b.Navigation("listMisReservasVuelo");

                    b.Navigation("vueloUsuarios");
                });

            modelBuilder.Entity("sistemaWEB.Models.Vuelo", b =>
                {
                    b.Navigation("listMisReservas");

                    b.Navigation("vueloUsuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
