using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConiferApp.Models
{
    public partial class DbCantidadTarifasXlineas : DbContext
    {
        public DbCantidadTarifasXlineas()
            : base("name=DbCantidadTarifasXlinea")
        {
        }

        public virtual DbSet<CantidadTarifasXlineas> CantidadTarifasXlineas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CantidadTarifasXlineas>()
                .Property(e => e.Tipo_de_tarifa)
                .IsUnicode(false);

            modelBuilder.Entity<CantidadTarifasXlineas>()
                .Property(e => e.Línea)
                .IsUnicode(false);
        }
    }
}
