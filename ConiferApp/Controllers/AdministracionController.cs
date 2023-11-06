using ExcelDataReader;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ConiferApp.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
    {
        private readonly string cadena = ConfigurationManager.AppSettings["CadenaBD"];
        private DataSet dtsTablas = new DataSet();

        public ActionResult Inicio()
        {
            return View();
        }

        private DataTable InsertExcelData(string filename)
        {
            FileStream fsSource = new FileStream(filename, FileMode.Open, FileAccess.Read);

            IExcelDataReader reader = ExcelReaderFactory.CreateReader(fsSource);

            dtsTablas = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            reader.Close();
            return dtsTablas.Tables[0];
        }

        public ActionResult ImportarLineas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarLineas(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarLineas(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public ActionResult ImportarCoches()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarCoches(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarCoches(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public ActionResult ImportarChoferes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarChoferes(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarChoferes(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public ActionResult ImportarTarifas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarTarifas(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarTarifas(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public ActionResult ImportarHojasDeRutas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarHojasDeRutas(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarHojasDeRutas(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public ActionResult ImportarZonas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarZonas(HttpPostedFileBase file)
        {
            string filepath = Server.MapPath("~/Excel/") + file.FileName;
            file.SaveAs(Path.Combine(Server.MapPath("~/Excel/"), file.FileName));

            DataTable data = InsertExcelData(filepath);
            bool resultado = CargarZonas(data);
            if (resultado)
            {
                ViewData["Exito"] = "Se importaron los datos exitosamente";
            }
            else
            {
                ViewData["Error"] = "Hubo un problema al importar los datos";
            }
            return View();
        }

        public bool CargarLineas(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("Nro", "Nro");
                    s.ColumnMappings.Add("Corredor", "Corredor");
                    s.ColumnMappings.Add("Desde", "Desde");
                    s.ColumnMappings.Add("Hasta", "Hasta");

                    s.DestinationTableName = "Lineas";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

        public bool CargarCoches(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("dominio", "dominio");
                    s.ColumnMappings.Add("interno", "interno");
                    s.ColumnMappings.Add("modelo", "modelo");
                    s.ColumnMappings.Add("fechaAlta", "fechaAlta");

                    s.DestinationTableName = "Coches";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

        public bool CargarChoferes(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("legajo", "legajo");
                    s.ColumnMappings.Add("apellido", "apellido");
                    s.ColumnMappings.Add("nombre", "nombre");
                    s.ColumnMappings.Add("cuil", "cuil");
                    s.ColumnMappings.Add("fechaAlta", "fechaAlta");

                    s.DestinationTableName = "Choferes";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

        public bool CargarTarifas(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("Nombre", "Nombre");
                    s.ColumnMappings.Add("Precio", "Precio");
                    s.ColumnMappings.Add("FechaDesde", "FechaDesde");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");

                    s.DestinationTableName = "Tarifas";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

        public bool CargarHojasDeRutas(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("Fecha", "Fecha");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");

                    s.DestinationTableName = "HojasDeRutas";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

        public bool CargarZonas(DataTable tbData)
        {
            bool resultado = true;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(cn))
                {
                    s.ColumnMappings.Add("Descripcion", "Descripcion");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");
                    //s.ColumnMappings.Add("", "");

                    s.DestinationTableName = "Zonas";

                    s.BulkCopyTimeout = 1500;
                    try
                    {
                        s.WriteToServer(tbData);
                    }
                    catch (Exception e)
                    {
                        string st = e.Message;
                        resultado = false;
                    }
                }
            }
            return resultado;
        }

    }
}