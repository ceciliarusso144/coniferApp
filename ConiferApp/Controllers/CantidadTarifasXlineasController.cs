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
    public class CantidadTarifasXlineasController : Controller
    {
        private DbCantidadTarifasXlineas db = new DbCantidadTarifasXlineas();

        // GET: CantidadTarifasXlineas
        public ActionResult Index()
        {
            return View(db.CantidadTarifasXlineas.ToList());
        }

        public ActionResult VistaPrevia()
        {
            return new Rotativa.ViewAsPdf(db.CantidadTarifasXlineas.ToList());
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
