using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConiferApp.Models
{
    public partial class DbPermisos : DbContext
    {
        public DbPermisos()
            : base("name=DbPermisos")
        {
        }

        public virtual DbSet<Permisos> Permisos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permisos>()
                .Property(e => e.nombre)
                .IsUnicode(false);
        }
    }
}
