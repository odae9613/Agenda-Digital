using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using ImOdNotes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Controllers
{
    public class TareaController : Controller
    {
        private readonly MyDbContext _context;
        private readonly PaginacionService _paginationService;
        public TareaController(MyDbContext context)
        {
            _context = context;
            _paginationService = new PaginacionService();
        }

        // GET: Tarea
        public async Task<IActionResult> Index(int page = 1)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            //  PAGINACIÓN MANUAL   
            var tareasQuery = _context.Tareas
                .Include(t => t.CategoriaTarea)
                .Include(t => t.Usuario)
                .Where(t => t.UsuarioId == usuarioId);
            var paginado = _paginationService.Paginacion<Tarea>(tareasQuery, page);

            return View(paginado);
        }

        // GET: Tarea/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .Include(t => t.CategoriaTarea)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // GET: Tarea/Create
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            ViewBag.CategoriaId = new SelectList(
                _context.CategoriaTareas
                    .Where(c => c.UsuarioId == usuarioId),
                "Id",
                "Nombre"
            );
            return View();
        }

        // POST: Tarea/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Completado,Prioridad,FechaVencimiento,FechaCreacion,FechaActualizacion,CategoriaId")] Tarea tarea)
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

            tarea.UsuarioId = usuarioId;
            // Ignorar validación de propiedades de navegación
            ModelState.Remove("Usuario");
            ModelState.Remove("CategoriaTarea");
            // Validar que se haya seleccionado una categoría
            if (tarea.CategoriaId <= 0)
            {
                ModelState.AddModelError("CategoriaId", "Seleccione una categoría.");
            }

            if (ModelState.IsValid)
            {
                tarea.FechaCreacion = DateTime.Now;
                tarea.FechaActualizacion = DateTime.Now; 

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaTareas, "Id", "Nombre", tarea.CategoriaId);
            //ViewBag.UsuarioId = new SelectList(_context.Usuarios, "Id", "Apellido01", tarea.UsuarioId);
            return View(tarea);
        }

        // GET: Tarea/Edit/5
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

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
            if (tarea == null)
            {
                return NotFound();
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaTareas
                .Where(w => w.UsuarioId == usuarioId), "Id", "Nombre", tarea.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", tarea.UsuarioId);
            return View(tarea);
        }

        // POST: Tarea/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Completado,Prioridad,FechaVencimiento,FechaCreacion,FechaActualizacion,CategoriaId")] Tarea tarea)
        {
            // Obtener id del usuario autenticado desde los claims
            if (id != tarea.Id)
            {
                return NotFound();
            }

            // Usuario autenticado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            // Buscar la tarea original del usuario
            var tareaDb = await _context.Tareas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tareaDb == null)
            {
                return NotFound();
            }

            // Actualizar SOLO los campos editables
            tareaDb.Titulo = tarea.Titulo;
            tareaDb.Descripcion = tarea.Descripcion;
            tareaDb.Completado = tarea.Completado;
            tareaDb.Favorito = tarea.Favorito;
            tareaDb.Prioridad = tarea.Prioridad;
            tareaDb.FechaVencimiento = tarea.FechaVencimiento;
            tareaDb.CategoriaId = tarea.CategoriaId;

            // Mantener usuario logeado
            tareaDb.UsuarioId = usuarioId;

            // Fecha actualización
            tareaDb.FechaActualizacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaTareas, "Id", "Nombre", tarea.CategoriaId);
            return View(tarea);
        }

        // GET: Tarea/Delete/5
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

            var tarea = await _context.Tareas
                .Include(t => t.CategoriaTarea)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // POST: Tarea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);
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
            Tarea tarea = _context.Tareas
                .FirstOrDefault(x => x.Id == id && x.UsuarioId == usuarioId);
            if (tarea != null)
            {
                tarea.Favorito = !tarea.Favorito;
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

            int totalTareasFavoritos = await _context.Tareas
                                        .Where(e => e.Favorito && e.UsuarioId == usuarioId)  // Filtra solo las tareas favoritos del usuario
                                        .CountAsync();
            int pageSize = 5;
            int totalPaginas = (int)Math.Ceiling((double)totalTareasFavoritos / pageSize);
            ViewBag.TotalRegistros = totalTareasFavoritos;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;

            List<Tarea> data = await _context.Tareas
                                .Where(e => e.Favorito && e.UsuarioId == usuarioId)
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return View(data);
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }
}
