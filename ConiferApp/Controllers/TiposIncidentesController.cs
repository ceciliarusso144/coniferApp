using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ConiferApp.Models;

namespace ConiferApp.Controllers
{
    [Authorize]
    public class TiposIncidentesController : Controller
    {
        private DbTiposIncidentes db = new DbTiposIncidentes();

        // GET: TiposIncidentes
        public ActionResult Index()
        {
            return View(db.TiposIncidentes.ToList());
        }

        // GET: TiposIncidentes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposIncidentes tiposIncidentes = db.TiposIncidentes.Find(id);
            if (tiposIncidentes == null)
            {
                return HttpNotFound();
            }
            return View(tiposIncidentes);
        }

        // GET: TiposIncidentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TiposIncidentes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Vigente")] TiposIncidentes tiposIncidentes)
        {
            if (ModelState.IsValid)
            {
                tiposIncidentes.Vigente = true;
                db.TiposIncidentes.Add(tiposIncidentes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tiposIncidentes);
        }

        // GET: TiposIncidentes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposIncidentes tiposIncidentes = db.TiposIncidentes.Find(id);
            if (tiposIncidentes == null)
            {
                return HttpNotFound();
            }
            return View(tiposIncidentes);
        }

        // POST: TiposIncidentes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Vigente")] TiposIncidentes tiposIncidentes)
        {
            if (ModelState.IsValid)
            {
                tiposIncidentes.Vigente = true;
                db.Entry(tiposIncidentes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tiposIncidentes);
        }

        // GET: TiposIncidentes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposIncidentes tiposIncidentes = db.TiposIncidentes.Find(id);
            if (tiposIncidentes == null)
            {
                return HttpNotFound();
            }
            return View(tiposIncidentes);
        }

        // POST: TiposIncidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TiposIncidentes tiposIncidentes = db.TiposIncidentes.Find(id);
            tiposIncidentes.Vigente = false;
            //db.TiposIncidentes.Remove(tiposIncidentes);
            db.Entry(tiposIncidentes).State = EntityState.Modified;
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
