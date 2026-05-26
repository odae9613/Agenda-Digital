using System;
using System.Collections.Generic;
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
    public class CategoriaEventoController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriaEventoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaEvento
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdClaim, out int usuarioId);

            return View(await _context.CategoriaEventos
                .Where(w => w.UsuarioId == usuarioId)
                .ToListAsync());
        }

        // GET: CategoriaEvento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaEvento = await _context.CategoriaEventos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaEvento == null)
            {
                return NotFound();
            }

            return View(categoriaEvento);
        }

        // GET: CategoriaEvento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaEvento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] CategoriaEvento categoriaEvento)
        {
            ViewBag.User = User.Identity.Name;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                int.TryParse(userIdClaim, out int userId);
                categoriaEvento.UsuarioId = userId;

                _context.Add(categoriaEvento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaEvento);
        }

        // GET: CategoriaEvento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaEvento = await _context.CategoriaEventos.FindAsync(id);
            if (categoriaEvento == null)
            {
                return NotFound();
            }
            return View(categoriaEvento);
        }

        // POST: CategoriaEvento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] CategoriaEvento categoriaEvento)
        {
            if (id != categoriaEvento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaEvento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaEventoExists(categoriaEvento.Id))
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
            return View(categoriaEvento);
        }

        // GET: CategoriaEvento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaEvento = await _context.CategoriaEventos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaEvento == null)
            {
                return NotFound();
            }

            return View(categoriaEvento);
        }

        // POST: CategoriaEvento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaEvento = await _context.CategoriaEventos.FindAsync(id);
            if (categoriaEvento != null)
            {
                _context.CategoriaEventos.Remove(categoriaEvento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaEventoExists(int id)
        {
            return _context.CategoriaEventos.Any(e => e.Id == id);
        }
    }
}
