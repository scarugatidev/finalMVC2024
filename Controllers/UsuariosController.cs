using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using sistemaWEB.Models;
using sistemaWEB.Models.Business.ReservaHotel;
using static System.Collections.Specialized.BitVector32;

namespace sistemaWEB.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly MiContexto _context;
        public const string SessionKeyName = "_sessionNameUsuario";
        public const string SessionKeyAge = "_SessionUsuarioAge";

        public UsuariosController(MiContexto context)
        {
            _context = context;
            _context.usuarios.Include(x => x.listMisReservasVuelo).Load();
        }

        public IActionResult UsuarioSimple(string mensaje, int id)
        {
            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            Usuario usuario = _context.usuarios.FirstOrDefault(x => x.id == usuarioActual.id);

            if (id == 2)
            {
                ViewBag.ErrorCredito = mensaje;
            }
            if (id == 3)
            {
                ViewBag.ErrorPass = mensaje;
            }
            if (id == 1)
            {
                ViewBag.ErrorDatPersonales = mensaje;
            }

            if (!string.IsNullOrEmpty(mensaje))
            {
                ViewBag.ErrorCredito = mensaje;
            }


            return View(usuario);
        }


        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Loguin()
        {
            return View();
        }
        // Loguin: Usuarios
        [HttpPost]
        public async Task<IActionResult> Loguin([Bind("mail,password")] Usuario usuario)
        {

            string resp = this.login(usuario.password, usuario.mail);
            switch (resp)
            {
                case "OK":
                    return RedirectToAction("Index", "Menu");
                case "BLOQUEADO":
                    ViewBag.Error = "usuario bloqueado";
                    break;
                case "MAILERROR":
                    ViewBag.Error = "usuario o contraseña incorrectos";
                    break;
                case "INGRESARDATOS":
                    ViewBag.Error = "Debe ingresar un usuario y contraseña!";
                    break;
                case "FALTAUSUARIO":
                    ViewBag.Error = "No existe el usuario";
                    break;
                default:
                    break;
            }
            return View();
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return _context.usuarios != null ?
                        View(await _context.usuarios.ToListAsync()) :
                        Problem("Entity set 'MiContexto.usuarios'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {

                if ((this.existeUsuarioConDniOMail(usuario.dni, usuario.mail)))
                {
                    //   MessageBox.Show("ya existe un usuario con el mismo mail o dni.");
                    //true
                }
                else
                {
                    if (usuario.name.Length >= 3 && usuario.apellido.Length >= 3 && usuario.dni.Length == 8 && usuario.mail.Contains("@"))
                    {
                        //Dni, Nombre, apellido, Mail,pass, EsADM, Bloqueado);
                        await this.agregarUsuarioContextoAsync(usuario.dni, usuario.name, usuario.apellido, usuario.mail, usuario.password, usuario.esAdmin, usuario.bloqueado);

                        //  MessageBox.Show("Agregado con éxito");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // MessageBox.Show("Problemas al agregar");
                    }
                }

                //_context.Add(usuario);
                //await _context.SaveChangesAsync();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> agregarCredito([Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                if (this.AgregarCreditoContexto(usuarioActual.id, usuario.credito))
                {
                    mensaje = "Modificado con éxito";
                }
                else
                {
                    mensaje = "Problemas al modificar";
                }
            }
            return RedirectToAction(nameof(UsuarioSimple), new { mensaje = mensaje, id = 2 });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> modificarPassword([Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                if (await this.modificarUsuariocontexto(usuarioActual.id, usuarioActual.name, usuarioActual.apellido, usuarioActual.dni, usuarioActual.mail, usuario.password, usuarioActual.esAdmin, usuarioActual.bloqueado))
                {
                    mensaje = "Modificado con éxito";
                }

                else
                {
                    mensaje = "Problemas al modificar";
                }
            }
            return RedirectToAction(nameof(UsuarioSimple), new { mensaje = mensaje, id = 3 });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifiarUsuarioSimple([Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            string mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                if (await this.modificarUsuariocontexto(usuarioActual.id, usuario.name, usuario.apellido, usuario.dni, usuario.mail, usuario.password, usuarioActual.esAdmin, usuarioActual.bloqueado))
                {
                    mensaje = "Modificado con éxito";
                }
                else
                {
                    mensaje = "Modificado con éxito";
                }

            }
            return RedirectToAction(nameof(UsuarioSimple), new { mensaje = mensaje, id = 1 });
        }



        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(usuario.name) && !string.IsNullOrEmpty(usuario.apellido) &&
                    !string.IsNullOrEmpty(usuario.dni) && !string.IsNullOrEmpty(usuario.mail))
                    {
                        await this.modificarUsuariocontexto(id, usuario.name, usuario.apellido, usuario.dni, usuario.mail, usuario.password, usuario.esAdmin, usuario.bloqueado);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MiContexto.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                await this.eliminarUsuarioContext(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.usuarios?.Any(e => e.id == id)).GetValueOrDefault();
        }


        #region metodos privados Loguin
        public string login(string? _contraseña, string? _mail)
        {
            if (_contraseña != null && _mail != "" && _contraseña != null && _mail != "")
            {
                Usuario usuarioSeleccionados = _context.usuarios.Where(u => u.mail == _mail).FirstOrDefault();
                return validacionEstadoUsuario(usuarioSeleccionados, _mail, _contraseña);
            }
            else
            {
                return "INGRESARDATOS";
            }
        }

        private string validacionEstadoUsuario(Usuario? usuarioSeleccionados, string mailInput, string Inputpass)
        {
            return this.iniciarSesion(usuarioSeleccionados, mailInput, Inputpass);
        }

        public string iniciarSesion(Usuario? usuarioSeleccionados, string inputMail, string inputpass)
        {
            string codigoReturn;
            if (usuarioSeleccionados == null)
            {

                codigoReturn = "FALTAUSUARIO";
            }
            else if (usuarioSeleccionados.mail.Trim().Equals(inputMail) && usuarioSeleccionados.password.Trim() == inputpass && !usuarioSeleccionados.bloqueado)
            {
                codigoReturn = "OK";
                //this.usuarioActual = usuarioSeleccionados;
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                {
                    usuarioSeleccionados.listMisReservasVuelo = null;
                    Helper.SessionExtensions.Set(HttpContext.Session, "usuarioActual", usuarioSeleccionados);
                    var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
                }
            }
            else
            {
                usuarioSeleccionados.intentosFallidos++;
                this.IngresarIntentosFallidosContext(usuarioSeleccionados);
                //this.modificarUsuarioActual(usuarioSeleccionados);
                if (usuarioSeleccionados.intentosFallidos >= 3)
                {
                    IngresarUsuarioBloqueadoContext(usuarioSeleccionados);
                    usuarioSeleccionados.bloqueado = true;
                    codigoReturn = "BLOQUEADO";

                }
                else
                {
                    codigoReturn = "MAILERROR";
                }
            }

            return codigoReturn;
        }

        public void IngresarIntentosFallidosContext(Usuario usu)
        {
            try
            {
                Usuario usuario = _context.usuarios.FirstOrDefault(x => x.id == usu.id);
                usuario.intentosFallidos = usuario.intentosFallidos;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void IngresarUsuarioBloqueadoContext(Usuario usuario)
        {
            try
            {
                //Usuario usuario = this.getUsuarioActual();
                usuario.bloqueado = true;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void volverIntentosFallidosCeroContext(Usuario usuario)
        {
            try
            {
                // Usuario usuario = this.getUsuarioActual();
                usuario.intentosFallidos = 0;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region usuario

        //verificar si ya existe un usuario con ese mail o dni
        //devuelve true si encuentra
        public bool existeUsuarioConDniOMail(string dni, string mail)
        {
            return _context.usuarios.Any(u => u.dni == dni || u.mail == mail);
        }

        public async Task<bool> agregarUsuarioContextoAsync(string Dni, string Nombre, string apellido, string Mail, string Password, bool EsADM, bool Bloqueado)
        {
            //comprobación para que no me agreguen usuarios con DNI duplicado
            bool esValido = true;

            List<Usuario> listUsuarios = _context.usuarios.ToList();

            foreach (Usuario u in listUsuarios)
            {
                if (u.dni == Dni)
                {
                    esValido = false;
                }
            }
            try
            {
                if (esValido)
                {
                    Usuario nuevo = new Usuario { dni = Dni, name = Nombre, apellido = apellido, mail = Mail, password = Password, esAdmin = EsADM, bloqueado = Bloqueado };
                    _context.Add(nuevo);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> modificarUsuariocontexto(int Id, string Nombre, string Apellido, string Dni, string Mail, string pass, bool admin, bool bloqueado)
        {
            //busco usuario y lo asocio a la variable
            Usuario u = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
            if (u != null)
            {
                try
                {
                    u.name = Nombre;
                    u.apellido = Apellido;
                    u.dni = Dni.ToString();
                    u.mail = Mail;
                    u.password = pass;
                    u.esAdmin = admin;
                    u.bloqueado = bloqueado;
                    _context.usuarios.Update(u);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 1 registro
                return false;
            }
        }
        public async Task<bool> eliminarUsuarioContext(int Id)
        {
            Usuario u = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
            if (u != null)
            {
                try
                {
                    u.listMisReservasVuelo.Where(rv => rv.idUsuario == Id).ToList();
                    foreach (ReservaVuelo rv in u.listMisReservasVuelo)
                    {
                        int cantReservas = (int)(rv.pagado / rv.miVuelo.costo);
                        rv.miVuelo.vendido -= cantReservas;
                        _context.vuelos.Update(rv.miVuelo);
                    }
                    _context.usuarios.Remove(u);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                    throw;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 
                return false;
            }
        }

        public bool AgregarCreditoContexto(int id, double monto)
        {
            try
            {
                Usuario u = _context.usuarios.Where(u => u.id == id).FirstOrDefault();
                if (u != null)

                {
                    u.credito += monto;
                    _context.usuarios.Update(u);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
