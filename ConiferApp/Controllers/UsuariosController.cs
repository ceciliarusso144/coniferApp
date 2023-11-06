using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ConiferApp.Models;

namespace ConiferApp.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly DbUsuarios db = new DbUsuarios();

        public ActionResult Index()
        {
            return View(db.Usuarios.ToList());
        }

        [HttpPost]
        public ActionResult Index(string filtro)
        {
            var resultado = db.Usuarios.Where(u => u.Legajo.ToString().Contains(filtro)).OrderBy(u => u.Apellido).ToList();
            ViewData["Resultado"] = "buscando";
            return View(resultado);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUsuario,Apellido,Nombre,DNI,Legajo,Clave,Activo")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                usuarios.Activo = true;
                //usuarios.Clave = AccesoController.ConvertirSha256(usuarios.Clave);
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuarios);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUsuario,Apellido,Nombre,DNI,Legajo,Clave,Activo")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                usuarios.Activo = true;
                //usuarios.Clave = AccesoController.ConvertirSha256(usuarios.Clave);
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuarios);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuarios = db.Usuarios.Find(id);
            usuarios.Activo = false;
            db.Entry(usuarios).State = EntityState.Modified;
            //db.Usuarios.Remove(usuarios);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
