using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ConiferApp.Controllers
{
    [Authorize]

    

    public class ControlarProduccion2Controller : Controller
    {
        private readonly string cadena = ConfigurationManager.AppSettings["CadenaBD"];
        private int cantidadboletos = 0;
        private int cantidadboletosHR = 0;
        //private int banderaerror = 0;
        //private DateTime _fechaProduccion;
        // private string _nombrefile;
        // GET: Consolidado

      //  public ActionResult Index()
       // {
            // Aquí recuperas el dato de la base de datos
      //      Producto producto = ObtenerProductoDesdeLaBaseDeDatos();

       //     return View(producto);
       // }


        public ActionResult ControlarProduccion2()
        {
            return View();
        }

        public ActionResult ObtenerCantidadDeBoletos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ControlarProduccion2( DateTime fecha)
        {
            string data = Request.Form.Keys[1].ToString();
            //_fechaProduccion = fecha;
           
            if (data == "Controlar")
            {
                // Guardando datos de Sesión
                Session["Fecha"] = fecha;
                SqlConnection cnn = new SqlConnection(cadena);

                //// PRODUCCION
                // try
                // {
                //     SqlCommand cmd = new SqlCommand();
                //     string consulta = "sp_ObtenerTotalBoletosProduccion 28";
                //     cmd.CommandType = CommandType.Text;
                //     cmd.CommandText = consulta;
                //     cnn.Open();
                //     cmd.Connection = cnn;
                //     var Resultado = cmd.ExecuteReader(); 

                //     if (!Resultado.HasRows)
                //     {
                //         ViewData["Error"] = "No hay registros de Producción.";
                //         return View();
                //     }

                //     DataTable dtvalor = new DataTable();
                //     dtvalor.Load(Resultado);

                //     foreach (DataRow fila in dtvalor.Rows)
                //     {
                //          cantidadboletos = Int32.Parse(fila["cantidad"].ToString());
                //     }                                     
                // }
                // catch (Exception ex)
                // {
                //     ViewData["Error"] = "Ocurrió un problema al Consultar Cantidad de Boletos." + ex;
                //     return View();
                // }
                // finally
                // {
                //     cnn.Close();
                // } // cierre de try catch de consultar boletos

                // // HOJAS DE RUTA
                // try
                // {
                //     SqlCommand cmd = new SqlCommand();
                //     string consulta = "sp_ObtenerTotalBoletosHR";

                //     cmd.CommandType = CommandType.Text;
                //     cmd.CommandText = consulta;
                //     //SqlConnection cnn = new SqlConnection(cadena);

                //     cnn.Open();
                //     cmd.Connection = cnn;
                //     var Resultado = cmd.ExecuteReader();

                //     if (!Resultado.HasRows)
                //     {
                //         ViewData["Error"] = "No hay registros de Hojas de Ruta.";
                //         return View();
                //     }
                //     DataTable dtvalor = new DataTable();
                //     dtvalor.Load(Resultado);

                //     foreach (DataRow fila in dtvalor.Rows)
                //     {
                //         cantidadboletosHR = Int32.Parse(fila["cantidad"].ToString());
                //     }
                // }
                // catch (Exception ex)
                // {
                //     ViewData["Error"] = "Ocurrió un problema al Consultar Hojas de Ruta." + ex;
                //     return View();
                // }
                // finally
                // {
                //     cnn.Close();
                // } // cierre de try catch de consultar boletos
                // ViewData["Exito"] = "Cant. Boletos en Hoja de Ruta: " + cantidadboletosHR + " - Cant. boletos en Consolidado:" + cantidadboletos;
                // return View();

                int contadorboletos = 0;
                int contadorpendientes = 0;
                int contadornuevospendientes = 0;

                SqlCommand cmd = new SqlCommand();
                string consulta = "sp_Obtener_HR_ParaControlar";

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = consulta;

                cnn.Open();
                cmd.Connection = cnn;
                var Resultado = cmd.ExecuteReader();

                DataTable dtvalor = new DataTable();
                dtvalor.Load(Resultado);
                Resultado.Close();
                
                
                if (dtvalor.Rows.Count == 0 )   
                {
                    ViewData["ExitoNoHayRegistrosParaControlar"] = "No hay registros de Hojas de Ruta para Controlar.";
                    return View();
                }
                           

                foreach (DataRow fila in dtvalor.Rows)
                {
                    int _legajo     = Int32.Parse(fila["legajo_chofer"].ToString());
                    int _coche      = Int32.Parse(fila["coche"].ToString());
                    int _linea      = Int32.Parse(fila["linea"].ToString());
                    int _id         = Int32.Parse(fila["id"].ToString());
                    int _boletosHR  = Int32.Parse(fila["cantidad_boletos"].ToString());
                    
                    // Ver si se incluye fecha

                    contadorboletos += _boletosHR;

                    // Por cada registro de HR,s e busca en el la Produccion
                    SqlCommand sqlcmdPendiente1; 
                    sqlcmdPendiente1 = new SqlCommand("sp_ObtenerProduccionParaControlar", cnn);
                    sqlcmdPendiente1.CommandType = CommandType.StoredProcedure;

                    sqlcmdPendiente1.Parameters.AddWithValue("@legajo", _legajo);
                    sqlcmdPendiente1.Parameters.AddWithValue("@coche",  _coche);
                    sqlcmdPendiente1.Parameters.AddWithValue("@linea",  _linea);
                    sqlcmdPendiente1.Parameters.AddWithValue("@fecha",  fecha);

                    var ResultadoProduccion = sqlcmdPendiente1.ExecuteReader();

                    DataTable dt_produccion = new DataTable();
                    dt_produccion.Load(ResultadoProduccion);
                    ResultadoProduccion.Close();

                    if (dt_produccion.Rows.Count == 0 )
                    {
                        // No se encuentran datos de HR en Produccion
                        // debe guardarse como Pendientes
                        contadornuevospendientes += _boletosHR;

                        SqlCommand sqlcmd_ = new  SqlCommand("sp_InsertarPendiente", cnn);

                        //sqlcmdPendiente = new SqlCommand("sp_InsertarPendiente", cnn);
                        sqlcmd_.CommandType = CommandType.StoredProcedure;

                        //sqlcmdPendiente.close()
                        sqlcmd_.Parameters.Clear();
                        sqlcmd_.Parameters.AddWithValue("@legajo", _legajo);
                        sqlcmd_.Parameters.AddWithValue("@coche", _coche);
                        sqlcmd_.Parameters.AddWithValue("@linea", _linea);
                        sqlcmd_.Parameters.AddWithValue("@pendiente", _boletosHR);
                        sqlcmd_.Parameters.AddWithValue("@fecha", fecha); // Ver Fecha

                        var result = sqlcmd_.ExecuteNonQuery();
                       
                        // Tambien se Actualiza el Estado de la HR, como ya revisada
                        SqlCommand sqlcmdCerrarHR = new SqlCommand();
                        sqlcmdCerrarHR = new SqlCommand("sp_CerrarEstadoHR", cnn);
                        sqlcmdCerrarHR.CommandType = CommandType.StoredProcedure;

                        sqlcmdCerrarHR.Parameters.AddWithValue("@id", _id);
                        sqlcmdCerrarHR.ExecuteNonQuery();

                        break;
                    }
                    // 


                    foreach (DataRow filaproduccion in dt_produccion.Rows) // Ver si esta correcto que sean varios registros devueltos
                    {
                        int _boletosproduccion = Int32.Parse(filaproduccion["cantidadboletos"].ToString());

                        if (_boletosHR == _boletosproduccion)
                        {
                            //Misma cantidad de boletos
                            // Actualizar en Produccion ?
                            SqlCommand sqlcmdCerrarHR2 = new SqlCommand();
                            sqlcmdCerrarHR2 = new SqlCommand("sp_CerrarEstadoHR", cnn);
                            sqlcmdCerrarHR2.CommandType = CommandType.StoredProcedure;
                            sqlcmdCerrarHR2.Parameters.AddWithValue("@id", _id);
                            sqlcmdCerrarHR2.ExecuteNonQuery();


                            // Actualizando Produccion
                            SqlCommand sqlcmdCerrarProduccion = new SqlCommand();
                            sqlcmdCerrarProduccion = new SqlCommand("sp_CerrarEstadoProduccionDetalle", cnn);
                            sqlcmdCerrarProduccion.CommandType = CommandType.StoredProcedure;

                            sqlcmdCerrarProduccion.Parameters.AddWithValue("@legajo", _legajo);
                            sqlcmdCerrarProduccion.Parameters.AddWithValue("@coche", _coche);
                            sqlcmdCerrarProduccion.Parameters.AddWithValue("@linea", _linea);
                            sqlcmdCerrarProduccion.Parameters.AddWithValue("@fecha", fecha); // Ver Fecha
                            sqlcmdCerrarProduccion.ExecuteNonQuery();

                            break;
                        }

                        if (_boletosHR > _boletosproduccion)
                        {
                            //Vienen mas boletos en HR, la diferencia se guarda como pendiente
                            int _boletosdiferencia = _boletosHR - _boletosproduccion;

                            contadornuevospendientes += _boletosdiferencia;

                            SqlCommand sqlcmdPendiente3 = new SqlCommand();

                            sqlcmdPendiente3 = new SqlCommand("sp_InsertarPendiente", cnn);
                            sqlcmdPendiente3.CommandType = CommandType.StoredProcedure;

                            sqlcmdPendiente3.Parameters.AddWithValue("@legajo", _legajo);
                            sqlcmdPendiente3.Parameters.AddWithValue("@coche", _coche);
                            sqlcmdPendiente3.Parameters.AddWithValue("@linea", _linea);
                            sqlcmdPendiente3.Parameters.AddWithValue("@pendiente", _boletosdiferencia);
                            sqlcmdPendiente3.Parameters.AddWithValue("@fecha", fecha); // Ver Fecha

                            sqlcmdPendiente3.ExecuteNonQuery();

                            // Tambien se Actualiza el Estado de la HR, como ya revisada
                            SqlCommand sqlcmdCerrarHR3 = new SqlCommand();
                            sqlcmdCerrarHR3 = new SqlCommand("sp_CerrarEstadoHR", cnn);
                            sqlcmdCerrarHR3.CommandType = CommandType.StoredProcedure;

                            sqlcmdCerrarHR3.Parameters.AddWithValue("@id", _id);
                            sqlcmdCerrarHR3.ExecuteNonQuery();

                            // Actualizando Produccion
                            SqlCommand sqlcmdCerrarProduccion2 = new SqlCommand();
                            sqlcmdCerrarProduccion2 = new SqlCommand("sp_CerrarEstadoProduccionDetalle", cnn);
                            sqlcmdCerrarProduccion2.CommandType = CommandType.StoredProcedure;

                            sqlcmdCerrarProduccion2.Parameters.AddWithValue("@legajo", _legajo);
                            sqlcmdCerrarProduccion2.Parameters.AddWithValue("@coche", _coche);
                            sqlcmdCerrarProduccion2.Parameters.AddWithValue("@linea", _linea);
                            sqlcmdCerrarProduccion2.Parameters.AddWithValue("@fecha", fecha); // Ver Fecha
                            sqlcmdCerrarProduccion2.ExecuteNonQuery();

                            break;
                        }
                    } //foreach de produccion
                    break; 
                } // foreach de HR

                // Ya se revisaron todas las HR, ahora se deben guardar actualizar los pendientes
                //SqlCommand cmd = new SqlCommand();
                SqlCommand cmdPendientesNuevos = new SqlCommand("sp_ObtenerProduccionSinControlar", cnn);
                cmdPendientesNuevos.CommandType = CommandType.StoredProcedure;
                cmdPendientesNuevos.Parameters.AddWithValue("@fecha", fecha); // Ver Fecha
                //cmdPendientesNuevos.CommandText = consulta;

                var ResultadoHRPendientes = cmdPendientesNuevos.ExecuteReader();

                DataTable dt_pendientes = new DataTable();
                dt_pendientes.Load(ResultadoHRPendientes);

                ResultadoHRPendientes.Close();

                if (dt_pendientes.Rows.Count > 0 )
                {

                    foreach (DataRow filapendiente in dt_pendientes.Rows) // Ver si esta correcto que sean varios registros devueltos
                    {
                        int _legajopendiente = Int32.Parse(filapendiente["legajo"].ToString());
                        int _cochependiente = Int32.Parse(filapendiente["coche"].ToString());
                        int _lineapendiente = Int32.Parse(filapendiente["linea"].ToString());
                        //int _idpendiente = Int32.Parse(filapendiente["id"].ToString());
                        DateTime  _fechapendiente = Convert.ToDateTime(filapendiente["fecha"].ToString());
                        int _boletospendientes = Int32.Parse(filapendiente["cantidadboletos"].ToString());

                        contadorpendientes += _boletospendientes;

                        SqlCommand sqlcmdPendiente4 = new SqlCommand();
                        sqlcmdPendiente4 = new SqlCommand("sp_ActualizarPendiente", cnn);
                        sqlcmdPendiente4.CommandType = CommandType.StoredProcedure;

                        sqlcmdPendiente4.Parameters.AddWithValue("@legajo", _legajopendiente);
                        sqlcmdPendiente4.Parameters.AddWithValue("@coche", _cochependiente);
                        sqlcmdPendiente4.Parameters.AddWithValue("@linea", _lineapendiente);
                        sqlcmdPendiente4.Parameters.AddWithValue("@fecha", _fechapendiente);
                        sqlcmdPendiente4.Parameters.AddWithValue("@cantidad_boletos", _boletospendientes);
                        sqlcmdPendiente4.ExecuteNonQuery();
                    }
                }
              
                ViewData["Exito"] = "Fin Control de Producción. Total Boletos:"+ contadorboletos+" - Pendientes:"+ contadorpendientes+" - Nuevos Pendientes:"+ contadornuevospendientes ;
                return View();

            }
            else
            {
              SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "sp_CerrarEstadoProduccion";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;
                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteNonQuery();
                    consulta = "sp_CerrarEstadoHR";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;
                    cmd.Connection = cnn;
                    Resultado = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al Cerrar la Producción." + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                } // cierre de try catch de cargar archivo
                ViewData["ExitoCerrarProducción"] = "Se ha Cerrado la Produccion.";
                return View();
            }
        }
    }
}

