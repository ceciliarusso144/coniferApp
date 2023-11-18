using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ConiferApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly string cadena = ConfigurationManager.ConnectionStrings["CadenaBD"].ConnectionString;
        private DataTable reporte = new DataTable();

        public DataTable ObtenerCantidadTarifas(DateTime f1, DateTime f2, int id)
        {
            string consulta;
            DataTable tabla = new DataTable();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                #region Obtener Tabla              
                SqlCommand cmd = new SqlCommand();
                consulta = "ReporteCantidadTarifasXlinea";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fechaDesde", f1);
                cmd.Parameters.AddWithValue("@fechaHasta", f2);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = consulta;
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                tabla.Load(dr);
                dr.Close();
                return tabla;
                #endregion               
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public ActionResult CantidadTarifasXLinea()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CantidadTarifasXLinea(DateTime fecha1, DateTime fecha2, int linea)
        {
            Session["dt1"] = fecha1;
            Session["dt2"] = fecha2;
            Session["ln"] = linea;
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            reporte = ObtenerCantidadTarifas(fecha1, fecha2, linea);
            if (reporte.Rows.Count <= 0)
            {
                ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
            }
            else
            {
                ViewData["Resultado"] = "Cargado";
            }
            return View(reporte);
        }

        public ActionResult ImprimirCantidadTarifasXlinea()
        {
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            try
            {
                DateTime f1 = (DateTime)Session["dt1"];
                DateTime f2 = (DateTime)Session["dt2"];
                ViewData["Fecha1"] = f1.ToShortDateString();
                ViewData["Fecha2"] = f2.ToShortDateString();
                ViewData["Linea"] = Session["ln"];
                reporte = ObtenerCantidadTarifas((DateTime)Session["dt1"], (DateTime)Session["dt2"], (int)Session["ln"]);
                if (reporte.Rows.Count <= 0)
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
                return new Rotativa.ViewAsPdf(reporte);
            }
            catch (Exception)
            {
                ViewData["Mensaje"] = "NO HA INGRESADO DATOS PARA EL REPORTE";
                return new Rotativa.ViewAsPdf(reporte);
            }
        }

        public DataTable ObtenerBoletosPendientes(DateTime f1, DateTime f2, Boolean e)
        {
            string consulta;
            DataTable tabla = new DataTable();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                #region Obtener Tabla
                SqlCommand cmd = new SqlCommand();
                consulta = "sp_ObtenerPendientesPorFechaYEstado";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fechaDesde", f1);
                cmd.Parameters.AddWithValue("@fechaHasta", f2);
                cmd.Parameters.AddWithValue("@estado", e);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = consulta;
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                tabla.Load(dr);
                dr.Close();
                return tabla;
                #endregion               
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public ActionResult BoletosPendientes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BoletosPendientes(DateTime fecha1, DateTime fecha2, string estadoSeleccionado)
        {
            Session["dt1"] = fecha1;
            Session["dt2"] = fecha2;
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            if (estadoSeleccionado == "0")
            {
                Session["b"] = false;
                reporte = ObtenerBoletosPendientes(fecha1, fecha2, false);               
                if(reporte.Rows.Count <= 0) 
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";                 
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
            }
            else
            {
                reporte = ObtenerBoletosPendientes(fecha1, fecha2, true);
                Session["b"] = true;
                if (reporte.Rows.Count <= 0)
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
            }
            return View(reporte);
        }

        public ActionResult ImprimirBoletosPendientes()
        {
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            try
            {
                DateTime f1 = (DateTime)Session["dt1"];
                DateTime f2 = (DateTime)Session["dt2"];
                ViewData["Fecha1"] = f1.ToShortDateString();
                ViewData["Fecha2"] = f2.ToShortDateString();
                if ((Boolean)Session["b"])
                {
                    ViewData["Estado"] = "Pendientes saldados";
                }
                else
                {
                    ViewData["Estado"] = "Pendientes sin saldar";
                }
                reporte = ObtenerBoletosPendientes((DateTime)Session["dt1"], (DateTime)Session["dt2"], (Boolean)Session["b"]);
                if (reporte.Rows.Count <= 0)
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
                return new Rotativa.ViewAsPdf(reporte);
            }
            catch (Exception) 
            {
                ViewData["Mensaje"] = "NO HA INGRESADO DATOS PARA EL REPORTE";
                return new Rotativa.ViewAsPdf(reporte);
            }          
        }

        public DataTable ObtenerTotalesXCorredor(DateTime f1, DateTime f2, int id)
        {
            string consulta;
            DataTable tabla = new DataTable();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                #region Obtener Tabla              
                SqlCommand cmd = new SqlCommand();
                consulta = "";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fechaDesde", f1);
                cmd.Parameters.AddWithValue("@fechaHasta", f2);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = consulta;
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                tabla.Load(dr);
                dr.Close();
                return tabla;
                #endregion               
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public ActionResult TotalesXCorredor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TotalesXCorredor(DateTime fecha1, DateTime fecha2, int corredor)
        {
            Session["dt1"] = fecha1;
            Session["dt2"] = fecha2;
            Session["cr"] = corredor;
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            reporte = ObtenerTotalesXCorredor(fecha1, fecha2, corredor);
            if (reporte.Rows.Count <= 0)
            {
                ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
            }
            else
            {
                ViewData["Resultado"] = "Cargado";
            }
            return View(reporte);
        }

        public ActionResult ImprimirTotalesXCorredor()
        {
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            try
            {
                DateTime f1 = (DateTime)Session["dt1"];
                DateTime f2 = (DateTime)Session["dt2"];
                ViewData["Fecha1"] = f1.ToShortDateString();
                ViewData["Fecha2"] = f2.ToShortDateString();
                ViewData["Corredor"] = Session["cr"];
                reporte = ObtenerTotalesXCorredor((DateTime)Session["dt1"], (DateTime)Session["dt2"], (int)Session["cr"]);
                if (reporte.Rows.Count <= 0)
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
                return new Rotativa.ViewAsPdf(reporte);
            }
            catch (Exception)
            {
                ViewData["Mensaje"] = "NO HA INGRESADO DATOS PARA EL REPORTE";
                return new Rotativa.ViewAsPdf(reporte);
            }
        }

        public DataTable ObtenerTotalesXTarifa(DateTime f1, DateTime f2, int id)
        {
            string consulta;
            DataTable tabla = new DataTable();
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            try
            {
                #region Obtener Tabla              
                SqlCommand cmd = new SqlCommand();
                consulta = "";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fechaDesde", f1);
                cmd.Parameters.AddWithValue("@fechaHasta", f2);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = consulta;
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                tabla.Load(dr);
                dr.Close();
                return tabla;
                #endregion               
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cn.Close();
            }
        }

        public ActionResult TotalesXTarifa()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TotalesXTarifa(DateTime fecha1, DateTime fecha2, int corredor)
        {
            Session["dt1"] = fecha1;
            Session["dt2"] = fecha2;
            Session["cr"] = corredor;
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            reporte = ObtenerTotalesXTarifa(fecha1, fecha2, corredor);
            if (reporte.Rows.Count <= 0)
            {
                ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
            }
            else
            {
                ViewData["Resultado"] = "Cargado";
            }
            return View(reporte);
        }

        public ActionResult ImprimirTotalesXTarifa()
        {
            ViewData["Mensaje"] = null;
            ViewData["Resultado"] = null;
            try
            {
                DateTime f1 = (DateTime)Session["dt1"];
                DateTime f2 = (DateTime)Session["dt2"];
                ViewData["Fecha1"] = f1.ToShortDateString();
                ViewData["Fecha2"] = f2.ToShortDateString();
                ViewData["Corredor"] = Session["cr"];
                reporte = ObtenerTotalesXTarifa((DateTime)Session["dt1"], (DateTime)Session["dt2"], (int)Session["cr"]);
                if (reporte.Rows.Count <= 0)
                {
                    ViewData["Mensaje"] = "No hay datos registrados con esos filtros";
                }
                else
                {
                    ViewData["Resultado"] = "Cargado";
                }
                return new Rotativa.ViewAsPdf(reporte);
            }
            catch (Exception)
            {
                ViewData["Mensaje"] = "NO HA INGRESADO DATOS PARA EL REPORTE";
                return new Rotativa.ViewAsPdf(reporte);
            }
        }

    }
}
