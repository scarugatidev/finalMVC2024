using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaWEB.Models;

namespace sistemaWEB.Controllers
{
    public class HotelsController : Controller
    {
        private readonly MiContexto _context;

        public HotelsController(MiContexto context)
        {
            _context = context;
        }

        // GET: Hotels correcto
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.hoteles.Include(h => h.ubicacion);
            return View(await miContexto.ToListAsync());
        }
        //correctp
        public IActionResult MisHotelesQueVisite()
        {
            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            _context.ciudades.Load();
            IEnumerable misHoteles = this.misHotelesQueVisiteContext(usuarioActual.id);
            return View(misHoteles);
        }

        // GET: Hotels/Details/5 correcto
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.hoteles == null)
            {
                return NotFound();
            }

            var hotel = await _context.hoteles
                .Include(h => h.ubicacion)
                .FirstOrDefaultAsync(m => m.id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create correcto
        public IActionResult Create()
        {
            ViewData["idCiudad"] = new SelectList(_context.ciudades, "id", "nombre");
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to. correcto
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,capacidad,costo,nombre,idCiudad")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idCiudad"] = new SelectList(_context.ciudades, "id", "nombre", hotel.idCiudad);
            return View(hotel);
        }

        // GET: Hotels/Edit/5 correcto
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.hoteles == null)
            {
                return NotFound();
            }

            var hotel = await _context.hoteles.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            ViewData["idCiudad"] = new SelectList(_context.ciudades, "id", "nombre", hotel.idCiudad);
            return View(hotel);
        }

        // POST: Hotels/Edit/5 correcto
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,capacidad,costo,nombre,idCiudad")] Hotel hotel)
        {
            if (id != hotel.id)
            {
                return NotFound();
            }

            Hotel hotelAModificar = _context.hoteles.FirstOrDefault(hotel => hotel.id == id);
            if (ModelState.IsValid)
            {
                try
                {
                    hotelAModificar.capacidad = hotel.capacidad;
                    hotelAModificar.idCiudad = hotel.idCiudad;
                    hotelAModificar.id = hotel.id;
                    hotelAModificar.costo = hotel.costo;
                    hotelAModificar.nombre = hotel.nombre;
                    _context.Update(hotelAModificar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.id))
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
            ViewData["idCiudad"] = new SelectList(_context.ciudades, "id", "nombre", hotel.idCiudad);
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.hoteles == null)
            {
                return NotFound();
            }

            var hotel = await _context.hoteles
                .Include(h => h.ubicacion)
                .FirstOrDefaultAsync(m => m.id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.hoteles == null)
            {
                return Problem("Entity set 'MiContexto.hoteles'  is null.");
            }
            var hotel = await _context.hoteles.FindAsync(id);
            if (hotel != null)
            {
                await this.eliminarHotel(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return (_context.hoteles?.Any(e => e.id == id)).GetValueOrDefault();
        }


        #region hotels metodos privados

        public async Task<bool> eliminarHotel(int idHotel)
        {

            try
            {
                var hotelAEliminar = _context.hoteles.FirstOrDefault(hotel => hotel.id == idHotel);
                DateTime fechaActual = DateTime.Now;
                if (hotelAEliminar != null)
                {
                    foreach (var reservaHotel in hotelAEliminar.listMisReservas.Where(u => u.fechaDesde > fechaActual))
                    {
                        reservaHotel.miUsuario.credito += reservaHotel.pagado;
                        _context.reservaHoteles.Remove(reservaHotel);
                        _context.usuarios.Update(reservaHotel.miUsuario);
                    }

                    _context.hoteles.Remove(hotelAEliminar);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; //no se encontró el id del hotel
            }
            catch (Exception ex)
            {
                return false; //excepcion al eliminar el hotel
            }
        }

        public List<Hotel>? misHotelesQueVisiteContext(Int32 idUsuario)
        {
            try
            {
                var listHoteles = from hu in _context.HotelUsuario
                                  join h in _context.hoteles on hu.idHotel equals h.id
                                  where hu.idUsuario == idUsuario
                                  select new { h }.h;
                return listHoteles.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


    }
}
