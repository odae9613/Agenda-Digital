using System.ComponentModel.DataAnnotations;

namespace ImOdNotes.Core.Entities
{
    public class Evento
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Titulo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFinal { get; set; }
        public string? Ubicacion { get; set; }
        public bool Favorito { get; set; } = false;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int CategoriaId { get; set; }
        public CategoriaEvento? CategoriaEvento { get; set; } = null!;
    }
}
