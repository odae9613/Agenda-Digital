using System.Diagnostics;
using ImOdNotes.Data.Context;
using ImOdNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            int totalGastos = await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalGastos = totalGastos;
            int totalEventos = await _context.Eventos
                .Where(e => e.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalEventos = totalEventos;
            int totalNotas = await _context.Notas
                .Where(n => n.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalNotas = totalNotas;
            int totalTareas = await _context.Tareas
                .Where(n => n.UsuarioId == usuarioId).CountAsync();
            ViewBag.totalTareas = totalTareas;
            DateTime hoy = DateTime.Now;
            ViewBag.hoy = hoy;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
