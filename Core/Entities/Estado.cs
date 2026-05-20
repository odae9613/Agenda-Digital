namespace ImOdNotes.Core.Entities
{
    public class Estado
    {
        public int Id { get; set; }
        required public string Nombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}