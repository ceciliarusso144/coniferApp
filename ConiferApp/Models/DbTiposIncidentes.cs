using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConiferApp.Models
{
    public partial class DbTiposIncidentes : DbContext
    {
        public DbTiposIncidentes()
            : base("name=DbTiposIncidentes")
        {
        }

        public virtual DbSet<TiposIncidentes> TiposIncidentes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TiposIncidentes>()
                .Property(e => e.Nombre)
                .IsUnicode(false);
        }
    }
}
