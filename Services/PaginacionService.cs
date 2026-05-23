using ImOdNotes.Models.ViewModels;

namespace ImOdNotes.Services
{
    public class PaginacionService
    {
        public PaginadoViewModel<T> Paginacion<T>(
            IQueryable<T> query, int pagina
            )
        {
            int RegistrosPorPagina = Configuration.PaginaConfig.Get<T>();
            int TotalRegistros = query.Count();
            int TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);

            var Lista = query
                .Skip((pagina - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina)
                .ToList();

            return new PaginadoViewModel<T>
            {
                Lista = Lista,
                PaginaActual = pagina,
                RegistrosPorPagina = RegistrosPorPagina,
                TotalPaginas = TotalPaginas,
                TotalRegistros = TotalRegistros
            };
        }
    }
}
