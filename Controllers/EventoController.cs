using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ImOdNotes.Services;

namespace ImOdNotes.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        private readonly PaginacionService _paginacionService;
        private readonly MyDbContext _context;

        public EventoController(MyDbContext context)
        {
            _context = context;
            _paginacionService = new PaginacionService();
        }

        // GET: Evento
        public async Task<IActionResult> Index(int page = 1)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            //  PAGINACIÓN MANUAL
            var eventosPaginacion = _context.Eventos
                .Include(e => e.CategoriaEvento)
                .Include(e => e.Usuario)
                .Where(e => e.UsuarioId == usuarioId);
            var paginado = _paginacionService.Paginacion<Evento>(eventosPaginacion, page);

            return View(paginado);
        }

        // GET: Evento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.CategoriaEvento)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Evento/Create
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            ViewBag.CategoriaId = new SelectList(
                _context.CategoriaEventos
                    .Where(c => c.UsuarioId == usuarioId),
                "Id",
                "Nombre"
            );
            return View();
        }

        // POST: Evento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,FechaInicio,FechaFinal,HoraInicio,HoraFinal,Color,Ubicacion,Favorito,CategoriaId")] Evento evento)
        {
            // Obtener id del usuario autenticado desde los claims
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var usuarioId))
            {
                //return RedirectToAction("Login", "Usuario");
                return Unauthorized();
            }

            evento.UsuarioId = usuarioId;
            // Ignorar validación de propiedades de navegación
            ModelState.Remove("Usuario");
            ModelState.Remove("CategoriaEvento");
            // Validar que se haya seleccionado una categoría
            if (evento.CategoriaId <= 0)
            {
                ModelState.AddModelError("CategoriaId", "Seleccione una categoría.");
            }

            if (ModelState.IsValid)
            {
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-popular el SelectList para que la vista lo muestre correctamente
            ViewBag.CategoriaId = new SelectList(_context.CategoriaEventos, "Id", "Nombre", evento.CategoriaId);
            return View(evento);
        }

        // GET: Evento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }
            var evento = await _context.Eventos
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
            if (evento == null)
            {
                return NotFound();
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaEventos, "Id", "Nombre", evento.CategoriaId);
            return View(evento);
        }

        // POST: Evento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,FechaInicio,FechaFinal,HoraInicio,HoraFinal,Ubicacion,CategoriaId")] Evento evento)
        {
            // Usuario autenticado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            if (id != evento.Id)
                return NotFound();


            // Buscar la tarea original del usuario
            var eventoDb = await _context.Eventos
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (eventoDb == null)
                return NotFound();

            // Actualizar SOLO los campos editables
            eventoDb.Titulo = evento.Titulo;
            eventoDb.Descripcion = evento.Descripcion;
            eventoDb.FechaInicio = evento.FechaInicio;
            eventoDb.FechaFinal = evento.FechaFinal;
            eventoDb.FechaActualizacion = DateTime.Now;
            eventoDb.HoraInicio = evento.HoraInicio;
            eventoDb.HoraFinal = evento.HoraFinal;
            eventoDb.Ubicacion = evento.Ubicacion;
            eventoDb.Favorito = evento.Favorito;
            eventoDb.CategoriaId = evento.CategoriaId;

            // Mantener usuario logeado
            eventoDb.UsuarioId = usuarioId;

            await _context.SaveChangesAsync();
                
            ViewBag.CategoriaId = new SelectList(_context.CategoriaEventos
                .Where(w => w.UsuarioId == usuarioId), "Id", "Nombre", evento.CategoriaId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Evento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.CategoriaEvento)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Evento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Favorito(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }
            Evento? evento = _context.Eventos
                .FirstOrDefault(x => x.Id == id && x.UsuarioId == usuarioId);
            if (evento != null)
            {
                evento.Favorito = !evento.Favorito;
                await _context.SaveChangesAsync();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> ListaFavoritos(int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }
            //  PAGINACIÓN MANUAL
            var eventosPaginacion = _context.Eventos
                .Include(e => e.CategoriaEvento)
                .Include(e => e.Usuario)
                .Where(e => e.UsuarioId == usuarioId && e.Favorito);
            var paginado = _paginacionService.Paginacion<Evento>(eventosPaginacion, page);

            return View(paginado);
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
