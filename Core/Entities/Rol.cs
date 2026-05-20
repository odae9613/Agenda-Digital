namespace ImOdNotes.Core.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
