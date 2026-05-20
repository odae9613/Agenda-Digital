using System.Security.Claims;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ImOdNotes.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly ILogger<CalendarioController> _logger;
        private readonly MyDbContext _context;
        public CalendarioController(ILogger<CalendarioController> logger, MyDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        //public IActionResult Calendar()
        //{
        //    return View();
        //}
        
        public IActionResult Calendar()
        {
            List<object> items = new List<object>();

            ViewBag.User = User.Identity.Name;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }


            // EVENTOS
            var eventos = _context.Eventos
                .Where(w => w.UsuarioId == usuarioId)
                .ToList();

            foreach (var evento in eventos)
            {
                items.Add(new
                {
                    title = evento.Titulo,
                    start = evento.FechaInicio,
                    end = evento.FechaInicio.AddHours(1),

                    backgroundColor = "#8FB9B3",
                    borderColor = "#6D9B95",
                    textColor = "#ffffff",

                    extendedProps = new
                    {
                        tipo = "Evento",
                        descripcion = evento.Descripcion
                    }
                });
            }

            // TAREAS
            var tareas = _context.Tareas
                .Where(w => w.UsuarioId == usuarioId)
                .ToList();

            foreach (var tarea in tareas)
            {
                if (tarea.FechaVencimiento != null)
                {
                    items.Add(new
                    {
                        title = tarea.Titulo,
                        start = tarea.FechaVencimiento,

                        backgroundColor = "#D6B6D5",
                        borderColor = "#B58DB6",

                        extendedProps = new
                        {
                            tipo = "Tarea",
                            descripcion = tarea.Descripcion
                        }
                    });
                }
            }
            //ViewBag.idUsuario = Id;
            ViewBag.Eventos = JsonConvert.SerializeObject(items);

            return View();
        }
    }
}
