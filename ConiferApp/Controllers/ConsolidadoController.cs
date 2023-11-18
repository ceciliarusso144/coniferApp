using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConiferApp.Controllers
{
    [Authorize]
    public class ConsolidadoController : Controller
    {
        private readonly string cadena = ConfigurationManager.AppSettings["CadenaBD"];
        private int filas = 0;
        private int banderaerror = 0;
        private DateTime _fechaProduccion;
        private string _nombrefile;
        // GET: Consolidado
        public ActionResult Inicio()
        {
            return View();
        }

        public ActionResult CargarArchivoConsolidado()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarArchivoConsolidado(HttpPostedFileBase file, DateTime fecha)
        {
            string data = Request.Form.Keys[1].ToString();
            _fechaProduccion = fecha;
            _nombrefile = file.FileName;

            if (data == "Importar")
            {
                // Guardando datos de Sesión
                Session["Archivo"] = file;
                Session["Fecha"] = fecha;


                string filepath = Server.MapPath("~/Excel/") + file.FileName;
                file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));
                //string lMensajeResultado;


                SqlConnection cnn = new SqlConnection(cadena);


                // Validar que la fecha de producción no se encuentre cerrada
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Produccion_ConsultarPorFecha '" + fecha + "'";

                    cmd.CommandType = CommandType.Text;

                    //cmd.CommandType = proce
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader();

                    if (Resultado.HasRows)
                    {
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorProduccionCerrada"] = "La Fecha de Producción ya se encuentra CERRADA.";

                        //return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un error al intentar consultar Fechas de Producción. " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }


                // Validar ConsultarSiExiste el archivo consolidado
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Archivos_ConsultarSiExiste '" + file.FileName + "'";

                    cmd.CommandType = CommandType.Text;

                    //cmd.CommandType = proce
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader();

                    if (Resultado.HasRows)
                    {
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorArchivoYaExiste"] = "El archivo consolidado ya fue procesado.";
                        //return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al intentar consultar si el archivo ya fue procesado. " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }


                //Copiando datos desde archivo consolidado a tablas 
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "TRUNCATE TABLE Consolidado BULK INSERT Consolidado FROM '" + filepath + "' WITH ( ROWTERMINATOR = '\n', FIRSTROW = 1 )";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    filas = cmd.ExecuteNonQuery();
                    // lMensajeResultado = "Se cargaron " + filas + " registros exitosamente.."+;
                    //return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al cargar los datos del archivo " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }



                // Validar Header de Coche
                // SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarHeader";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    // int filas = cmd.ExecuteNonQuery();
                    var Resultado = cmd.ExecuteReader();

                    //SqlDataReader reader = cmd.ExecuteReader();
                    //int filas = cmd.ExecuteNonQuery();
                    //string CantidadFilas;
                    //CantidadFilas = reader.GetString(1);
                    //reader.Close();
                    // ViewData["Exito"] = "Paso."+filas;
                    if (!Resultado.HasRows)
                    {
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorCoches"] = "No hay registros de COCHES. ";
                        // return View();
                    }

                    // lMensajeResultado = lMensajeResultado + " Se encontraron " + filas + " registro/s de Header";
                    //// ViewData["Exito"] = lMensajeResultado;
                    ////return View();
                    // ViewData["ExitoHeader"] =  " Se encontraron " + filas + " registro/s de Header";
                    // return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Coches. " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }



                // Validar Cambios de Zona
                //SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarCambiosDeZona";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader();

                    if (!Resultado.HasRows)
                    {
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorZonas"] = "No hay registros de Zonas. ";
                        //return View();
                    }

                    // lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de Cambio de Zona";
                    //ViewData["ExitoCambioDeZona"] = "Se encontraron " + filas + " registros de Cambio de Zona";
                    //return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Zonas. " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }

                // Validar Apertura Chofer
                //SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarAperturaChofer";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); ;

                    if (!Resultado.HasRows)
                    {
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorLegajos"] = "No hay registros de  Legajos. ";
                        //return View();
                    }

                    //lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de Apertura Chofer";
                    //ViewData["AperturaChofer"] = "Se encontraron " + filas + " registros de Apertura Chofer";
                    //return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Legajos." + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }

                // Validar 2daApertura Chofer
                // SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_Validar2daAperturaChofer";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); ;

                    if (!Resultado.HasRows)
                    {
                        // Apertura 2 lo tomamos como la apertura 1
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorLegajos"] = "No hay registros de Legajos. ";
                        //return View();
                    }

                    //lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de  2da Apertura Chofer";
                    //ViewData["2daAperturaChofer"] = "Se encontraron " + filas + " registros de  2da Apertura Chofer";
                    //return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Legajos." + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }

                // Validar Validar Cierre Chofer
                //SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarCierreChofer";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); ;

                    if (!Resultado.HasRows)
                    {
                        // Tomamos como apertura 1
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorLegajos"] = "No hay registros de Legajos. ";
                        //return View();
                    }

                    // lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de  Cierre de Chofer";
                    //ViewData["Exito"] = lMensajeResultado;
                    //  ViewData["CierreChofer"] = "Se encontraron " + filas + " registros de  Cierre de Chofer";
                    // return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros   Cierre de Chofer " + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }


                // Validar Validar Cambio de Linea
                // SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarCambioDeLinea";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); ;

                    if (!Resultado.HasRows)
                    {   // 
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorLineas"] = "No hay registros de Lineas ";
                        //return View();
                    }

                    //lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de  Cambio de Linea";
                    //ViewData["ExitoCambioDeLinea"] = "Se encontraron " + filas + " registros de  Cambio de Linea";
                    //return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Lineas." + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }


                // Validar Transacciones Económicas
                //SqlConnection cnn = new SqlConnection(cadena);
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ValidarTransaccionesEconomicas";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); ;

                    if (!Resultado.HasRows)
                    {
                        // Valida Transacciones Economicas / Boletos
                        banderaerror = 1;
                        ViewData["Error"] = "Archivo con errores:";
                        ViewData["ErrorBoletos"] = "No hay registros de Boletos";
                        //return View();
                    }

                    //  lMensajeResultado = lMensajeResultado + "Se encontraron " + filas + " registros de  Transacciones Económicas";
                    // ViewData["ExitoTransaccionesEconomicas"] = "Se encontraron " + filas + " registros de  Transacciones Económicas";
                    // return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al leer registros de Boletos" + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                }

                if (banderaerror == 1)
                {
                    ViewData["Error"] = "Archivo con errores:";
                    return View();
                }


                ViewData["Exito"] = "Se cargaron " + filas + " registros exitosamente.";
                return View();

            }
            else  // Caso de Carga de Archivo 
            {
                SqlConnection cnn = new SqlConnection(cadena);

                

                try
                {
                    SqlCommand cmd = new SqlCommand();
                    string consulta = "Consolidado_ObtenerDatosParaProcesar";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = consulta;
                   //SqlConnection cnn = new SqlConnection(cadena);

                    cnn.Open();
                    cmd.Connection = cnn;
                    var Resultado = cmd.ExecuteReader(); 

                    if (!Resultado.HasRows)
                    {
                        ViewData["Error"] = "No hay registros en el Consolidado.";
                        return View();
                    }

                    ///////prueba //////
                    
                    DataTable dtvalor = new DataTable();

                    dtvalor.Load(Resultado);

                    ////////

                    ///////////////////////
                    // DataTable Cabecera
                    ///////////////////////
                    DataTable produccion_detalle = new DataTable();
                    DataColumn column;
                    //DataRow row;

                    //Definiendo tabla 
                    // Campo Id
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "id"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo fecha
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.DateTime"),
                        ColumnName = "fecha"
                    };
                    produccion_detalle.Columns.Add(column);


                    // Campo linea
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "linea"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo coche
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "coche"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo legajo
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "legajo"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo hora inicio
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = "horainicio"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo hora fin
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = "horafin"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo nro. controladora
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = "numerodecontroladora"
                    };
                    produccion_detalle.Columns.Add(column);

                    // Campo vueltas
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "vueltas"
                    };
                    produccion_detalle.Columns.Add(column);

                    ///////////////////////
                    // DataTable Detalle
                    ///////////////////////
                    DataTable tarifasXproduccion_detalle = new DataTable();

                    // Campo Id cabecera        //////// id de tabla produccion detalle
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "idcabecera"
                    };
                    tarifasXproduccion_detalle.Columns.Add(column);

                    // Campo Id tarifa
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "idtarifa"
                    };
                    tarifasXproduccion_detalle.Columns.Add(column);

                    // Campo cantidad de boletos
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = "cantidad"
                    };
                    tarifasXproduccion_detalle.Columns.Add(column);


                    // Iniciando variables
                    int contador = 0;                   // Contador de renglones
                    int vueltas = 0;                    //Registra la cantidad de vueltas
                    string numerodecontroladora = "";   // Guarda el id de controladora que se esta procesando
                    int coche = 0;                      // Coche
                    int haycabecera = 0;                // Indica si se inicio  una cabecera nueva
                    string tiporegistro;                // se utiliza para detectar cual tipo de registro es
                    string stringfecha = "";            // guarda la fecha 
                    int linea = 0;                      //Nro de Linea que se esta procesando 
                    string horainicio = " ";             // Hora de inicio de chofer
                    string horafin = " ";                // Hora de Fin de chofer
                    int contadorid = 1;                 //Contador de id datatable cabecera
                    int legajo = 0;                     // Legajo del cocher que se esta procesando
                    int legajonuevo = 0;                // Legajo que se evalua cuando cambia
                    int idtarifaexterna = 0;            // Guarda el nro. de Id Externo de una tarifa, el id que viene en el consolidado
                    int cantidadboletos = 0;              // Contador de Boletos
                    int zona = 0;                       // Guarda la ultima zona indicada en el consolidado
                    int zonainicial = 0;                // Guarda la Zona inicial
                    int contadorboletos = 0;            // Contador de Boletos 
                    DataRow rowcabecera;
                    DataRow rowdetalle;

                    rowcabecera = null;   // VER //////////////////////////////////////////////////////////

                    // Recorriendo reader con los datos de respuesta
                    //while (Resultado.HasRows)

                    foreach (DataRow fila in dtvalor.Rows)
                    {
                        string cadenatexto = fila["dato"].ToString();
                        //string cadenatexto = Resultado.GetString(0);

                        //string cadenatexto = dtvalor.Rows[contador]["dato"].ToString();

                        if (cadenatexto.StartsWith("1"))  // Header de Coche, inicia un coche nuevo
                        {
                            if (contador == 0)
                            { vueltas = 1; }
                            else { vueltas = 0; }
                            
                            stringfecha = cadenatexto.Substring(1, 2) + '/' + cadenatexto.Substring(3, 2) + '/' + cadenatexto.Substring(5, 4);
                            linea = Int32.Parse(cadenatexto.Substring(9, 3));


                            // Validar para lineas 930 y 934
                            if (linea == 930)
                            { linea = 30; }

                            if (linea == 934)
                            { linea = 34; }

                            coche = Int32.Parse(cadenatexto.Substring(12, 4));
                            haycabecera = 1;
                            numerodecontroladora = "";
                        }
                        else
                        {
                            tiporegistro = cadenatexto.Substring(1, 2);

                            switch (tiporegistro)
                            {
                                // Numero de Controladora
                                case "ID":
                                    numerodecontroladora = cadenatexto.Substring(5, 8);
                                    break;

                                // Se registra fecha
                                case "AU":
                                    stringfecha = cadenatexto.Substring(7, 2) + '/' + cadenatexto.Substring(9, 2) + '/' + cadenatexto.Substring(11, 4);
                                    //cadenatexto.Substring(1, 2) + '/' + cadenatexto.Substring(3, 2) + '/' + cadenatexto.Substring(5, 4);
                                    break;

                                // Cambio de Linea
                                case "CL":
                                    int posiblenuevalinea = Int32.Parse(cadenatexto.Substring(3, 4));

                                    // Validar para lineas 930 y 934
                                    if (posiblenuevalinea == 930)
                                    { posiblenuevalinea = 30; }

                                    if (posiblenuevalinea == 934)
                                    { posiblenuevalinea = 34; }

                                    if (posiblenuevalinea != linea)
                                    {
                                       if (contadorid == 56)
                                        {
                                            //int a = 1; 
                                        }

                                        // hay cambio de linea, se debe registrar
                                        // la hora fin del registro anterior
                                        rowcabecera["horafin"] = horafin;
                                        //rowcabecera["id"] = contadorid;

                                        
                                        produccion_detalle.Rows.Add(rowcabecera);
                                        rowcabecera = null;

                                        // Creando nuevo registro de cabecera
                                        contadorid++;
                                        
                                        rowcabecera = produccion_detalle.NewRow();
                                        rowcabecera["id"] = contadorid;
                                        rowcabecera["linea"] = linea;
                                        rowcabecera["numerodecontroladora"] = numerodecontroladora;
                                        //produccion_detalle.Rows.Add(rowcabecera);
                                    }
                                    break;

                                // Cambio de Chofer
                                case "CH":
                                    legajonuevo = Int32.Parse(cadenatexto.Substring(7, 4));
                                    horainicio = cadenatexto.Substring(3, 2) + ":" + cadenatexto.Substring(5, 2);

                                    if (haycabecera == 1 || legajonuevo != legajo)
                                    // Si el legajo cambio, entonces hay nuevo registro cabecera
                                    {
                                        /* if (rowcabecera == null)
                                         {
                                             contadorid++;
                                             rowcabecera = produccion_detalle.NewRow();
                                             rowcabecera["id"] = contadorid;
                                         }
                                        */
                                        if (rowcabecera != null)
                                        {
                                           rowcabecera["horafin"] = horafin;  // Cerramos el anterior registro
                                        }

                                        if (contadorid == 1 || legajo != 0)
                                        {

                                            if (contadorid != 1)
                                            {
                                                produccion_detalle.Rows.Add(rowcabecera);
                                                rowcabecera = null;
                                            }
                                            rowcabecera = produccion_detalle.NewRow();
                                            contadorid++;
                                            rowcabecera["id"] = contadorid; // Nuevo registro
                                            legajo = legajonuevo;
                                        }

                                        rowcabecera["fecha"] = Convert.ToDateTime(stringfecha);    //ACA SALTA EL ERROR
                                        rowcabecera["linea"] = linea;
                                        rowcabecera["coche"] = coche;
                                        rowcabecera["legajo"] = legajo;
                                        rowcabecera["horainicio"] = horainicio;
                                        rowcabecera["numerodecontroladora"] = numerodecontroladora;

                                        // Aqui va la validacion por mas de 100 vueltas, pero no lo
                                        // hacemos
                                        rowcabecera["vueltas"] = vueltas;

                                    }
                                    haycabecera = 0;

                                    break;

                                // Cierre de Chofer
                                case "CC":
                                    // Se guarda la hora fin que se registro
                                    horafin = cadenatexto.Substring(3, 2) + ":" + cadenatexto.Substring(5, 2);
                                    break;

                                // Registro de Transaccion Económica
                                case "US":
                                    idtarifaexterna = Int32.Parse(cadenatexto.Substring(5, 2));
                                    cantidadboletos = Int32.Parse(cadenatexto.Substring(7, 2));
                                    zona = Int32.Parse(cadenatexto.Substring(3, 2));


                                    if (zona < zonainicial )
                                    {
                                        
                                        if (contadorboletos != 0)
                                        {
                                            rowcabecera["vueltas"] = vueltas;
                                            rowcabecera["horafin"] = horafin;

                                            produccion_detalle.Rows.Add(rowcabecera);
                                            contadorboletos = 0;
                                            rowcabecera = null;

                                            // Se crea nuevo registro cabecera
                                            rowcabecera = produccion_detalle.NewRow();
                                            contadorid++;
                                            rowcabecera["id"] = contadorid; // Nuevo registro
                                            rowcabecera["numerodecontroladora"] = numerodecontroladora;
                                        }
                                    

                                         vueltas++;
                                         contadorboletos += cantidadboletos;
                                         rowcabecera["fecha"] = Convert.ToDateTime(stringfecha);
                                         rowcabecera["linea"] = linea;
                                         rowcabecera["coche"] = coche;
                                         rowcabecera["legajo"] = legajo;
                                         rowcabecera["horainicio"] = horainicio;
                                         rowcabecera["numerodecontroladora"] = numerodecontroladora;
                                         // Aqui validacion de 100 vueltas
                                         // No lo contemplamos

                                         rowcabecera["vueltas"] = vueltas;


                                    }


                                    // Obtener el nro. de Id que corresponde a la tarifa
                                    //int idtarifa = idtarifaexterna;

                                    SqlCommand _sqlcmd = new SqlCommand("select * from tarifas where idexterno = " + idtarifaexterna, cnn)
                                    {
                                        CommandType = CommandType.Text
                                    };
                                    DataTable _dtt = new DataTable();

                                    var _rdr = _sqlcmd.ExecuteReader();
                                    _dtt.Load(_rdr);

                                    int idtarifa = Convert.ToInt32(_dtt.Rows[0]["id"].ToString());



                                    // Buscar en el DataTable detalle si se encuentra la tarifa
                                    // en caso de estar se hace update, sino se hace insert
                                    string expresion = "idtarifa = " + idtarifa.ToString();
                                    
                                    DataRow rowbusqueda = tarifasXproduccion_detalle.Select(expresion).FirstOrDefault();

                                    if (rowbusqueda != null)
                                    {
                                        // Actualizando cantidad de boletos
                                        //int cantidadanterior = Int32.Parse(rowbusqueda["cantidad"].ToString());
                                        //rowbusqueda["cantidad"] = cantidadanterior + cantidadboletos;
                                        //tarifasXproduccion_detalle.Rows["cantidad"].SetField("cantidad", cantidadanterior + cantidadboletos) ;
                                        //tarifasXproduccion_detalle.Rows[0]["Cantidad"] = cantidadanterior + cantidadboletos;

                                        foreach (DataRow renglon in tarifasXproduccion_detalle.Rows)
                                        {
                                            if (renglon["idtarifa"].ToString() == idtarifa.ToString())
                                            {
                                                int cantidadanterior = Int32.Parse(renglon["cantidad"].ToString());

                                                renglon["cantidad"] = cantidadanterior + cantidadboletos;
                                                break;
                                            }
                                        }


                                    }
                                    else
                                    {
                                        // No se encontró la tarifa, se debe agregar
                                        rowdetalle = tarifasXproduccion_detalle.NewRow();
                                        rowdetalle["idcabecera"] = contadorid;
                                        rowdetalle["idtarifa"] = idtarifa;
                                        rowdetalle["cantidad"] = cantidadboletos;
                                        tarifasXproduccion_detalle.Rows.Add(rowdetalle);
                                        rowdetalle = null;
                                    }


                                    // Ver la vuelta
                                    rowcabecera["vueltas"] = vueltas;
                                    zonainicial = Int32.Parse(cadenatexto.Substring(3, 2));

                                    break;



                                default:
                                    break;
                            }




                        }

                        contador++;
                    }



                    //int prueba = 0;





                    /////// INSERTAR DATOS EN LA BD
                    SqlCommand sqlcmd = new SqlCommand();

                    sqlcmd = new SqlCommand("sp_InsertarProduccion", cnn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@fecha", _fechaProduccion);

                    sqlcmd.Parameters.AddWithValue("@estado","ABIERTO");

                    DataTable _dt = new DataTable();

                    var reader = sqlcmd.ExecuteReader();

                    _dt.Load(reader);

                    int idproduccion = Convert.ToInt32(_dt.Rows[0]["idproduccion"].ToString());

                    ///////////INSERTAR ARCHIVO ///////////

                    sqlcmd = new SqlCommand("sp_InsertarArchivo", cnn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlcmd.Parameters.AddWithValue("@idProduccion", idproduccion);
                    sqlcmd.Parameters.AddWithValue("@nombre", _nombrefile);
                    sqlcmd.Parameters.AddWithValue("@fechacarga", DateTime.Now.ToShortDateString());

                    sqlcmd.ExecuteNonQuery();


                    ///////////
                    foreach (DataRow PD_Fila in produccion_detalle.Rows)
                    {
                        int idCabeceraActual;
                        idCabeceraActual = Convert.ToInt32(PD_Fila["id"].ToString());


                        // if (idCabeceraActual = 56 || idCabeceraActual = 85 idCabeceraActual = )


                        //insertar en la tabla produccion detalle <- dt produccion_detalle

                        sqlcmd = new SqlCommand("sp_InsertarProduccionDetalle", cnn)
                        {
                            CommandType = CommandType.StoredProcedure
                        };


                        var _fecha = PD_Fila["fecha"].ToString();

                        if (_fecha == "") continue;

                       fecha = Convert.ToDateTime(PD_Fila["fecha"].ToString());

                        linea  = Convert.ToInt32(PD_Fila["linea"].ToString());
                        

                         coche = Convert.ToInt32(PD_Fila["coche"].ToString());

                        

                         legajo = Convert.ToInt32(PD_Fila["legajo"].ToString());

                        

                       horainicio = PD_Fila["horainicio"].ToString();
                        horafin = PD_Fila["horafin"].ToString();
                        int controladora = Convert.ToInt32(PD_Fila["numerodecontroladora"].ToString());
                        vueltas = Convert.ToInt32(PD_Fila["vueltas"].ToString());

                        sqlcmd.Parameters.AddWithValue("@idproduccion", idproduccion);
                        sqlcmd.Parameters.AddWithValue("@fecha", fecha);
                        sqlcmd.Parameters.AddWithValue("@linea", linea);
                        sqlcmd.Parameters.AddWithValue("@coche", coche);
                        sqlcmd.Parameters.AddWithValue("@legajo", legajo);
                        sqlcmd.Parameters.AddWithValue("@horainicio", horainicio);
                        sqlcmd.Parameters.AddWithValue("@horafin", horafin);
                        sqlcmd.Parameters.AddWithValue("@controladora", controladora);
                        sqlcmd.Parameters.AddWithValue("@vueltas", vueltas);


                       // cnn.Open();

                        reader = sqlcmd.ExecuteReader();
                        _dt.Clear();
                        _dt.Load(reader);

                        int idProduccionDetalle = Convert.ToInt32(_dt.Rows[0]["idProduccionDetalle"].ToString());

                        // Recorriendo detalle 

                        foreach (DataRow TXP_Fila in tarifasXproduccion_detalle.Rows)
                        {
                            if ( Convert.ToInt32(TXP_Fila["idcabecera"].ToString()) == idCabeceraActual) 
                            {
                                sqlcmd = new SqlCommand("[sp_InsertarTarifaXProduccionDetalle]", cnn)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };


                                int idproducciondetalle = idCabeceraActual;
                                int idtarifa = Convert.ToInt32(TXP_Fila["idtarifa"].ToString());
                                int cantidad = Convert.ToInt32(TXP_Fila["cantidad"].ToString());

                                sqlcmd.Parameters.AddWithValue("@idproducciondetalle", idProduccionDetalle);
                                sqlcmd.Parameters.AddWithValue("@idtarifa", idtarifa);
                                sqlcmd.Parameters.AddWithValue("@cantidad", cantidad);

                                sqlcmd.ExecuteNonQuery();
                            }


                        
                        }
                             









                    }



                    /*
                     foreach(DataRow rowcabecera in tablacabecera.Rows)
                        {
                            int ActualIdCabecera;
                            ActualIdCabecera = rowcabecera["id"];

                            foreach(DataRow rowdetalle in tabladetalle.Rows)
                                {
                                    if (rowdetalle["idcabecera"] = 
                                }
                        }
                    bucle para insertar cabecera
                    {
                        //insertas cabecera
                        //traes ese id recien insertado// 
                           id_cabecera = select max(id) from cabecera
                        
                        bucle para insertar detalle de la cabecera de arriba
                        {
                            insertas detalles con el id de arriba
                            idcabecera = ic_cabecera;
                        }


                    }





                    */
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = "Hubo un problema al Cargar el Archivo Consolidado." + ex;
                    return View();
                }
                finally
                {
                    cnn.Close();
                } // cierre de try catch de cargar archivo


                return View();

                //}
            }
            // return View();
        }
    }
}

