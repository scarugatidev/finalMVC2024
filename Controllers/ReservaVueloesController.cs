using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaWEB.Models;
using sistemaWEB.Models.Business.ReservaHotel;
using sistemaWEB.Models.Business.ReservaVuelo;

namespace sistemaWEB.Controllers
{
    public class ReservaVueloesController : Controller
    {
        private readonly MiContexto _context;

        public ReservaVueloesController(MiContexto context)
        {
            _context = context;
        }

        // GET: ReservaVueloes
        public IActionResult MisReservasVuelo()
        {

            _context.reservaVuelos.Load();
            _context.vuelos.Load();
            _context.usuarios.Load();
            _context.ciudades.Load();

            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            usuarioActual.listMisReservasVuelo = new List<ReservaVuelo>();
            var listMiReservaVuelo = from rv in _context.reservaVuelos
                                     join u in _context.usuarios on rv.idUsuario equals u.id
                                     where rv.idUsuario == usuarioActual.id
                                     select new { rv }.rv;

            listMiReservaVuelo.ToList();
            usuarioActual.listMisReservasVuelo = listMiReservaVuelo.ToList();
            return View(usuarioActual.listMisReservasVuelo);
        }




        // GET: ReservaVueloes
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.reservaVuelos.Include(r => r.miUsuario).Include(r => r.miVuelo);
            return View(await miContexto.ToListAsync());
        }

        // GET: ReservaVueloes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos
                .Include(r => r.miUsuario)
                .Include(r => r.miVuelo)
                .FirstOrDefaultAsync(m => m.idReservaVuelo == id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }

            return View(reservaVuelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConsulta([Bind("cantPasajeros,CiudadOrigen,CiudadDestino, fecha")] BusinessReservaVuelo reservaVuelo)
        {
            return RedirectToAction(nameof(Create), "ReservaVueloes", new RouteValueDictionary(reservaVuelo));
        }


        // GET: ReservaVueloes/Create
        public async Task<IActionResult> Create(BusinessReservaVuelo reservaVuelo)
        {

            if (reservaVuelo.idVuelo != 0 && reservaVuelo.cantPasajeros != 0)
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                string resultado = await this.comprarVuelo(reservaVuelo.idVuelo, usuarioActual, reservaVuelo.cantPasajeros);
                switch (resultado)
                {
                    case "yaCompro":
                        ViewBag.Error += "Ya compraste este vuelo. ";
                        break;
                    case "exito":
                        Vuelo vueloSeleccionado = _context.vuelos.FirstOrDefault(v => v.id == reservaVuelo.idVuelo);
                        int asientosDisponibles = vueloSeleccionado.capacidad;
                        //  dataGridView1.Rows[rowIndex].Cells["Cantidad"].Value = asientosDisponibles -= cantidad;
                        ViewBag.Error += "Reserva realiza con éxito";
                        break;

                    case "sinSaldo":
                        ViewBag.Error += "No tienes suficiente crédito para realizar la compra";
                        break;
                    case "error":
                        ViewBag.Error += "Hubo un error inesperado, volvé a intentarlo ";
                        break;
                }

            }

            Ciudad ciudadOrigen = _context.ciudades.FirstOrDefault(ciudad => ciudad.id == reservaVuelo.CiudadOrigen);
            Ciudad ciudadDestino = _context.ciudades.FirstOrDefault(ciudad => ciudad.id == reservaVuelo.CiudadDestino);


            if (ciudadOrigen != null && ciudadDestino != null && reservaVuelo.fecha != null && reservaVuelo.cantPasajeros != 0)
            {
                List<Vuelo> vuelosEncontrados = this.buscarVuelos(ciudadOrigen, ciudadDestino, reservaVuelo.fecha, reservaVuelo.cantPasajeros);

                if (vuelosEncontrados.Count >= 1)
                {
                    reservaVuelo.vuelos = new List<BusinessVuelos>();
                    foreach (var itemVuelo in vuelosEncontrados)
                    {
                        double costoTotal = itemVuelo.costo * reservaVuelo.cantPasajeros;
                        BusinessVuelos vuelos = new BusinessVuelos()
                        {
                            id = itemVuelo.id,
                            aerolinea = itemVuelo.aerolinea,
                            avion = itemVuelo.avion,
                            capacidad = itemVuelo.capacidad,
                            costoTotal = Convert.ToString(costoTotal),
                            fechaFormateada = itemVuelo.fecha,
                            vueloDestinoNombre = itemVuelo.destino.nombre,
                            vueloOrigenNombre = itemVuelo.origen.nombre

                        };
                        reservaVuelo.vuelos.Add(vuelos);
                    }
                }
                else
                {
                    reservaVuelo.vuelos = new List<BusinessVuelos>();
                    ViewBag.Error += "No se encontraron vuelos. ";
                }
            }
            else
            {
                if (reservaVuelo.cantPasajeros == 0)
                {
                    ViewBag.Error += "Debe ingresar la cantidad de pasajeros para realizar la consulta";
                }

                reservaVuelo.vuelos = new List<BusinessVuelos>();
            }
            ViewData["CiudadDestino"] = new SelectList(_context.ciudades, "id", "nombre");
            ViewData["CiudadOrigen"] = new SelectList(_context.ciudades, "id", "nombre");
            return View(new BusinessReservaVuelo() { vuelos = reservaVuelo.vuelos, fecha = DateTime.Now.Date });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar([Bind("cantPasajeros,CiudadOrigen,CiudadDestino,fecha")] BusinessReservaVuelo BusinessReservaVuelo, [Bind("id,vueloOrigenNombre,vueloDestinoNombre,capacidad,costoTotal,fechaFormateada,aerolinea,avion")] BusinessVuelos BusinessVuelos)
        {

            BusinessReservaVuelo.idVuelo = BusinessVuelos.id;
            return RedirectToAction(nameof(Create), new RouteValueDictionary(BusinessReservaVuelo));
        }

        // POST: ReservaVueloes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idReservaVuelo,idVuelo,idUsuario,pagado")] ReservaVuelo reservaVuelo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservaVuelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos.FindAsync(id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Edit/5
        public async Task<IActionResult> EditMiReservaVuelo(int? id)
        {
            _context.vuelos.Load();
            _context.usuarios.Load();
            _context.ciudades.Load();

            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var resp = Helper.SessionExtensions.Get<string>(HttpContext.Session, "errorMensajeEditReservaVuelo");
            if (resp != null)
            {
                ViewBag.errorMensajeEditReservaVuelo = resp;
            }

            var reservaVuelo = await _context.reservaVuelos.FindAsync(id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            Helper.SessionExtensions.delete<string>(HttpContext.Session, "errorMensajeEditReservaVuelo");
            return View(reservaVuelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMiReservaVuelo(int id, [Bind("idReservaVuelo")] ReservaVuelo reservaVuelo, int cantidad, int idVuelo)
        {
            if (id != reservaVuelo.idReservaVuelo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    double nuevoCosto = this.CalcularNuevoCosto(idVuelo, cantidad);
                    string resultado = (this.modificarReservaVuelo(idVuelo, reservaVuelo.idReservaVuelo, cantidad, nuevoCosto));
                    Helper.SessionExtensions.Set(HttpContext.Session, "errorMensajeEditReservaVuelo", resultado);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaVueloExists(reservaVuelo.idReservaVuelo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EditMiReservaVuelo));
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // POST: ReservaVueloes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idReservaVuelo,idVuelo,idUsuario,pagado")] ReservaVuelo reservaVuelo)
        {
            if (id != reservaVuelo.idReservaVuelo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservaVuelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaVueloExists(reservaVuelo.idReservaVuelo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos
                .Include(r => r.miUsuario)
                .Include(r => r.miVuelo)
                .FirstOrDefaultAsync(m => m.idReservaVuelo == id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }

            var erroMensajeDeleteReservaVuelos = Helper.SessionExtensions.Get<string>(HttpContext.Session, "erroMensajeDeleteReservaVuelos");
            if (erroMensajeDeleteReservaVuelos != null)
            {
                ViewBag.erroMensajeDeleteReservaVuelos = erroMensajeDeleteReservaVuelos;
            }
            Helper.SessionExtensions.delete<string>(HttpContext.Session, "erroMensajeDeleteReservaVuelos");
            return View(reservaVuelo);
        }

        // POST: ReservaVueloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reservaVuelos == null)
            {
                return Problem("Entity set 'MiContexto.reservaVuelos'  is null.");
            }
            var reservaVuelo = await _context.reservaVuelos.FindAsync(id);
            var resp = string.Empty;
            if (reservaVuelo != null)
            {
                resp = this.eliminarReservaVuelo(id);


                if (resp == "Fecha" || resp == "error")
                {
                    Helper.SessionExtensions.Set(HttpContext.Session, "erroMensajeDeleteReservaVuelos", resp);
                    return RedirectToAction(nameof(Delete));
                }
            }
            return RedirectToAction(nameof(MisReservasVuelo));
        }

        private bool ReservaVueloExists(int id)
        {
            return (_context.reservaVuelos?.Any(e => e.idReservaVuelo == id)).GetValueOrDefault();
        }


        #region funciones reserva vuelos


        public List<Vuelo> buscarVuelos(Ciudad origen, Ciudad destino, DateTime? fecha, int cantidadPax)
        {
            // && vuelo.fecha.Date == fecha.Date
            var vuelosDisponibles = _context.vuelos.Where(vuelo => vuelo.origen.nombre == origen.nombre && vuelo.destino.nombre == destino.nombre && vuelo.capacidad >= cantidadPax + vuelo.vendido).ToList();
            return vuelosDisponibles;
        }


        public async Task<string> comprarVuelo(int vueloId, Usuario usuarioActual, int cantidad)
        {
            Vuelo vuelo = _context.vuelos.Where(v => v.id == vueloId).FirstOrDefault();

            if (_context.vueloUsuarios.Any(vu => vu.idVuelo == vueloId && vu.idUsuario == usuarioActual.id))
            {
                return "yaCompro";
            }

            if (vuelo != null && cantidad > 0 && cantidad <= vuelo.capacidad - vuelo.vendido)
            {
                double costoTotal = vuelo.costo * cantidad;
                if (usuarioActual.credito >= costoTotal)
                {
                    try
                    {
                        usuarioActual.credito -= costoTotal;
                        vuelo.vendido += cantidad;
                        vuelo.listPasajeros.Add(usuarioActual);

                        ReservaVuelo reserva = new ReservaVuelo();
                        reserva.idVuelo = vuelo.id;
                        reserva.idUsuario = usuarioActual.id;
                        reserva.pagado = costoTotal;
                        _context.reservaVuelos.Add(reserva);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return "error";
                    }

                    try
                    {
                        await vincularVueloUsuarios(vueloId, usuarioActual.id, cantidad);
                    }
                    catch (Exception ex)
                    {
                        return "error";
                    }

                    return "exito";
                }
                return "sinSaldo";

            }
            return "error";

        }


        public async Task<bool> vincularVueloUsuarios(int vueloId, int usuarioId, int cant)
        {
            try
            {
                Vuelo vu = _context.vuelos.Where(v => v.id == vueloId).FirstOrDefault();
                Usuario us = _context.usuarios.Where(u => u.id == usuarioId).FirstOrDefault();
                VueloUsuario vueloUsuarioSelected = _context.vueloUsuarios.Where(vus => vus.idUsuario == usuarioId && vus.idVuelo == vueloId).FirstOrDefault();
                if (us != null && vu != null && vueloUsuarioSelected != null)
                {
                    us.listVuelosTomados.Add(vu);
                    _context.usuarios.Update(us);
                    await _context.SaveChangesAsync();

                    vueloUsuarioSelected.user = null;
                    vueloUsuarioSelected.vuelo = null;
                    vueloUsuarioSelected.cantidad = cant;
                    _context.vueloUsuarios.Update(vueloUsuarioSelected);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    vueloUsuarioSelected = new VueloUsuario();
                    {
                        vueloUsuarioSelected.idVuelo = vueloId;
                        vueloUsuarioSelected.idUsuario = usuarioId;
                        vueloUsuarioSelected.cantidad = cant;
                    }
                    vueloUsuarioSelected.user = null;
                    vueloUsuarioSelected.vuelo = null;
                    _context.vueloUsuarios.Add(vueloUsuarioSelected);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public double CalcularNuevoCosto(int vueloId, int nuevaCantidad)
        {

            double costoBase = ObtenerCostoBase(vueloId);
            double nuevoCosto = costoBase * nuevaCantidad;
            return nuevoCosto;
        }

        public double ObtenerCostoBase(int vueloId)
        {
            Vuelo vuelo = _context.vuelos.FirstOrDefault(v => v.id == vueloId);

            if (vuelo != null)
            {
                return vuelo.costo;
            }
            else
            {

                return 0;
            }
        }





        public string modificarReservaVuelo(int idVuelo, int idReserva, int cantidad, double costo)
        {
            DateTime fechaActual = DateTime.Now;
            try
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                ReservaVuelo rv = _context.reservaVuelos.Where(rv => rv.idReservaVuelo == idReserva).FirstOrDefault();
                Vuelo v = rv.miVuelo;
                Usuario u = _context.usuarios.FirstOrDefault(x => x.id == usuarioActual.id);

                VueloUsuario vueloUsuarioSelected = _context.vueloUsuarios.Where(vus => vus.idUsuario == usuarioActual.id && vus.idVuelo == idVuelo).FirstOrDefault();
                if (rv != null)
                {
                    if (v.fecha.Date >= fechaActual.Date)
                    {

                        int cantReservas = vueloUsuarioSelected.cantidad;

                        if (cantidad > cantReservas)
                        {
                            //Calculo la diferencia
                            int diferencia = cantidad - cantReservas;

                            double nuevoMonto = 0;
                            if (costo != rv.pagado)
                                nuevoMonto = diferencia * v.costo;
                            else
                                nuevoMonto = costo;

                            int disponibilidad = v.capacidad - v.vendido;
                            if (disponibilidad > diferencia)
                            {

                                //verifico si tiene credito
                                if (u.credito > nuevoMonto)
                                {
                                    //le cobro la diferencia 
                                    rv.miUsuario.credito = rv.miUsuario.credito - nuevoMonto;
                                    //actualizo el valor pagado de la nueva reserva 
                                    rv.pagado = rv.pagado + nuevoMonto;
                                    //sumo vendido de la diferencia a vuelo
                                    v.vendido += diferencia;
                                    vueloUsuarioSelected.cantidad += diferencia;
                                    _context.SaveChanges();
                                    return "reservaModificada";
                                }
                                else
                                {
                                    return "credito";
                                    //el usuario no tiene credito suficiente
                                }
                            }
                            else
                            {
                                return "capacidad";
                            }
                        }
                        else
                        {   //si la diferencia es menor
                            int diferencia = cantReservas - cantidad;
                            double nuevoMonto = diferencia * v.costo;
                            if (nuevoMonto > 0)
                            {
                                //devuelvo el dinero al usuario
                                rv.miUsuario.credito = rv.miUsuario.credito + nuevoMonto;
                                //actualizo el valor pagado en reserva
                                rv.pagado = rv.pagado - nuevoMonto;
                                //resto vendido a vuelo
                                v.vendido -= diferencia;
                                vueloUsuarioSelected.cantidad -= diferencia;
                                _context.SaveChanges();
                                return "reservaModificada";
                            }
                            if (nuevoMonto == 0)
                            {
                                return "nomodifica";
                            }
                            else
                            {
                                return "error";
                            }

                        }
                    }
                    else
                    {
                        return "fecha";
                    }

                }
            }
            catch (Exception e)
            {
                return "error";
            }

            return "error";
        }



        public string eliminarReservaVuelo(int reservaVueloId)
        {
            try
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                ReservaVuelo reservaVuelo = _context.reservaVuelos.Include(u => u.miUsuario).Where(rv => rv.idReservaVuelo == reservaVueloId).First();
                Vuelo v = _context.vuelos.FirstOrDefault(x => x.id == reservaVuelo.idVuelo);

                VueloUsuario vueloUsuarioSelected = _context.vueloUsuarios.Include(x => x.user).Where(vus => vus.idUsuario == usuarioActual.id && vus.idVuelo == reservaVuelo.idVuelo).FirstOrDefault();

                if (reservaVuelo != null)
                {
                    DateTime fechaActual = DateTime.Now;

                    if (v.fecha.Date >= fechaActual.Date)
                    {
                        double costoTotalReserva = reservaVuelo.pagado;
                        reservaVuelo.miUsuario.credito += costoTotalReserva;
                        //calculo la cant de reservas para luego eliminarlo de la lista de reservas del vuelo y del usuario
                        int cantReservas = (int)(reservaVuelo.pagado / v.costo);
                        v.vendido -= cantReservas;
                        v.listMisReservas.Remove(reservaVuelo);
                        reservaVuelo.miUsuario.listMisReservasVuelo.Remove(reservaVuelo);
                        //elimino la relacion vuelo usuario
                        if (vueloUsuarioSelected != null)
                            _context.vueloUsuarios.Remove(vueloUsuarioSelected);
                        //
                        _context.reservaVuelos.Remove(reservaVuelo);
                        _context.SaveChanges();
                        return "ReservaEliminada";
                    }
                    else
                    {
                        return "Fecha";
                    }
                }
                return "ok";
            }
            catch (Exception e)
            {
                return "error";
            }

        }

        #endregion

    }
}
