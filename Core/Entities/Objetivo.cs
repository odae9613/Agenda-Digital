using System.ComponentModel.DataAnnotations;
using ImOdNotes.Core.Enums;

namespace ImOdNotes.Core.Entities
{
    public class Objetivo
    {
        public int Id { get; set; }
        [Required, MaxLength(200)] public string Titulo { get; set; } = "";
        public string? Descripcion { get; set; }
        public Progreso Progreso { get; set; } = Progreso.NoIniciado;
        public DateTime? FechaObjetivo { get; set; }
        public bool Completado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public bool Favorito { get; set; } = false;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public CategoriaObjetivo? CategoriaObjetivo { get; set; } = null; // Personal | Health | Finance | Work | Learning
        public int CategoriaId { get; set; }

    }
}
