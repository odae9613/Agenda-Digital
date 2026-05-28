using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ImOdNotes.Core.Entities;
using ImOdNotes.Core.Enums;

namespace ImOdNotes.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;
        [Required]
        public string Apellido01 { get; set; } = string.Empty;
        public string? Apellido02 { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaNacimiento { get; set; }
        [NotMapped]
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;

                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;

                return edad;
            }
        }
        public string? LugarNacimiento { get; set; }
        public string? Nacionalidad { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        public Genero Genero { get; set; } = Genero.Otro;
        public int RolId { get; set; }
        public Rol? Rol { get; set; } = null!;
        //public Rol? Rol { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? URLFotoPerfil { get; set; }
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
        public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
        public ICollection<Objetivo> Objetivos { get; set; } = new List<Objetivo>();
    }
    public enum Genero
    {
        Masculino,
        Femenino,
        Otro
    }
}
