using System.Diagnostics;
using ImOdNotes.Data.Context;
using ImOdNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using ImOdNotes.Core.Entities;
using ImOdNotes.DTO;

namespace ImOdNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Inicio()
        {
            ViewBag.usuario = User.Identity.Name;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }
            ViewBag.usuarioId = usuarioId;

            DateTime hoy = DateTime.Today;
            ViewBag.hoy = hoy;
            /*  GASTOS  */
            //Todos los gastos
            int totalGastos = await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalGastos = totalGastos;
            /*  EVENTOS */
            //  Cantidad total de todos los eventos
            int totalEventos = await _context.Eventos   
                .Where(e => e.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalEventos = totalEventos;
            //  Lista de todos los eventos de hoy
            List<Evento> listaEventos = await _context.Eventos
                .Where(e => e.UsuarioId == usuarioId && e.FechaInicio == hoy).ToListAsync();
            ViewBag.listaEventos = listaEventos;
            //  Eventos de hoy
            int eventosHoy = await _context.Eventos 
                .Where(w => w.UsuarioId == usuarioId && w.FechaInicio == hoy).CountAsync();
            ViewBag.eventosHoy = eventosHoy;
            const int SEMANA = 7;
            DateTime restarSemana = DateTime.Now.AddDays(SEMANA);
            //  Eventos de la semana... Notificación
            List<Evento> eventoSemana = await _context.Eventos 
                .Where(e => e.UsuarioId == usuarioId && e.FechaInicio >= DateTime.Now && e.FechaInicio <= restarSemana).ToListAsync();
            ViewBag.eventoSemana = eventoSemana;
            //  Eventos del mes
            List<Evento> eventoMes = await _context.Eventos 
                .Where(e => e.UsuarioId == usuarioId && e.FechaInicio.Month == DateTime.Now.Month).ToListAsync();
            ViewBag.eventoMes = eventoMes;
            /*  NOTAS   */
            int totalNotas = await _context.Notas
                .Where(n => n.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalNotas = totalNotas;
            /*  TAREAS    */
            //  Todas las tareas
            int totalTareas = await _context.Tareas 
                .Where(n => n.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalTareas = totalTareas;
            //  Lista de todos los eventos de hoy
            List<Tarea> listaTareas = await _context.Tareas
                .Where(e => e.UsuarioId == usuarioId && e.FechaVencimiento == hoy).ToListAsync();
            ViewBag.listaTareas = listaTareas;
            //  Tareas hoy
            int tareasHoy = await _context.Tareas
                .Where(w => w.UsuarioId == usuarioId && w.FechaVencimiento == DateTime.Today).CountAsync();
            ViewBag.tareasHoy = tareasHoy;
            //  Tareas de la semana
            List<Tarea> tareasSemana = await _context.Tareas
                .Where(e => e.UsuarioId == usuarioId && e.FechaVencimiento >= DateTime.Now && e.FechaVencimiento <= restarSemana).ToListAsync();
            ViewBag.tareasSemana = tareasSemana;
            //  Tareas del mes
            List<Tarea> tareaMes = await _context.Tareas
                .Where(e => e.UsuarioId == usuarioId && e.FechaVencimiento.HasValue && e.FechaVencimiento.Value.Month == DateTime.Now.Month).ToListAsync();
            ViewBag.tareaMes = tareaMes;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado([FromBody] CambiarEstadoDto model)
        {
            var tarea = _context.Tareas.Find(model.Id);

            if (tarea == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No encontrada"
                });
            }

            tarea.Completado = model.Completado;

            _context.SaveChanges();

            return Json(new
            {
                success = true
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
