using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sistemaWEB.Models;
using sistemaWEB.Models.Business.Menu;

namespace sistemaWEB.Controllers
{
    public class MenuController : Controller
    {
        // GET: MenuController
        public ActionResult Index()
        {
            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            return View(new BusinessMenu() { flagPermisos = usuarioActual.esAdmin });
        }

        // GET: MenuController/Details/5
        public ActionResult CerrarSesion()
        {
            Helper.SessionExtensions.delete<Usuario>(HttpContext.Session, "usuarioActual");
            return RedirectToAction("Loguin", "Usuarios");
        }

    }
}
