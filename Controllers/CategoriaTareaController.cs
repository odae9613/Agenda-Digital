using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImOdNotes.Core.Entities;
using ImOdNotes.Data.Context;

namespace ImOdNotes.Controllers
{
    public class CategoriaTareaController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriaTareaController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaTarea
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriaTareas.ToListAsync());
        }

        // GET: CategoriaTarea/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaTarea = await _context.CategoriaTareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaTarea == null)
            {
                return NotFound();
            }

            return View(categoriaTarea);
        }

        // GET: CategoriaTarea/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaTarea/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] CategoriaTarea categoriaTarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaTarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaTarea);
        }

        // GET: CategoriaTarea/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaTarea = await _context.CategoriaTareas.FindAsync(id);
            if (categoriaTarea == null)
            {
                return NotFound();
            }
            return View(categoriaTarea);
        }

        // POST: CategoriaTarea/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] CategoriaTarea categoriaTarea)
        {
            if (id != categoriaTarea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaTarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaTareaExists(categoriaTarea.Id))
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
            return View(categoriaTarea);
        }

        // GET: CategoriaTarea/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaTarea = await _context.CategoriaTareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaTarea == null)
            {
                return NotFound();
            }

            return View(categoriaTarea);
        }

        // POST: CategoriaTarea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaTarea = await _context.CategoriaTareas.FindAsync(id);
            if (categoriaTarea != null)
            {
                _context.CategoriaTareas.Remove(categoriaTarea);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaTareaExists(int id)
        {
            return _context.CategoriaTareas.Any(e => e.Id == id);
        }
    }
}
