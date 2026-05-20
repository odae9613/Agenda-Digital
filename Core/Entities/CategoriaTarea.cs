using System.ComponentModel.DataAnnotations;

namespace ImOdNotes.Core.Entities
{
    public class CategoriaTarea
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int UsuarioId { get; internal set; }
    }
}
