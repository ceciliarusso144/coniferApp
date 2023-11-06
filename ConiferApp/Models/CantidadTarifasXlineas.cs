namespace ConiferApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CantidadTarifasXlineas
    {
        public int id { get; set; }

        [Column("Tipo de tarifa")]
        [StringLength(50)]
        public string Tipo_de_tarifa { get; set; }

        [Column("Cantidad de boletos")]
        public int? Cantidad_de_boletos { get; set; }

        [StringLength(50)]
        public string Línea { get; set; }
    }
}
