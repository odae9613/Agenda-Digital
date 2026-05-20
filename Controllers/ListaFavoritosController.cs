using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Controllers
{
    public class ListaFavoritosController : Controller
    {
        private readonly MyDbContext _context;

        public ListaFavoritosController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int totalEventosFavoritos = await _context.Eventos
                                        .Where(e => e.Favorito)  // Filtra solo los eventos favoritos
                                        .CountAsync();
            int pageSize = 5;
            int totalPaginas = (int)Math.Ceiling((double)totalEventosFavoritos / pageSize);

            ViewBag.TotalRegistros = totalEventosFavoritos;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;

            List<Evento> data = await _context.Eventos
                                .Where(e => e.Favorito)
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return View(data);
        }
    }
}
