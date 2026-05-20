using System.ComponentModel.DataAnnotations;

namespace ImOdNotes.Core.Entities
{
    public class CategoriaEvento
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
        public int UsuarioId { get; internal set; }
    }
}
