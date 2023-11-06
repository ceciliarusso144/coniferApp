using ConiferApp.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace ConiferApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly string cadena = ConfigurationManager.ConnectionStrings["CadenaBD"].ConnectionString;
        public static Usuario logueado;
       
        public ActionResult Login()
        {          
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            try
            {
                int Legajo = oUsuario.Legajo;
                if (oUsuario.Legajo != 0 && oUsuario.Clave != null)
                {
                    //oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

                    using (SqlConnection cn = new SqlConnection(cadena))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                        cmd.Parameters.AddWithValue("Legajo", oUsuario.Legajo);
                        cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cn.Open();
                        oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    }

                    if (oUsuario.IdUsuario != 0)
                    {
                        FormsAuthentication.SetAuthCookie(oUsuario.IdUsuario.ToString(), false);            
                        logueado = GetUsuario(oUsuario.Legajo);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewData["Mensaje"] = "Usuario no encontrado";
                    }
                }
                else
                {
                    if(Legajo == 0 && string.IsNullOrEmpty(oUsuario.Clave))
                    {
                        ViewData["Mensaje"] = "No ingresó ningún dato...";
                    }
                    else if (string.IsNullOrEmpty(oUsuario.Clave))
                    {
                        ViewData["Mensaje"] = "No ingresó contraseña...";
                    }
                    else
                    {
                        ViewData["Mensaje"] = "No ingresó su legajo...";
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewData["Mensaje"] = "Problema con la base de datos";
                return View();
            }          
        }

        private Usuario GetUsuario(int legajo)
        {          
            SqlConnection cnn = new SqlConnection(cadena);
            try
            {
                SqlCommand cmd = new SqlCommand();
                string consulta = "sp_ObtenerUsuario";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Legajo", legajo);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = consulta;

                cnn.Open();
                cmd.Connection = cnn;
                SqlDataReader lector = cmd.ExecuteReader();
                if (lector != null && lector.Read())
                {
                    Usuario u = new Usuario
                    {
                        IdUsuario = int.Parse(lector["IdUsuario"].ToString()),
                        Apellido = lector["Apellido"].ToString(),
                        Nombre = lector["Nombre"].ToString(),
                        DNI = int.Parse(lector["DNI"].ToString()),
                        Legajo = legajo,
                        Clave = lector["Clave"].ToString()
                    };
                    return u;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnn.Close();
            }
        }

        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}