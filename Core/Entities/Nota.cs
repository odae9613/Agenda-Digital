using System.ComponentModel.DataAnnotations;

namespace ImOdNotes.Core.Entities
{
    public class Nota
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Titulo { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public bool Favorito { get; set; } = false;
        public DateTime FechaCreacion { get; internal set; }
        public DateTime FechaActualizacion { get; internal set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; } = null!;

        public int CategoriaId { get; set; }
        public CategoriaNota? CategoriaNota { get; set; } = null!;
    }
}
