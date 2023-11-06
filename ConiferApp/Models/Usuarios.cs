namespace ConiferApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MinLength(3, ErrorMessage ="El nombre debe contener al menos 3 caracteres")]
        //[RegularExpression("^[a-zA-Z]$", ErrorMessage = "Debe ingresar un nombre válido")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "El apellido debe contener al menos 3 caracteres")]
        //[RegularExpression("^[a-zA-Z]$", ErrorMessage = "Debe ingresar un apellido válido")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Debe ingresar un DNI correcto (sin puntos)")]
        public int? DNI { get; set; }

        [Required]
        [RegularExpression("^[0-9]{4}$", ErrorMessage ="Debe ingresar un legajo de 4 dígitos (sin punto)")]
        public int Legajo { get; set; }

        [Required]
        [Display(Name ="Contraseña")]
        [StringLength(500)]
        public string Clave { get; set; }

        public Boolean Activo { get; set; }
    }
}
