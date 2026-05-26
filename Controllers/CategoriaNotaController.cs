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
    public class CategoriaNotaController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriaNotaController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaNota
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdClaim, out int usuarioId);

            return View(await _context.CategoriaNotas
                .Where(w => w.UsuarioId == usuarioId)
                .ToListAsync());
        }

        // GET: CategoriaNota/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaNota = await _context.CategoriaNotas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaNota == null)
            {
                return NotFound();
            }

            return View(categoriaNota);
        }

        // GET: CategoriaNota/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaNota/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] CategoriaNota categoriaNota)
        {
            ViewBag.User = User.Identity.Name;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                int.TryParse(userIdClaim, out int userId);
                categoriaNota.UsuarioId = userId;

                _context.Add(categoriaNota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaNota);
        }

        // GET: CategoriaNota/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaNota = await _context.CategoriaNotas.FindAsync(id);
            if (categoriaNota == null)
            {
                return NotFound();
            }
            return View(categoriaNota);
        }

        // POST: CategoriaNota/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] CategoriaNota categoriaNota)
        {
            if (id != categoriaNota.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaNota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaNotaExists(categoriaNota.Id))
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
            return View(categoriaNota);
        }

        // GET: CategoriaNota/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaNota = await _context.CategoriaNotas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaNota == null)
            {
                return NotFound();
            }

            return View(categoriaNota);
        }

        // POST: CategoriaNota/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaNota = await _context.CategoriaNotas.FindAsync(id);
            if (categoriaNota != null)
            {
                _context.CategoriaNotas.Remove(categoriaNota);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaNotaExists(int id)
        {
            return _context.CategoriaNotas.Any(e => e.Id == id);
        }
    }
}
