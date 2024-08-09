using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using sistemaWEB.Models;

namespace sistemaWEB.Controllers
{
    public class VueloesController : Controller
    {
        private readonly MiContexto _context;

        public VueloesController(MiContexto context)
        {
            _context = context;
            _context.vuelos.Include(v => v.listPasajeros).Include(v => v.listMisReservas).Include(v => v.vueloUsuarios).Include(c => c.origen).Load();
            _context.vueloUsuarios.Load();
        }

        // GET: Vueloes
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.vuelos.Include(v => v.destino).Include(v => v.origen);
            return View(await miContexto.ToListAsync());
        }

        public IActionResult MisVuelosEnCualViaje()
        {
            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            usuarioActual.listMisReservasVuelo = new List<ReservaVuelo>();
            var listMisVuelo = from v in _context.vuelos
                                     join vu in _context.vueloUsuarios on v.id equals vu.idVuelo
                                     where vu.idUsuario == usuarioActual.id
                                     select new { v }.v;
            return View(listMisVuelo.ToList());
        }

        // GET: Vueloes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.vuelos == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos
                .Include(v => v.destino)
                .Include(v => v.origen)
                .FirstOrDefaultAsync(m => m.id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        // GET: Vueloes/Create
        public IActionResult Create()
        {
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre");
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre");
            return View();
        }

        // POST: Vueloes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,CiudadOrigenId,CiudadDestinoId,capacidad,vendido,costo,fecha,aerolinea,avion")] Vuelo vuelo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vuelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // GET: Vueloes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.vuelos.Include(x => x.origen).Include(x => x.destino) == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos.FindAsync(id);
            if (vuelo == null)
            {
                return NotFound();
            }
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // POST: Vueloes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,CiudadOrigenId,CiudadDestinoId,capacidad,vendido,costo,fecha,aerolinea,avion")] Vuelo vuelo)
        {
            if (id != vuelo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vuelo.origen = _context.ciudades.FirstOrDefault(x => x.id == vuelo.CiudadOrigenId);
                    vuelo.destino = _context.ciudades.FirstOrDefault(x => x.id == vuelo.CiudadDestinoId);
                    string resultado = (this.modificarVuelo(id, vuelo.origen, vuelo.destino, vuelo.capacidad, vuelo.costo, vuelo.fecha, vuelo.aerolinea, vuelo.avion));
                    switch (resultado)
                    {
                        case "exito":
                            //  MessageBox.Show("Vuelo modificado exitosamente");
                            break;
                        case "capacidad":
                            //  MessageBox.Show("La capacidad es menor a la cantidad de personas que reservaron el vuelo");
                            break;
                        case "error":
                            //   MessageBox.Show("Ocurrió un problema al querer modificar el vuelo");
                            break;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VueloExists(vuelo.id))
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
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // GET: Vueloes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vuelos == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos
                .Include(v => v.destino)
                .Include(v => v.origen)
                .FirstOrDefaultAsync(m => m.id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        // POST: Vueloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vuelos == null)
            {
                return Problem("Entity set 'MiContexto.vuelos'  is null.");
            }
            var vueloAEliminar = await _context.vuelos.FindAsync(id);
            if (vueloAEliminar != null)
            {
                if (vueloAEliminar.fecha > DateTime.Now)
                {
                    vueloAEliminar.listMisReservas.ForEach(r => r.miUsuario.credito += r.pagado);
                }
                _context.vuelos.Remove(vueloAEliminar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VueloExists(int id)
        {
            return (_context.vuelos?.Any(e => e.id == id)).GetValueOrDefault();
        }


        #region funciones vuelos

        public string modificarVuelo(int id, Ciudad cOrigen, Ciudad cDestino, int capacidad, double costo, DateTime fecha, string aerolinea, string avion)
        {
            try
            {
                Vuelo vueloModificado = _context.vuelos.Where(v => v.id == id).FirstOrDefault();

                if (vueloModificado != null)
                {
                    int disponibilidad = vueloModificado.capacidad - vueloModificado.vendido;
                    if (capacidad >= disponibilidad)
                    {
                        vueloModificado.origen = cOrigen;
                        vueloModificado.destino = cDestino;
                        vueloModificado.costo = costo;
                        vueloModificado.fecha = fecha;
                        vueloModificado.aerolinea = aerolinea;
                        vueloModificado.avion = avion;
                        vueloModificado.capacidad = capacidad;
                        _context.vuelos.Update(vueloModificado);
                        _context.SaveChangesAsync();
                        return "exito";
                    }
                    {
                        return "capacidad";
                    }
                }

            }
            catch (Exception e)
            {
                return "error";
            }

            return "error";
        }

        public int obtenerNombreCiudad(string nombre)
        {
            Ciudad ciudad = _context.ciudades.FirstOrDefault(ciudad => ciudad.nombre == nombre);
            return ciudad != null ? ciudad.id : -1;
        }

        public bool eliminarVuelo(int id)
        {
            DateTime fechaActual = DateTime.Now;
            try
            {
                bool salida = false;
                Vuelo vueloAEliminar = _context.vuelos.Where(v => v.id == id).FirstOrDefault();
                if (vueloAEliminar != null)
                {
                    if (vueloAEliminar.fecha > fechaActual)
                    {
                        vueloAEliminar.listMisReservas.ForEach(r => r.miUsuario.credito += r.pagado);
                    }
                    _context.vuelos.Remove(vueloAEliminar);
                    salida = true;
                }

                if (salida)
                    _context.SaveChanges();
                return salida;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        #endregion


    }
}
