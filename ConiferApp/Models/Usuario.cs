using System;
using System.ComponentModel.DataAnnotations;

namespace ConiferApp.Models
{
    public class Usuario
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int Legajo { get; set; }
       
        public string Apellido { get; set; }

        public string Nombre { get; set; }

        public int DNI { get; set; }

        public Boolean Activo { get; set; }

        public int Rol { get; set; }


        [Required]
        public string Clave { get; set; }

        [Required]
        public string ConfirmarClave { get; set; }

        public string Nombre_y_Apellido()
        {
            return Nombre + " " + Apellido;
        }
    }
    
}