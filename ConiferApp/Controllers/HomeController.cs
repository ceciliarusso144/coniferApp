using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace ConiferApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {    
        private readonly string cadena = ConfigurationManager.AppSettings["CadenaBD"];
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ULogueado()
        {
            ViewBag.Message = "DNI: " + AccesoController.logueado.DNI + " - Legajo: " + AccesoController.logueado.Legajo;
            return View();
        }

        [HttpPost]
        public ActionResult ULogueado(string Clave, string ConfirmarClave)
        {
            bool actualizado;
            string mensaje;
            int idUsuario = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerIdUsuario", cn);
                    cmd.Parameters.AddWithValue("Legajo", AccesoController.logueado.Legajo);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    idUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                if (Clave == "" || ConfirmarClave == "")
                {
                    if (Clave == "" && ConfirmarClave == "")
                    {
                        ViewData["Mensaje"] = "No ingresó la nueva contraseña, ni tampoco su confirmación";
                        return View();
                    }
                    else if (Clave == "")
                    {
                        ViewData["Mensaje"] = "No ingresó la nueva contraseña";
                        return View();
                    }
                    else
                    {
                        ViewData["Mensaje"] = "No ingresó la confirmación de la nueva contraseña";
                        return View();
                    }
                }
                else if (idUsuario != 0 && Clave == ConfirmarClave)
                {
                    Clave = AccesoController.ConvertirSha256(Clave);
                    using (SqlConnection cn = new SqlConnection(cadena))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ModificarUsuario", cn);
                        cmd.Parameters.AddWithValue("IdUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("Legajo", AccesoController.logueado.Legajo);
                        cmd.Parameters.AddWithValue("Clave", Clave);
                        cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                        mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                    }
                    if (actualizado)
                    {
                        ViewData["Correcto"] = mensaje;
                        return View();
                    }
                    else
                    {
                        ViewData["Mensaje"] = mensaje;
                        return View();
                    }
                }
                else
                {
                    ViewData["Mensaje"] = "Las contraseñas no coinciden";
                    return View();
                }
            }
            catch(Exception)
            {
                ViewData["Mensaje"] = "Problema con la base de datos";
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";
            return View();
        }

        public ActionResult Informes()
        {
            ViewBag.Message = "";
            return View();
        }

        public ActionResult Salir()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Acceso");
        }

    }
}