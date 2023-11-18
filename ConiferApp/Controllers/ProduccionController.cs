using ConiferApp.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ConiferApp.Controllers
{
    public class ProduccionController : Controller
    {
        private readonly string cadena = ConfigurationManager.AppSettings["CadenaBD"];
        public ActionResult ControlarProduccion(int? page, string Ordenamiento, string cadenaBusqueda, string tipoBusqueda)  //seria el index
        {

            //armamos el modelo y lo retornamos

            IEnumerable<ProduccionDetalle> _pDetalle = ArmarListaModelo();

            if (!string.IsNullOrEmpty(cadenaBusqueda))
            {
                //_pDetalle = _pDetalle.Where(p => p.coche.Contains(cadenaBusqueda));
                
                switch (tipoBusqueda)
                {
                    case "Coche":
                        _pDetalle = _pDetalle.Where(p => p.coche == Convert.ToInt32(cadenaBusqueda));
                        break;
                    
                    case "Linea":
                        _pDetalle = _pDetalle.Where(p => p.linea == Convert.ToInt32(cadenaBusqueda));
                        break;
                    
                    case "Legajo":
                        _pDetalle = _pDetalle.Where(p => p.legajo == Convert.ToInt32(cadenaBusqueda));
                        break;
                    
                    case "Controladora":
                        _pDetalle = _pDetalle.Where(p => p.controladora == cadenaBusqueda);
                        break;
                    
                    case "Fecha":
                        DateTime _fecha = DateTime.Parse(cadenaBusqueda).Date;
                        _pDetalle = _pDetalle.Where(p => p.fecha == _fecha);
                        break;
                }
            }
            
            ViewBag.OrdenLinea = string.IsNullOrEmpty(Ordenamiento) ? "Linea_desc" : "";
            ViewBag.OrdenCoche = Ordenamiento == "Coche" ? "Coche_desc" : "Coche";

            switch (Ordenamiento)
            {
                case "Linea_desc":
                    _pDetalle = _pDetalle.OrderByDescending(p => p.linea);
                    break;
                
                case "Coche":
                    _pDetalle = _pDetalle.OrderBy(p => p.coche);
                    break;
                
                case "Coche_desc":
                    _pDetalle = _pDetalle.OrderByDescending(p => p.coche);
                    break;
            }
            
            
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(_pDetalle.ToPagedList(pageNumber, pageSize));

            
        }

        
        public ActionResult Edit(int id) //muestra la vista con el modelo a editar
        {

            SqlConnection cnn = new SqlConnection(cadena);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string consulta = "select * from ProduccionDetalle where id = " + id.ToString();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = consulta;

            cnn.Open();
            cmd.Connection = cnn;
            var Resultado = cmd.ExecuteReader();

            dt.Load(Resultado);

            ProduccionDetalle pmodel = new ProduccionDetalle
            {
                id = Convert.ToInt32(dt.Rows[0]["id"].ToString()),

                idproduccion = Convert.ToInt32(dt.Rows[0]["idproduccion"].ToString()),

                fecha = Convert.ToDateTime(dt.Rows[0]["fecha"].ToString()),

                linea = Convert.ToInt32(dt.Rows[0]["linea"].ToString()),

                coche = Convert.ToInt32(dt.Rows[0]["coche"].ToString()),

                legajo = Convert.ToInt32(dt.Rows[0]["legajo"].ToString()),

                horainicio = dt.Rows[0]["horainicio"].ToString(),

                horafin = dt.Rows[0]["horafin"].ToString(),

                controladora = dt.Rows[0]["controladora"].ToString(),

                vueltas = Convert.ToDecimal(dt.Rows[0]["vueltas"].ToString())
            };

            
            return View(pmodel);
        }

        [HttpPost]
        public ActionResult Edit(int id, ProduccionDetalle pmodel)    //edita el modelo
        {
            if (id != pmodel.id)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //aca edita la tabla de la db

                    
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction(nameof(ControlarProduccion));
            }
            return View(pmodel);
        }



        public ActionResult Details(int id)
        {
            SqlConnection cnn = new SqlConnection(cadena);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string consulta = "select * from ProduccionDetalle where id = " + id.ToString();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = consulta;

            cnn.Open();
            cmd.Connection = cnn;
            var Resultado = cmd.ExecuteReader();

            dt.Load(Resultado);
            
            ProduccionDetalle pmodel = new ProduccionDetalle
            {
                id = Convert.ToInt32(dt.Rows[0]["id"].ToString()),

                idproduccion = Convert.ToInt32(dt.Rows[0]["idproduccion"].ToString()),

                fecha = Convert.ToDateTime(dt.Rows[0]["fecha"].ToString()),

                linea = Convert.ToInt32(dt.Rows[0]["linea"].ToString()),

                coche = Convert.ToInt32(dt.Rows[0]["coche"].ToString()),

                legajo = Convert.ToInt32(dt.Rows[0]["legajo"].ToString()),

                horainicio = dt.Rows[0]["horainicio"].ToString(),

                horafin = dt.Rows[0]["horafin"].ToString(),

                controladora = dt.Rows[0]["controladora"].ToString(),

                vueltas = Convert.ToDecimal(dt.Rows[0]["vueltas"].ToString())
            };
           
            return View(pmodel);
        }


        public List<ProduccionDetalle> ArmarListaModelo()
        {
            SqlConnection cnn = new SqlConnection(cadena);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            string consulta = "select * from ProduccionDetalle order by id";

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = consulta;

            cnn.Open();
            cmd.Connection = cnn;
            var Resultado = cmd.ExecuteReader();

            dt.Load(Resultado);

            List<ProduccionDetalle> _lista = new List<ProduccionDetalle>();
            
            foreach (DataRow fila in dt.Rows)
            {
                _lista.Add(new ProduccionDetalle
                {
                    id = Convert.ToInt32(fila["id"].ToString()),

                    idproduccion = Convert.ToInt32(fila["idproduccion"].ToString()),

                    fecha = Convert.ToDateTime(fila["fecha"].ToString()),

                    linea = Convert.ToInt32(fila["linea"].ToString()),

                    coche = Convert.ToInt32(fila["coche"].ToString()),

                    legajo = Convert.ToInt32(fila["legajo"].ToString()),

                    horainicio = fila["horainicio"].ToString(),

                    horafin = fila["horafin"].ToString(),

                    controladora = fila["controladora"].ToString(),

                    vueltas = Convert.ToDecimal(fila["vueltas"].ToString())
                }) ;
            }

            return _lista;

        }

    }
}