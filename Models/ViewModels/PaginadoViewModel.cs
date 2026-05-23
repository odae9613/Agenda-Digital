namespace ImOdNotes.Models.ViewModels
{
    public class PaginadoViewModel<T> : IPaginadoViewModel
    {
        public List<T> Lista { get; set; } = new List<T>();
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }

    public interface IPaginadoViewModel
    {
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
