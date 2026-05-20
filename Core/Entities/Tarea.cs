using System.ComponentModel.DataAnnotations;
using ImOdNotes.Core.Enums;

namespace ImOdNotes.Core.Entities
{
    public class Tarea
    {
        public int Id { get; set; }
        [Required, MaxLength(300)] public string Titulo { get; set; } = "";
        public string? Descripcion { get; set; }
        public bool Completado { get; set; } = false;
        public bool Favorito { get; set; } = false;
        public Prioridad Prioridad { get; set; } = Prioridad.Baja;
        
        public DateTime? FechaVencimiento { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int CategoriaId { get; set; }
        public CategoriaTarea? CategoriaTarea { get; set; }

    }
}
