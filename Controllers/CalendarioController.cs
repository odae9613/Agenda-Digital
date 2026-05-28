using System.Security.Claims;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ImOdNotes.Controllers
{
    [Authorize]
    [Route("Calendario")]
    public class CalendarioController : Controller
    {
        private readonly ILogger<CalendarioController> _logger;
        private readonly MyDbContext _context;
        public CalendarioController(ILogger<CalendarioController> logger, MyDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Calendar()
        {
            return View();
        }
        [HttpGet("GetTareasEventos")]
        public IActionResult GetTareasEventos()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            var eventos = _context.Eventos
                .Where(e => e.UsuarioId == usuarioId && e.FechaInicio != null)
                .Select(e => new
                {
                    id = e.Id,
                    title = e.Titulo,
                    start = e.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),

                    backgroundColor = "#8FB9B3",
                    borderColor = "#6D9B95",
                    textColor = "#ffffff",

                    extendedProps = new
                    {
                        tipo = "Evento",
                        descripcion = e.Descripcion
                    }
                })
                .ToList();

            var tareas = _context.Tareas
                .Include(t => t.CategoriaTarea)
                .Where(t => t.UsuarioId == usuarioId &&
                            t.FechaVencimiento != null)
                .Select(t => new
                {
                    id = t.Id,
                    title = t.Titulo,
                    start = t.FechaVencimiento.Value.ToString("yyyy-MM-ddTHH:mm:ss"),

                    backgroundColor = "#D6B6D5",
                    borderColor = "#B58DB6",
                    textColor = "#ffffff",

                    extendedProps = new
                    {
                        tipo = "Tarea",
                        descripcion = t.Descripcion
                    }
                })
                .ToList();

            var items = eventos.Cast<object>()
                .Concat(tareas.Cast<object>())
                .ToList();

            return Json(items);
        }
    }
}
