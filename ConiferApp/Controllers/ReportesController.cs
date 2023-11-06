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
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }
      
        public DataTable CantidadTarifasXlinea(int id)
        {
            SqlConnection cn = new SqlConnection(cadena);
            cn.Open();
            string consulta;
            try
            {
                #region Obtener Tabla
                DataTable tabla = new DataTable();
                SqlCommand cmd = new SqlCommand();
                consulta = "select t.Nombre, c.cantidad" +
                           "from Tarifas t, ProduccionDetalle pd, TarifasXProduccionDetalle c" +
                           "where t.Id=c.idtarifa and c.idproducciondetalle=pd.id and pd.linea=@id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = consulta;             
                cmd.Connection = cn;
                SqlDataReader dr=cmd.ExecuteReader();

                if(dr.HasRows)
                {
                    while(dr.Read()) 
                    {
                        tabla.Rows.Add(
                            dr["Id"].ToString(),
                            dr["Nombre"].ToString(),
                            dr["cantidad"].ToString()
                            );
                    }
                }
                //SqlDataAdapter da = new SqlDataAdapter(cmd);              
                //da.Fill(tabla);
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
       
        public ActionResult CantidadTarifas()
        {
            DataTable tabla = CantidadTarifasXlinea(65);
            return View(tabla);
        }
     
    }
}
