using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace ConiferApp.Models
{
    public class ProduccionDetalle
    {
        public int id { get; set; }
        public int idproduccion { get; set; }
        public DateTime fecha { get; set; }
        public int linea { get; set; }
        public int coche { get; set; }
        public int legajo { get; set; }
        public string horainicio { get; set; }
        public string horafin { get; set; }
        public string controladora { get; set; }
        public decimal vueltas { get; set; }

       




    }
}