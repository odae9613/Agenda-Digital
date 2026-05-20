using System.ComponentModel.DataAnnotations;

namespace ImOdNotes.Core.Entities
{
    public class Gasto
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Titulo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public string? Notas { get; set; }
        public string Tipo { get; set; } = "Gasto"; // Ingreso | Gasto
        public bool Favorito { get; set; } = false;
        public int CategoriaId { get; set; }
        public CategoriaGasto? CategoriaGasto { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; } = null!;
    }
}
