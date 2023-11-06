using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ConiferApp.Models
{
    public partial class DbRoles : DbContext
    {
        public DbRoles()
            : base("name=DbRoles")
        {
        }

        public virtual DbSet<Roles> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.descripcion)
                .IsUnicode(false);
        }
    }
}
