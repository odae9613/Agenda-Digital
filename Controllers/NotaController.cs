using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ImOdNotes.Controllers
{
    public class NotaController : Controller
    {
        private readonly MyDbContext _context;

        public NotaController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Notas
        public async Task<IActionResult> Index(int page = 1)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            //
            int totalNotasFavoritos = await _context.Notas
                                        .Where(e => e.Favorito && e.UsuarioId == usuarioId)  // Filtra solo las notas favoritos del usuario
                                        .CountAsync();
            int pageSize = 5;
            int totalPaginas = (int)Math.Ceiling((double)totalNotasFavoritos / pageSize);
            ViewBag.TotalRegistros = totalNotasFavoritos;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;

            List<Nota> data = await _context.Notas
                                .Where(e => e.Favorito && e.UsuarioId == usuarioId)
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            //

            var nota = _context.Notas
                .Include(n => n.CategoriaNota)
                .Include(n => n.Usuario)
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();
            return View(await nota);
        }

        // GET: Notas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Notas
                .Include(n => n.CategoriaNota)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // GET: Notas/Create
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            ViewBag.CategoriaId = new SelectList(
                _context.CategoriaNotas
                    .Where(c => c.UsuarioId == usuarioId),
                "Id",
                "Nombre"
            );
            return View();
        }

        // POST: Notas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Contenido,Favorito,IsPinned,Color,FechaCreacion,FechaActualizacion,CategoriaId")] Nota nota)
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

            nota.UsuarioId = usuarioId;
            // Ignorar validación de propiedades de navegación
            ModelState.Remove("Usuario");
            ModelState.Remove("CategoriaNota");
            // Validar que se haya seleccionado una categoría
            if (nota.CategoriaId <= 0)
            {
                ModelState.AddModelError("CategoriaId", "Seleccione una categoría.");
            }

            if (ModelState.IsValid)
            {
                nota.FechaCreacion = DateTime.Now;
                nota.FechaActualizacion = DateTime.Now;
                _context.Notas.Add(nota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaNotas.Where(w => w.UsuarioId == usuarioId), "Id", "Nombre", nota.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", nota.UsuarioId);
            return View(nota);
        }

        // GET: Notas/Edit/5
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

            var nota = await _context.Notas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
            if (nota == null)
            {
                return NotFound();
            }
            ViewBag.CategoriaId = new SelectList(_context.CategoriaNotas, "Id", "Nombre", nota.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", nota.UsuarioId);
            return View(nota);
        }

        // POST: Notas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Contenido,Favorito,IsPinned,Color,FechaCreacion,FechaActualizacion,CategoriaId")] Nota nota)
        {
            if (id != nota.Id)
            {
                return NotFound();
            }

            // Usuario autenticado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized();
            }

            // Buscar la nota original del usuario
            var notaDb = await _context.Notas
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

            if (notaDb == null)
            {
                return NotFound();
            }

            // Actualizar SOLO los campos editables
            notaDb.Titulo = nota.Titulo;
            notaDb.Contenido = nota.Contenido;
            notaDb.Favorito = nota.Favorito;
            notaDb.CategoriaId = nota.CategoriaId;

            // Mantener usuario logeado
            notaDb.UsuarioId = usuarioId;
            // Fecha actualización
            notaDb.FechaActualizacion = DateTime.Now;


            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(nota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaExists(nota.Id))
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
            ViewBag.CategoriaId = new SelectList(_context.CategoriaNotas, "Id", "Nombre", nota.CategoriaId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido01", nota.UsuarioId);
            return View(nota);
        }

        // GET: Notas/Delete/5
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

            var nota = await _context.Notas
                .Include(n => n.CategoriaNota)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // POST: Notas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nota = await _context.Notas.FindAsync(id);
            if (nota != null)
            {
                _context.Notas.Remove(nota);
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
            Nota nota = _context.Notas
                .FirstOrDefault(x => x.Id == id && x.UsuarioId == usuarioId);
            if (nota != null)
            {
                nota.Favorito = !nota.Favorito;
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

            int totalNotasFavoritos = await _context.Notas
                                        .Where(e => e.Favorito && e.UsuarioId == usuarioId)  // Filtra solo las notas favoritos del usuario
                                        .CountAsync();
            int pageSize = 5;
            int totalPaginas = (int)Math.Ceiling((double)totalNotasFavoritos / pageSize);
            ViewBag.TotalRegistros = totalNotasFavoritos;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;

            List<Nota> data = await _context.Notas
                                .Where(e => e.Favorito && e.UsuarioId == usuarioId)
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return View(data);
        }

        private bool NotaExists(int id)
        {
            return _context.Notas.Any(e => e.Id == id);
        }
    }
}
